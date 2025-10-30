using ECommerce.APIs.ResponseModels.ErrorModels;
using ECommerce.Core.Common.Constants;
using ECommerce.Infrastructure.Database;
using ECommerce.Infrastructure.Database.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace ECommerce.APIs.Extensions;

public static class AppExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider serviceProvider = scope.ServiceProvider;

        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        var appDbContext = serviceProvider.GetRequiredService<AppDbContext>();
        var appDatabaseSeeder = serviceProvider.GetRequiredService<AppDatabaseSeeder>();

        try
        {
            await appDbContext.Database.MigrateAsync();
            logger.LogInformation("Database migrations were applied successfully for AppDbContext.");
            await appDatabaseSeeder.SeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error: {Message}", ex.Message);
            throw;
        }
    }

    public static void UseSwaggerAndSwaggerUi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "FreshCart API v1");
            options.RoutePrefix = "swagger";
        });
    }

    public static void UseLocalization(this WebApplication app)
    {
        var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
        app.UseRequestLocalization(localizationOptions);
    }

    public static void MapFallbackEndPoint(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var localizer = serviceProvider.GetRequiredService<IStringLocalizer<Program>>();

        app.MapFallback(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/json";

            var response = new ApiErrorResponse(
                StatusCodes.Status404NotFound,
                localizer[L.General.EndpointNotFound, context.Request.Path]
            );

            await context.Response.WriteAsJsonAsync(response);
        });
    }
}