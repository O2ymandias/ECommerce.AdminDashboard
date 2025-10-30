using AutoMapper;
using ECommerce.Core.Dtos.ProductDtos;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.CategoryModule;

namespace ECommerce.Application.Maps.Resolvers.CategoryResolvers;

internal class CategoryNameTranslationResolver(ICultureService cultureService)
    : IValueResolver<Category, CategoryResult, string>
{
    public string Resolve(Category source, CategoryResult destination, string destMember, ResolutionContext context)
    {
        if (cultureService.CanTranslate)
        {
            var catTrans = source.Translations.FirstOrDefault(t => t.LanguageCode == cultureService.LanguageCode);
            return catTrans is not null ? catTrans.Name : source.Name;
        }

        return source.Name;
    }
}