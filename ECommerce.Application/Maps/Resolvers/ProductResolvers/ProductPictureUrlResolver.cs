using AutoMapper;
using ECommerce.Core.Dtos.ProductDtos;
using ECommerce.Core.Models.ProductModule;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Application.Maps.Resolvers.ProductResolvers;

public class ProductPictureUrlResolver(IConfiguration config) : IValueResolver<Product, ProductResult, string>
{
    public string Resolve(Product source, ProductResult destination, string destMember, ResolutionContext context)
    {
        return string.IsNullOrEmpty(source.PictureUrl)
            ? string.Empty
            : $"{config["BaseUrl"]}/{source.PictureUrl}";
    }
}