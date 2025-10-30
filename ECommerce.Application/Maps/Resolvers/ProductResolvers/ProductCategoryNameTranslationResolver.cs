using AutoMapper;
using ECommerce.Core.Dtos.ProductDtos;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.ProductModule;

namespace ECommerce.Application.Maps.Resolvers.ProductResolvers;

internal class ProductCategoryNameTranslationResolver(ICultureService cultureService)
    : IValueResolver<Product, ProductResult, string>
{
    public string Resolve(Product source, ProductResult destination, string destMember, ResolutionContext context)
    {
        if (cultureService.CanTranslate)
        {
            var catTrans =
                source.Category.Translations.FirstOrDefault(t => t.LanguageCode == cultureService.LanguageCode);
            return catTrans is not null ? catTrans.Name : source.Category.Name;
        }

        return source.Category.Name;
    }
}