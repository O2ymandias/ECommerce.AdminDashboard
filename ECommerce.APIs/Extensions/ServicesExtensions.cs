using ECommerce.APIs.Middlewares;
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
using ECommerce.Infrastructure.Database.SeedData;
using ECommerce.Infrastructure.Repositories.RedisRepo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;
using ECommerce.APIs.ResponseModels.ErrorModels;

namespace ECommerce.APIs.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection RegisterAppServices(this IServiceCollection services, IConfiguration config)
    {
        var defaultConnection = config.GetConnectionString("Default");
        var redisConnection = config.GetConnectionString("Redis") ?? "localhost";

        services.AddDbContext<AppDbContext>(options => { options.UseSqlServer(defaultConnection); });
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ExceptionMiddleware>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IWishlistService, WishlistService>();
        services.AddScoped<IRatingService, RatingService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICheckoutService, CheckoutService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IApiErrorResponseFactory, ApiErrorResponseFactory>();
        services.AddScoped<ICultureService, CultureService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ISalesService, SalesService>();

        services.AddTransient<AppDatabaseSeeder>();
        services.AddTransient<IEmailSender, EmailSender>();
        services.AddTransient<IImageUploader, ImageUploader>();

        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var options = ConfigurationOptions.Parse(redisConnection, true);
            options.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(options);
        });
        services.AddSingleton<IRedisRepository, RedisRepository>();
        services.AddSingleton<ICacheService, CacheService>();
        services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

        services.AddAutoMapper(typeof(MappingProfiles));
        services.ConfigureModelStateValidationError();
        services.ConfigureLocalization();
        services.RegisterCors(config);

        services.Configure<CartOptions>(config.GetSection("CartOptions"));
        services.Configure<EmailOptions>(config.GetSection("EmailOptions"));
        services.Configure<JwtOptions>(config.GetSection("JwtOptions"));
        services.Configure<RefreshTokenOptions>(config.GetSection("RefreshTokenOptions"));
        services.Configure<StripeOptions>(config.GetSection("StripeOptions"));
        services.Configure<ImageUploaderOptions>(config.GetSection("ImageUploaderOptions"));
        services.Configure<AdminOptions>(config.GetSection("AdminOptions"));

        services
            .AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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

        services.ConfigureSwagger();

        return services;
    }

    private static IServiceCollection RegisterCors(this IServiceCollection services, IConfiguration config)
    {
        services.AddCors(setup =>
        {
            setup.AddPolicy("ECommerce", policyBuilder =>
            {
                policyBuilder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins(
                        config["ClientUrl"] ?? throw new Exception("Failed to parse the client url."),
                        config["DashboardUrl"] ?? throw new Exception("Failed to parse the client url.")
                    )
                    .AllowCredentials();
            });
        });
        return services;
    }

    private static IServiceCollection ConfigureModelStateValidationError(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(config =>
        {
            config.InvalidModelStateResponseFactory = (action) =>
            {
                List<string> errors = [];
                foreach (var value in action.ModelState.Values)
                foreach (var modelError in value.Errors)
                    errors.Add(modelError.ErrorMessage);

                var errorFactory = action.HttpContext.RequestServices.GetRequiredService<IApiErrorResponseFactory>();
                var error = errorFactory.CreateValidationErrorResponse(errors);

                return new BadRequestObjectResult(error);
            };
        });
        return services;
    }

    private static IServiceCollection ConfigureLocalization(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddDataAnnotationsLocalization(opts =>
            {
                opts.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(type);
            });

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

    private static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "FreshCart API",
                Version = "v1",
                Description = "A modern e-commerce backend API built with ASP.NET Core and Clean Architecture",
                Contact = new OpenApiContact
                {
                    Name = "FreshCart Team",
                    Email = "support@freshcart.com"
                }
            });

            // Add JWT Authentication to Swagger
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });

        return services;
    }
}