using AutoMapper;
using ECommerce.Core.Dtos.ProductDtos;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.BrandModule;

namespace ECommerce.Application.Maps.Resolvers.BrandResolvers;

internal class BrandNameTranslationResolver(ICultureService cultureService) : IValueResolver<Brand, BrandResult, string>
{
    public string Resolve(Brand source, BrandResult destination, string destMember, ResolutionContext context)
    {
        if (cultureService.CanTranslate)
        {
            var brandTrans = source.Translations.FirstOrDefault(t => t.LanguageCode == cultureService.LanguageCode);
            return brandTrans is not null ? brandTrans.Name : source.Name;
        }

        return source.Name;
    }
}