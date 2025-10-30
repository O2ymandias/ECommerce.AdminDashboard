using AutoMapper;
using ECommerce.Core.Dtos.OrderDtos;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.OrderModule;

namespace ECommerce.Application.Maps.Resolvers.DeliveryMethodResolvers;

internal class DeliveryMethodShortNameResolver(ICultureService cultureService)
    : IValueResolver<DeliveryMethod, DeliveryMethodResult, string>
{
    public string Resolve(DeliveryMethod source, DeliveryMethodResult destination, string destMember,
        ResolutionContext context)
    {
        if (cultureService.CanTranslate)
        {
            var deliveryMethodTrans =
                source.Translations.FirstOrDefault(t => t.LanguageCode == cultureService.LanguageCode);
            return deliveryMethodTrans is not null ? deliveryMethodTrans.ShortName : source.ShortName;
        }

        return source.ShortName;
    }
}