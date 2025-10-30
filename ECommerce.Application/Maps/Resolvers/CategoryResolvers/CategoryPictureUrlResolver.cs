using AutoMapper;
using ECommerce.Core.Dtos.ProductDtos;
using ECommerce.Core.Models.CategoryModule;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Application.Maps.Resolvers.CategoryResolvers;

internal class CategoryPictureUrlResolver(IConfiguration config) : IValueResolver<Category, CategoryResult, string>
{
    public string Resolve(Category source, CategoryResult destination, string destMember, ResolutionContext context)
    {
        return string.IsNullOrEmpty(source.PictureUrl)
            ? string.Empty
            : $"{config["BaseUrl"]}/{source.PictureUrl}";
    }
}