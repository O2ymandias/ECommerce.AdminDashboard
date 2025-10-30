using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Database.Migrations
{
	/// <inheritdoc />
	public partial class SeedingPictureURLsForBrandsTable : Migration
	{
		//private static readonly List<string> pictureURLs =
		//[
		//	"images/brands/Starbucks.png" ,
		//	"images/brands/Costa.png" ,
		//	"images/brands/Cilantro.png" ,
		//	"images/brands/TBS.png" ,
		//	"images/brands/OnTheRun.png" ,
		//	"images/brands/Caribou.png",
		//	"images/brands/KrispyKreme.png" ,
		//];

		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			//for (int i = 0; i < pictureURLs.Count; i++)
			//{
			//	migrationBuilder.UpdateData(
			//	   table: "Brands",
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
			//		table: "Brands",
			//		keyColumn: "Id",
			//		keyValue: id,
			//		column: "PictureUrl",
			//		value: null);
			//}
		}
	}
}
