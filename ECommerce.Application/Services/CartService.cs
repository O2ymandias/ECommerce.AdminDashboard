using AutoMapper;
using ECommerce.Core.Common.Constants;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.CartDtos;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Repositories;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.CartModule;
using ECommerce.Core.Models.ProductModule;
using ECommerce.Core.Specifications.ProductSpecifications;
using Microsoft.Extensions.Localization;

namespace ECommerce.Application.Services;

public class CartService(
    IRedisRepository redisRepo,
    IProductService productService,
    IUnitOfWork unitOfWork,
    IStringLocalizer<CartService> localizer,
    IMapper mapper)
    : ICartService
{
    public async Task<CartResult?> GetCartAsync(string cartId)
    {
        var cart = await redisRepo.GetAsync<Cart>(cartId);
        return cart is not null
            ? mapper.Map<CartResult>(cart)
            : null;
    }

    public async Task<CartUpdateResult> AddToCartAsync(CartItemInput item)
    {
        var cart = await redisRepo.GetAsync<Cart>(item.CartId) ?? new Cart(item.CartId);

        var productSpecs = new ProductSpecs(
            specsParams: new ProductSpecsParams { ProductId = item.ProductId },
            enablePagination: false,
            enableSorting: false,
            enableTracking: false,
            enableSplittingQuery: true
        );

        productSpecs.IncludeRelatedData(p => p.Translations);

        var product = await unitOfWork
            .Repository<Product>()
            .GetAsync(productSpecs);

        var result = new CartUpdateResult();

        if (product is null)
        {
            result.Updated = false;
            result.Message = localizer[L.Cart.ProductNotFound];
            return result;
        }

        if (product.UnitsInStock == 0)
        {
            result.Updated = false;
            result.Message = localizer[L.Cart.OutOfStock, item.ProductId];
            return result;
        }

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existingItem is not null)
        {
            var maxQty = await productService.GetMaxOrderQuantityAsync(item.ProductId);
            existingItem.Quantity = Math.Min(existingItem.Quantity + 1, maxQty);

            result.Message = localizer[L.Cart.IncreaseQuantity, item.ProductId, existingItem.Quantity];
        }
        else
        {
            cart.Items.Add(new CartItem()
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductPictureUrl = product.PictureUrl,
                ProductPrice = product.Price,
                Quantity = 1,

                NameTranslations = product.Translations.ToDictionary(x => x.LanguageCode.ToString(), x => x.Name)
            });

            result.Message = localizer[L.Cart.AddSuccess, item.ProductId];
        }

        var isSet = await redisRepo.SetAsync(cart.Id, cart);

        if (isSet)
        {
            result.Updated = true;
            return result;
        }
        else
        {
            result.Updated = false;
            result.Message = localizer[L.Cart.AddFailed, item.ProductId];
            return result;
        }
    }

    public async Task<CartUpdateResult> RemoveFromCartAsync(CartItemInput item)
    {
        var result = new CartUpdateResult();

        var cart = await redisRepo.GetAsync<Cart>(item.CartId);

        if (cart is null)
        {
            result.Updated = false;
            result.Message = localizer[L.Cart.CartNotFound];
            return result;
        }

        if (!cart.Items.Any(i => i.ProductId == item.ProductId))
        {
            result.Updated = false;
            result.Message = localizer[L.Cart.ProductNotInCart, item.ProductId];
            return result;
        }

        cart.Items.RemoveAll(i => i.ProductId == item.ProductId);

        var isSet = await redisRepo.SetAsync(cart.Id, cart);
        if (isSet)
        {
            result.Updated = true;
            result.Message = localizer[L.Cart.RemoveSuccess, item.ProductId];
            return result;
        }
        else
        {
            result.Updated = false;
            result.Message = localizer[L.Cart.RemoveFailed, item.ProductId];
            return result;
        }
    }

    public async Task<CartUpdateResult> UpdateQuantityAsync(UpdateQuantityInput item)
    {
        if (item.NewQuantity <= 0)
            return new CartUpdateResult
                { Message = localizer[L.Cart.QuantityMustBeGreaterThanZero, item.NewQuantity], };

        var cart = await redisRepo.GetAsync<Cart>(item.CartId);
        if (cart is null) return new CartUpdateResult { Message = localizer[L.Cart.CartNotFound, item.CartId], };

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existingItem is null)
            return new CartUpdateResult { Message = localizer[L.Cart.ProductNotInCart, item.ProductId] };

        var maxQty = await productService.GetMaxOrderQuantityAsync(item.ProductId);

        existingItem.Quantity = Math.Min(item.NewQuantity, maxQty);

        var isSet = await redisRepo.SetAsync(cart.Id, cart);

        if (isSet)
            return new CartUpdateResult
            {
                Updated = true,
                Message = localizer[L.Cart.UpdateQuantitySuccess, item.ProductId, existingItem.Quantity]
            };

        return new CartUpdateResult { Message = localizer[L.Cart.UpdateQuantityFailed, item.CartId, item.ProductId] };
    }

    public async Task<bool> DeleteCartAsync(string id) => await redisRepo.DeleteAsync(id);
}