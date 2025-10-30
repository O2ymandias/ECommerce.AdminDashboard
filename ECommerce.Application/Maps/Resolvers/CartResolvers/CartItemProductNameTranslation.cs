using AutoMapper;
using ECommerce.Core.Dtos.CartDtos;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.CartModule;

namespace ECommerce.Application.Maps.Resolvers.CartResolvers;

internal class CartItemProductNameTranslation(ICultureService cultureService)
    : IValueResolver<CartItem, CartItemResult, string>
{
    public string Resolve(CartItem source, CartItemResult destination, string destMember, ResolutionContext context)
    {
        if (
            cultureService.CanTranslate &&
            source.NameTranslations.TryGetValue(cultureService.LanguageCode.ToString().ToLower(), out string? value)
        )
        {
            return value ?? source.ProductName;
        }

        return source.ProductName;
    }
}