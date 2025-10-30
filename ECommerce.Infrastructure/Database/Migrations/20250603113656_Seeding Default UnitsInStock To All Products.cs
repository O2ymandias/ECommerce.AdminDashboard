using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Database.Migrations
{
	/// <inheritdoc />
	public partial class SeedingDefaultUnitsInStockToAllProducts : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			//migrationBuilder.Sql(
			//	@"
			//             UPDATE Products
			//             SET UnitsInStock = 100;
			//             "
			//);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			//migrationBuilder.Sql(
			//	@"
			//             UPDATE Products
			//             SET UnitsInStock = 0;
			//             "
			//);
		}
	}
}
