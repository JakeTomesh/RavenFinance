using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RavenFinance.Migrations
{
    /// <inheritdoc />
    public partial class PaymentSourceDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentSource_StatementItem_StatementItemId",
                table: "PaymentSource");

            migrationBuilder.DropForeignKey(
                name: "FK_RecurringBills_PaymentSource_PaymentSourceId",
                table: "RecurringBills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentSource",
                table: "PaymentSource");

            migrationBuilder.RenameTable(
                name: "PaymentSource",
                newName: "PaymentSources");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentSource_StatementItemId",
                table: "PaymentSources",
                newName: "IX_PaymentSources_StatementItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentSources",
                table: "PaymentSources",
                column: "PaymentSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentSources_StatementItem_StatementItemId",
                table: "PaymentSources",
                column: "StatementItemId",
                principalTable: "StatementItem",
                principalColumn: "StatementItemId");

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
                name: "FK_PaymentSources_StatementItem_StatementItemId",
                table: "PaymentSources");

            migrationBuilder.DropForeignKey(
                name: "FK_RecurringBills_PaymentSources_PaymentSourceId",
                table: "RecurringBills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentSources",
                table: "PaymentSources");

            migrationBuilder.RenameTable(
                name: "PaymentSources",
                newName: "PaymentSource");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentSources_StatementItemId",
                table: "PaymentSource",
                newName: "IX_PaymentSource_StatementItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentSource",
                table: "PaymentSource",
                column: "PaymentSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentSource_StatementItem_StatementItemId",
                table: "PaymentSource",
                column: "StatementItemId",
                principalTable: "StatementItem",
                principalColumn: "StatementItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecurringBills_PaymentSource_PaymentSourceId",
                table: "RecurringBills",
                column: "PaymentSourceId",
                principalTable: "PaymentSource",
                principalColumn: "PaymentSourceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
