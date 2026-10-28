using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using RavenFinance.Data;

#nullable disable

namespace RavenFinance.Migrations
{
    /// <summary>
    /// Adds RecurringBills.ItemType (0 = Deposit, 1 = Expense). Existing bills were all expenses, so default to 1.
    /// </summary>
    [DbContext(typeof(RavenDbContext))]
    [Migration("20261103000000_RecurringBillItemType")]
    public partial class RecurringBillItemType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemType",
                table: "RecurringBills",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "RecurringBills");
        }
    }
}
