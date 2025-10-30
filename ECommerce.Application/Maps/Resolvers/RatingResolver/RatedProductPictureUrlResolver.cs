using AutoMapper;
using ECommerce.Core.Dtos.RatingDtos;
using ECommerce.Core.Models.ProductModule;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Application.Maps.Resolvers.RatingResolver;

internal class RatedProductPictureUrlResolver(IConfiguration config) : IValueResolver<Product, RatedProduct, string>
{
    public string Resolve(Product source, RatedProduct destination, string destMember, ResolutionContext context)
    {
        return string.IsNullOrEmpty(source.PictureUrl)
            ? string.Empty
            : $"{config["BaseUrl"]}/{source.PictureUrl}";
    }
}