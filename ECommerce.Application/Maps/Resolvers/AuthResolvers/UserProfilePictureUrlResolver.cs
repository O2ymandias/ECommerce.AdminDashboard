using AutoMapper;
using ECommerce.Core.Dtos.ProfileDtos;
using ECommerce.Core.Models.AuthModule;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Application.Maps.Resolvers.AuthResolvers;

internal class UserProfilePictureUrlResolver(IConfiguration config)
    : IValueResolver<AppUser, AccountInfoResult, string?>
{
    public string? Resolve(AppUser source, AccountInfoResult destination, string? destMember, ResolutionContext context)
    {
        if (source.PictureUrl is null) return null;

        return string.IsNullOrEmpty(source.PictureUrl)
            ? string.Empty
            : $"{config["BaseUrl"]}/{source.PictureUrl}";
    }
}