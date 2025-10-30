using AutoMapper;
using ECommerce.Core.Dtos.ProductDtos;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.ProductModule;

namespace ECommerce.Application.Maps.Resolvers.ProductResolvers;

internal class ProductBrandNameTranslationResolver(ICultureService cultureService)
    : IValueResolver<Product, ProductResult, string>
{
    public string Resolve(Product source, ProductResult destination, string destMember, ResolutionContext context)
    {
        if (cultureService.CanTranslate)
        {
            var brandTrans =
                source.Brand.Translations.FirstOrDefault(t => t.LanguageCode == cultureService.LanguageCode);
            return brandTrans is not null ? brandTrans.Name : source.Brand.Name;
        }

        return source.Brand.Name;
    }
}