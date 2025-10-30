using ECommerce.Dashboard.Extensions;
using Microsoft.Extensions.Options;

namespace ECommerce.Dashboard;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		#region Add services to the container.

		builder.Services.AddControllersWithViews();
		builder.Services.RegisterAppServices(builder.Configuration);

		#endregion

		var app = builder.Build();

		#region Configure the HTTP request pipeline.

		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Home/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseRouting();

		app.UseAuthorization();

		app.MapStaticAssets();

		var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
		app.UseRequestLocalization(localizationOptions);

		app.MapControllerRoute(
			name: "default",
			pattern: "{controller=Products}/{action=Index}/{id?}")
			.WithStaticAssets();

		#endregion

		app.Run();
	}
}
