using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using RavenFinance.Data;

#nullable disable

namespace RavenFinance.Migrations
{
    /// <summary>Adds the IsDeleted soft-delete flag to FinanceRecords.</summary>
    [DbContext(typeof(RavenDbContext))]
    [Migration("20261101000000_AddFinanceRecordIsDeleted")]
    public partial class AddFinanceRecordIsDeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FinanceRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FinanceRecords");
        }
    }
}
