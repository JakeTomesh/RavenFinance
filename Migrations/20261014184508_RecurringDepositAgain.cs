using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RavenFinance.Migrations
{
    /// <inheritdoc />
    public partial class RecurringDepositAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecurringBills_PaymentSources_PaymentSourceId1",
                table: "RecurringBills");

            migrationBuilder.DropIndex(
                name: "IX_RecurringBills_PaymentSourceId1",
                table: "RecurringBills");

            migrationBuilder.DropColumn(
                name: "PaymentSourceId1",
                table: "RecurringBills");

            migrationBuilder.AddColumn<int>(
                name: "PaymentSourceId",
                table: "RecurringBills",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBills_PaymentSourceId",
                table: "RecurringBills",
                column: "PaymentSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecurringBills_PaymentSources_PaymentSourceId",
                table: "RecurringBills",
                column: "PaymentSourceId",
                principalTable: "PaymentSources",
                principalColumn: "PaymentSourceId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecurringBills_PaymentSources_PaymentSourceId",
                table: "RecurringBills");

            migrationBuilder.DropIndex(
                name: "IX_RecurringBills_PaymentSourceId",
                table: "RecurringBills");

            migrationBuilder.DropColumn(
                name: "PaymentSourceId",
                table: "RecurringBills");

            migrationBuilder.AddColumn<int>(
                name: "PaymentSourceId1",
                table: "RecurringBills",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBills_PaymentSourceId1",
                table: "RecurringBills",
                column: "PaymentSourceId1");

            migrationBuilder.AddForeignKey(
                name: "FK_RecurringBills_PaymentSources_PaymentSourceId1",
                table: "RecurringBills",
                column: "PaymentSourceId1",
                principalTable: "PaymentSources",
                principalColumn: "PaymentSourceId");
        }
    }
}
