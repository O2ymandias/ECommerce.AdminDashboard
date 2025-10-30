using AutoMapper;
using ECommerce.Core.Dtos.ProductDtos;
using ECommerce.Core.Models.BrandModule;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Application.Maps.Resolvers.BrandResolvers;

public class BrandPictureUrlResolver(IConfiguration config) : IValueResolver<Brand, BrandResult, string>
{
    public string Resolve(Brand source, BrandResult destination, string destMember, ResolutionContext context)
    {
        return string.IsNullOrEmpty(source.PictureUrl)
            ? string.Empty
            : $"{config["BaseUrl"]}/{source.PictureUrl}";
    }
}