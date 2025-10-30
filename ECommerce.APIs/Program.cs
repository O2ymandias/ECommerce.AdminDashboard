using ECommerce.APIs.Extensions;
using ECommerce.APIs.Middlewares;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ECommerce.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add services to the container.

            builder.Services
                .AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
                    options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    options.SerializerSettings.DateFormatString = "o";
                });

            builder.Services.AddOpenApi();

            builder.Services.RegisterAppServices(builder.Configuration);

            #endregion

            var app = builder.Build();

            await app.InitializeDatabaseAsync();

            #region Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerAndSwaggerUi();
            }

            app.UseMiddleware<ExceptionMiddleware>();

            //app.UseHttpsRedirection();

            app.MapStaticAssets();

            app.UseCors("ECommerce");

            app.UseAuthorization();

            app.UseLocalization();

            app.MapControllers().WithStaticAssets();

            app.MapFallbackEndPoint();

            #endregion

            await app.RunAsync();
        }
    }
}