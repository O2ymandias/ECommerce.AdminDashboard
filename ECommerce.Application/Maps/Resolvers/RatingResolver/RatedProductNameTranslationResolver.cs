using AutoMapper;
using ECommerce.Core.Dtos.RatingDtos;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.ProductModule;

namespace ECommerce.Application.Maps.Resolvers.RatingResolver;

internal class RatedProductNameTranslationResolver(ICultureService cultureService)
    : IValueResolver<Product, RatedProduct, string>
{
    public string Resolve(Product source, RatedProduct destination, string destMember, ResolutionContext context)
    {
        if (cultureService.CanTranslate)
        {
            var ProductTrans = source.Translations.FirstOrDefault(t => t.LanguageCode == cultureService.LanguageCode);
            return ProductTrans is not null ? ProductTrans.Name : source.Name;
        }

        return source.Name;
    }
}