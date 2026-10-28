using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using RavenFinance.Data;

#nullable disable

namespace RavenFinance.Migrations
{
    /// <summary>
    /// Adds Savings.Amount, the initial amount a savings account starts with. Existing savings start at 0.
    /// </summary>
    [DbContext(typeof(RavenDbContext))]
    [Migration("20261104000000_SavingInitialAmount")]
    public partial class SavingInitialAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Savings",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Savings");
        }
    }
}
