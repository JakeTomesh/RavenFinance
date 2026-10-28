using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using RavenFinance.Data;

#nullable disable

namespace RavenFinance.Migrations
{
    /// <summary>
    /// Makes StatementItems.PaymentSourceId nullable so an item can be paid from a savings account
    /// (StatementItems.SavingId) instead of a payment source.
    /// </summary>
    [DbContext(typeof(RavenDbContext))]
    [Migration("20261102000000_StatementItemPaymentSourceNullable")]
    public partial class StatementItemPaymentSourceNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StatementItems_PaymentSources_PaymentSourceId",
                table: "StatementItems");

            migrationBuilder.DropIndex(
                name: "IX_StatementItems_PaymentSourceId",
                table: "StatementItems");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentSourceId",
                table: "StatementItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_StatementItems_PaymentSourceId",
                table: "StatementItems",
                column: "PaymentSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_StatementItems_PaymentSources_PaymentSourceId",
                table: "StatementItems",
                column: "PaymentSourceId",
                principalTable: "PaymentSources",
                principalColumn: "PaymentSourceId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StatementItems_PaymentSources_PaymentSourceId",
                table: "StatementItems");

            migrationBuilder.DropIndex(
                name: "IX_StatementItems_PaymentSourceId",
                table: "StatementItems");

            // Savings-paid rows have no payment source and cannot exist in the old schema
            migrationBuilder.Sql("DELETE FROM StatementItems WHERE PaymentSourceId IS NULL");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentSourceId",
                table: "StatementItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatementItems_PaymentSourceId",
                table: "StatementItems",
                column: "PaymentSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_StatementItems_PaymentSources_PaymentSourceId",
                table: "StatementItems",
                column: "PaymentSourceId",
                principalTable: "PaymentSources",
                principalColumn: "PaymentSourceId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
