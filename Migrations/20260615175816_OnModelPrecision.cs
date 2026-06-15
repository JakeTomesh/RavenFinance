using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RavenFinance.Migrations
{
    /// <inheritdoc />
    public partial class OnModelPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Categories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Categories",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
