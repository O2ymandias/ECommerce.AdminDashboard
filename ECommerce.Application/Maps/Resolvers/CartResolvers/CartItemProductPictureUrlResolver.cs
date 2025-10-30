using AutoMapper;
using ECommerce.Core.Dtos.CartDtos;
using ECommerce.Core.Models.CartModule;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Application.Maps.Resolvers.CartResolvers;

internal class CartItemProductPictureUrlResolver(IConfiguration config)
    : IValueResolver<CartItem, CartItemResult, string>
{
    public string Resolve(CartItem source, CartItemResult destination, string destMember, ResolutionContext context)
    {
        return string.IsNullOrEmpty(source.ProductPictureUrl)
            ? string.Empty
            : $"{config["BaseUrl"]}/{source.ProductPictureUrl}";
    }
}