using AutoMapper;
using ECommerce.Core.Dtos.ProductDtos;
using ECommerce.Core.Models.ProductModule;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Application.Maps.Resolvers.GalleryResolvers;

internal class GalleryPictureUrlResolver(IConfiguration config)
    : IValueResolver<ProductGallery, ProductGalleryResult, string>
{
    public string Resolve(ProductGallery source, ProductGalleryResult destination, string destMember,
        ResolutionContext context)
    {
        return string.IsNullOrEmpty(source.PictureUrl)
            ? string.Empty
            : $"{config["BaseUrl"]}/{source.PictureUrl}";
    }
}