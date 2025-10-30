using AutoMapper;
using ECommerce.Core.Common.Pagination;
using ECommerce.Core.Common.SpecsParams;
using ECommerce.Core.Dtos.ProductDtos;
using ECommerce.Core.Dtos.WishlistDtos;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.WishlistModule;
using ECommerce.Core.Specifications.WishlistItemSpecifications;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services;

public class WishlistService(IUnitOfWork unitOfWork, IMapper mapper, ICultureService cultureService)
    : IWishlistService
{
    public async Task<PaginationResult<WishlistItemResult>> GetUserWishListItemsAsync(
        WishlistItemSpecsParams specsParams)
    {
        var canTranslate = cultureService.CanTranslate;

        WishlistItemSpecs wishlistItemsSpecs = new WishlistItemSpecs(specsParams: specsParams, enablePagination: true,
            enableSorting: true, enableTracking: false, enableSplittingQuery: canTranslate);

        wishlistItemsSpecs.IncludeRelatedData(
            x => x.Product,
            x => x.Product.Category,
            x => x.Product.Brand
        );

        if (canTranslate)
        {
            wishlistItemsSpecs.IncludeRelatedData(
                x => x.Product.Translations,
                x => x.Product.Category.Translations,
                x => x.Product.Brand.Translations
            );
        }

        var wishlistItems = await unitOfWork
            .Repository<WishlistItem>()
            .GetAllAsync(wishlistItemsSpecs);

        IReadOnlyList<WishlistItemResult> result =
        [
            .. wishlistItems.Select(wishlistItem => new WishlistItemResult()
            {
                Product = mapper.Map<ProductResult>(wishlistItem.Product),
                CreatedAt = wishlistItem.CreatedAt
            })
        ];

        return new PaginationResult<WishlistItemResult>
        {
            PageNumber = specsParams.PageNumber,
            PageSize = specsParams.PageSize,
            Total = await GetTotalAsync(specsParams.UserId),
            Results = result,
        };
    }

    public async Task<bool> AddToWishListAsync(WishlistItemInput input, string userId)
    {
        var alreadyAdded = await GetWishlistItemAsync(input.ProductId, userId, track: false);

        if (alreadyAdded is not null) return false;

        unitOfWork.Repository<WishlistItem>().Add(new WishlistItem
        {
            UserId = userId,
            ProductId = input.ProductId,
            CreatedAt = DateTime.UtcNow
        });

        await unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteFromWishlistAsync(WishlistItemInput input, string userId)
    {
        var existingWishlistItem = await GetWishlistItemAsync(input.ProductId, userId, track: true);
        if (existingWishlistItem is null) return false;

        unitOfWork
            .Repository<WishlistItem>()
            .Delete(existingWishlistItem);

        await unitOfWork.SaveChangesAsync();
        return true;
    }

    private async Task<WishlistItem?> GetWishlistItemAsync(int productId, string userId, bool track)
    {
        WishlistItemSpecs specs = new WishlistItemSpecs(
            specsParams: new WishlistItemSpecsParams { ProductId = productId, UserId = userId, },
            enablePagination: false, enableSorting: false, enableTracking: track, enableSplittingQuery: false);

        return await unitOfWork
            .Repository<WishlistItem>()
            .GetAsync(specs, checkLocalCache: false);
    }

    public async Task<bool> ClearWishlistAsync(string userId)
    {
        WishlistItemSpecs wishlistItemsSpecs = new WishlistItemSpecs(
            specsParams: new WishlistItemSpecsParams { UserId = userId },
            enablePagination: false, enableSorting: false, enableTracking: true, enableSplittingQuery: false);

        var wishlistItems = await unitOfWork
            .Repository<WishlistItem>()
            .GetAllAsync(wishlistItemsSpecs);

        await unitOfWork.BeginTransactionAsync();
        try
        {
            foreach (var item in wishlistItems)
            {
                unitOfWork.Repository<WishlistItem>().Delete(item);
            }

            await unitOfWork.CommitTransactionAsync();
            return true;
        }
        catch (Exception)
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<int> GetTotalAsync(string userId)
    {
        WishlistItemSpecs totalSpecs = new WishlistItemSpecs(
            specsParams: new WishlistItemSpecsParams { UserId = userId },
            enablePagination: false, enableSorting: false, enableTracking: false, enableSplittingQuery: false);

        return await unitOfWork
            .Repository<WishlistItem>()
            .CountAsync(totalSpecs);
    }

    public async Task<IReadOnlyList<int>> GetWishlistProductIdsAsync(string userId)
    {
        WishlistItemSpecs wishlistItemsSpecs = new WishlistItemSpecs(
            specsParams: new WishlistItemSpecsParams { UserId = userId },
            enablePagination: false, enableSorting: false, enableTracking: false, enableSplittingQuery: false);

        var query = unitOfWork
            .Repository<WishlistItem>()
            .GetAllAsQueryable(wishlistItemsSpecs);

        IReadOnlyList<int> ids = await query.Select(x => x.ProductId).ToListAsync();

        return ids;
    }
}