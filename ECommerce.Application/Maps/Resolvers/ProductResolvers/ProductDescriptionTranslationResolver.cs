using AutoMapper;
using ECommerce.Core.Dtos.ProductDtos;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.ProductModule;

namespace ECommerce.Application.Maps.Resolvers.ProductResolvers;

internal class ProductDescriptionTranslationResolver(ICultureService cultureService)
    : IValueResolver<Product, ProductResult, string>
{
    public string Resolve(Product source, ProductResult destination, string destMember, ResolutionContext context)
    {
        if (cultureService.CanTranslate)
        {
            var productTrans = source.Translations.FirstOrDefault(t => t.LanguageCode == cultureService.LanguageCode);
            return productTrans is not null ? productTrans.Description : source.Description;
        }

        return source.Description;
    }
}