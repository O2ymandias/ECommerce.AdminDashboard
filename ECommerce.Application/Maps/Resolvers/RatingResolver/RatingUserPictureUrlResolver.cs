using AutoMapper;
using ECommerce.Core.Dtos.RatingDtos;
using ECommerce.Core.Models.AuthModule;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Application.Maps.Resolvers.RatingResolver;

internal class RatingUserPictureUrlResolver(IConfiguration config) : IValueResolver<AppUser, RatingUser, string?>
{
    public string? Resolve(AppUser source, RatingUser destination, string? destMember, ResolutionContext context)
    {
        if (source.PictureUrl is null) return null;

        return string.IsNullOrEmpty(source.PictureUrl)
            ? string.Empty
            : $"{config["BaseUrl"]}/{source.PictureUrl}";
    }
}