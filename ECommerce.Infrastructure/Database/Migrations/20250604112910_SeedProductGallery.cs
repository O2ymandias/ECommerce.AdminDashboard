using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Database.Migrations
{
	/// <inheritdoc />
	public partial class SeedProductGallery : Migration
	{
		//private static readonly object[,] galleryData = new object[,]
		//{
		//	{ "images/products/gallery/Double-Caramel-Frappuccino-1.jpg", "Double Caramel Frappuccino", 1 },
		//	{ "images/products/gallery/Double-Caramel-Frappuccino-2.jpg", "Double Caramel Frappuccino", 1 },
		//	{ "images/products/gallery/Double-Caramel-Frappuccino-3.jpg", "Double Caramel Frappuccino", 1 },
		//	{ "images/products/gallery/Double-Caramel-Frappuccino-4.jpg", "Double Caramel Frappuccino", 1 }
		//};

		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			//migrationBuilder.InsertData(
			//	table: "ProductGalleries",
			//	columns: ["PictureUrl", "AltText", "ProductId"],
			//	values: galleryData);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			//migrationBuilder.Sql(@"
			//	DELETE FROM ProductGalleries
			//	WHERE ProductId = 1
			//	AND PictureUrl IN (
			//		'images/products/gallery/Double-Caramel-Frappuccino-1.jpg',
			//		'images/products/gallery/Double-Caramel-Frappuccino-2.jpg',
			//		'images/products/gallery/Double-Caramel-Frappuccino-3.jpg',
			//		'images/products/gallery/Double-Caramel-Frappuccino-4.jpg'
			//	);
			//");
		}
	}
}
