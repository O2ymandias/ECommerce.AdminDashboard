using AutoMapper;
using ECommerce.Application.Maps.Resolvers.AuthResolvers;
using ECommerce.Application.Maps.Resolvers.BrandResolvers;
using ECommerce.Application.Maps.Resolvers.CartResolvers;
using ECommerce.Application.Maps.Resolvers.CategoryResolvers;
using ECommerce.Application.Maps.Resolvers.DeliveryMethodResolvers;
using ECommerce.Application.Maps.Resolvers.GalleryResolvers;
using ECommerce.Application.Maps.Resolvers.OrderResolvers;
using ECommerce.Application.Maps.Resolvers.ProductResolvers;
using ECommerce.Application.Maps.Resolvers.RatingResolver;
using ECommerce.Core.Dtos.AuthDtos;
using ECommerce.Core.Dtos.CartDtos;
using ECommerce.Core.Dtos.OrderDtos;
using ECommerce.Core.Dtos.ProductDtos;
using ECommerce.Core.Dtos.ProfileDtos;
using ECommerce.Core.Dtos.RatingDtos;
using ECommerce.Core.Models.AuthModule;
using ECommerce.Core.Models.BrandModule;
using ECommerce.Core.Models.CartModule;
using ECommerce.Core.Models.CategoryModule;
using ECommerce.Core.Models.OrderModule;
using ECommerce.Core.Models.OrderModule.Owned;
using ECommerce.Core.Models.ProductModule;
using ECommerce.Core.Models.WishlistModule;

namespace ECommerce.Application.Maps;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        #region Product

        CreateMap<Product, ProductResult>()
            .ForMember(dest => dest.Brand, opts => opts.MapFrom<ProductBrandNameTranslationResolver>())
            .ForMember(dest => dest.BrandId, opts => opts.MapFrom(src => src.BrandId))
            .ForMember(dest => dest.Category, opts => opts.MapFrom<ProductCategoryNameTranslationResolver>())
            .ForMember(dest => dest.CategoryId, opts => opts.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.PictureUrl, opts => opts.MapFrom<ProductPictureUrlResolver>())
            .ForMember(dest => dest.Description, opts => opts.MapFrom<ProductDescriptionTranslationResolver>())
            .ForMember(dest => dest.Name, opts => opts.MapFrom<ProductNameTranslationResolver>());

        CreateMap<Brand, BrandResult>()
            .ForMember(dest => dest.PictureUrl, opts => opts.MapFrom<BrandPictureUrlResolver>())
            .ForMember(dest => dest.Name, opts => opts.MapFrom<BrandNameTranslationResolver>());

        CreateMap<Category, CategoryResult>()
            .ForMember(dest => dest.PictureUrl, opts => opts.MapFrom<CategoryPictureUrlResolver>())
            .ForMember(dest => dest.Name, opts => opts.MapFrom<CategoryNameTranslationResolver>());

        CreateMap<ProductGallery, ProductGalleryResult>()
            .ForMember(dest => dest.PictureUrl, opts => opts.MapFrom<GalleryPictureUrlResolver>());

        #endregion


        // Cart
        CreateMap<Cart, CartResult>();
        CreateMap<CartItem, CartItemResult>()
            .ForMember(dest => dest.ProductPictureUrl, opts => opts.MapFrom<CartItemProductPictureUrlResolver>())
            .ForMember(dest => dest.ProductName, opts => opts.MapFrom<CartItemProductNameTranslation>());


        // Wishlist
        CreateMap<ProductResult, WishlistItem>();

        // Order
        CreateMap<Order, OrderResult>()
            .ForMember(dest => dest.OrderId, opts => opts.MapFrom(src => src.Id))
            .ForMember(dest => dest.DeliveryMethodCost, opts => opts.MapFrom(src => src.DeliveryMethod.Cost))
            .ForMember(dest => dest.UserEmail, opts => opts.MapFrom(src => src.User.Email));

        CreateMap<OrderItem, OrderItemResult>()
            .ForMember(dest => dest.ProductId, opts => opts.MapFrom(src => src.Product.Id))
            .ForMember(dest => dest.PictureUrl, opts => opts.MapFrom(src => src.Product.PictureUrl))
            .ForMember(dest => dest.ProductName, opts => opts.MapFrom<OrderItemProductNameResolver>());

        CreateMap<ShippingAddressInput, ShippingAddress>();

        CreateMap<DeliveryMethod, DeliveryMethodResult>()
            .ForMember(dest => dest.ShortName, opts => opts.MapFrom<DeliveryMethodShortNameResolver>())
            .ForMember(dest => dest.Description, opts => opts.MapFrom<DeliveryMethodDescriptionResolver>())
            .ForMember(dest => dest.DeliveryTime, opts => opts.MapFrom<DeliveryMethodDeliveryTimeResolver>());


        // Auth
        CreateMap<AppUser, AccountInfoResult>()
            .ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.Id))
            .ForMember(dest => dest.PictureUrl, opts => opts.MapFrom<UserProfilePictureUrlResolver>());

        CreateMap<Address, AddressResult>();

        // Rating
        CreateMap<Rating, RatingResult>();

        CreateMap<Product, RatedProduct>()
            .ForMember(dest => dest.Name, opts => opts.MapFrom<RatedProductNameTranslationResolver>())
            .ForMember(dest => dest.PictureUrl, opts => opts.MapFrom<RatedProductPictureUrlResolver>());

        CreateMap<AppUser, RatingUser>()
            .ForMember(dest => dest.PictureUrl, opts => opts.MapFrom<RatingUserPictureUrlResolver>());
    }
}