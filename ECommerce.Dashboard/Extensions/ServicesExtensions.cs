using ECommerce.APIs.Utilities.Localization;
using ECommerce.Application.Maps;
using ECommerce.Application.Services;
using ECommerce.Core.Common.Constants;
using ECommerce.Core.Common.Options;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Repositories;
using ECommerce.Core.Interfaces.Services;
using ECommerce.Core.Models.AuthModule;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Database;
using ECommerce.Infrastructure.Repositories.RedisRepo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

namespace ECommerce.Dashboard.Extensions;
public static class ServicesExtensions
{
	public static IServiceCollection RegisterAppServices(this IServiceCollection services, IConfiguration config)
	{

		services.AddDbContext<AppDbContext>(opts =>
		{
			opts.UseSqlServer(config.GetConnectionString("Default"));
		});

		services.AddHttpClient("MyApi", client =>
		{
			client.BaseAddress = new Uri($"{config["BaseUrl"]}/api/");
			client.DefaultRequestHeaders.Add("Accept", "application/json");
		});

		services.AddLocalization();

		services.RegisterScopedServices();
		services.RegisterSingletonServices(config);
		services.ConfigureOptions(config);
		services.RegisterIdentityServices();
		services.ConfigureJWTAuthentication(config);
		services.ConfigureLocalization();

		return services;
	}


	private static IServiceCollection RegisterScopedServices(this IServiceCollection services)
	{
		services.AddScoped<IUnitOfWork, UnitOfWork>();
		services.AddScoped<IProductService, ProductService>();
		services.AddScoped<ICultureService, CultureService>();
		services.AddScoped<ITokenService, TokenService>();
		services.AddScoped<IEmailSender, EmailSender>();
		services.AddScoped<IImageUploader, ImageUploader>();
		services.AddScoped<IAuthService, AuthService>();
		return services;
	}

	private static IServiceCollection RegisterSingletonServices(this IServiceCollection services, IConfiguration config)
	{
		var redisConnection = config.GetConnectionString("Redis") ?? "localhost";
		services.AddSingleton<IConnectionMultiplexer>(sp =>
		{
			var config = ConfigurationOptions.Parse(redisConnection, true);
			config.AbortOnConnectFail = false;
			return ConnectionMultiplexer.Connect(config);
		});
		services.AddAutoMapper(typeof(MappingProfiles));
		services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
		services.AddSingleton<IRedisRepository, RedisRepository>();
		services.AddSingleton<ICacheService, CacheService>();
		return services;
	}

	private static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration config)
	{
		services.Configure<ImageUploaderOptions>(config.GetSection("ImageUploaderOptions"));
		services.Configure<AdminOptions>(config.GetSection("AdminOptions"));
		services.Configure<JwtOptions>(config.GetSection("JwtOptions"));
		services.Configure<RefreshTokenOptions>(config.GetSection("RefreshTokenOptions"));

		return services;
	}

	private static IServiceCollection RegisterIdentityServices(this IServiceCollection services)
	{
		services
			.AddIdentity<AppUser, IdentityRole>(config =>
			{
				config.User.RequireUniqueEmail = true;
				config.Password.RequiredLength = 6;
				config.Password.RequireNonAlphanumeric = true;
				config.Password.RequiredUniqueChars = 1;
				config.Password.RequireUppercase = true;
				config.Password.RequireDigit = true;
			})
			.AddEntityFrameworkStores<AppDbContext>()
			.AddDefaultTokenProviders();

		return services;
	}

	private static IServiceCollection ConfigureJWTAuthentication(this IServiceCollection services, IConfiguration config)
	{
		services
		.AddAuthentication(config =>
		{
			config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{
			var jwtOptions = config.GetSection("JwtOptions").Get<JwtOptions>()
				?? throw new Exception("JwtOptions section is missing.");

			options.TokenValidationParameters = new TokenValidationParameters()
			{
				ValidateIssuer = true,
				ValidIssuer = jwtOptions.Issuer,

				ValidateAudience = true,
				ValidAudience = jwtOptions.Audience,

				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey)),

				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero
			};
		});

		return services;
	}

	private static IServiceCollection ConfigureLocalization(this IServiceCollection services)
	{
		services.AddLocalization(opts => opts.ResourcesPath = "Resources");

		services.Configure<RequestLocalizationOptions>(opts =>
		{
			opts.SupportedCultures = L.SupportedCultures;
			opts.SupportedUICultures = L.SupportedCultures;
			opts.DefaultRequestCulture = new RequestCulture(
				culture: L.DefaultCulture,
				uiCulture: L.DefaultCulture
				);
		});
		return services;
	}
}