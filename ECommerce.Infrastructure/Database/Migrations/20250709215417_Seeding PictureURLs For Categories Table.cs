using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Database.Migrations
{
	/// <inheritdoc />
	public partial class SeedingPictureURLsForCategoriesTable : Migration
	{

		//private static readonly List<string> pictureURLs =
		//[
		//	"images/categories/Frappuccino.jpg" ,
		//	"images/categories/Latte.jpg" ,
		//	"images/categories/Mocha.jpg" ,
		//	"images/categories/Macchiato.jpg" ,
		//	"images/categories/Matcha.jpg" ,
		//	"images/categories/Cake.jpg",
		//	"images/categories/Donuts.jpg" ,
		//	"images/categories/Salad.jpg" ,
		//];

		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			//for (int i = 0; i < pictureURLs.Count; i++)
			//{
			//	migrationBuilder.UpdateData(
			//	   table: "Categories",
			//	   keyColumn: "Id",
			//	   keyValue: i + 1,
			//	   column: "PictureUrl",
			//	   value: pictureURLs[i]
			//	);
			//}
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			//for (int id = 1; id <= 7; id++)
			//{
			//	migrationBuilder.UpdateData(
			//		table: "Categories",
			//		keyColumn: "Id",
			//		keyValue: id,
			//		column: "PictureUrl",
			//		value: null);
			//}
		}
	}
}
