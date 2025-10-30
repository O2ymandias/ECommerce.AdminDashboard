using AutoMapper;
using ECommerce.Core.Dtos.OrderDtos;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.OrderModule;

namespace ECommerce.Application.Maps.Resolvers.OrderResolvers;

internal class OrderItemProductNameResolver(ICultureService cultureService)
    : IValueResolver<OrderItem, OrderItemResult, string>
{
    public string Resolve(OrderItem source, OrderItemResult destination, string destMember, ResolutionContext context)
    {
        if (
            cultureService.CanTranslate &&
            source.Product.NameTranslations.TryGetValue(cultureService.LanguageCode.ToString(), out string? val)
        )
        {
            return val ?? source.Product.Name;
        }

        return source.Product.Name;
    }
}