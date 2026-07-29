using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RavenFinance.Migrations
{
    /// <inheritdoc />
    public partial class ViewModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StatementItem_PaymentSources_PaymentSourceId",
                table: "StatementItem");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentSourceId",
                table: "StatementItem",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StatementItem_PaymentSources_PaymentSourceId",
                table: "StatementItem",
                column: "PaymentSourceId",
                principalTable: "PaymentSources",
                principalColumn: "PaymentSourceId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StatementItem_PaymentSources_PaymentSourceId",
                table: "StatementItem");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentSourceId",
                table: "StatementItem",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_StatementItem_PaymentSources_PaymentSourceId",
                table: "StatementItem",
                column: "PaymentSourceId",
                principalTable: "PaymentSources",
                principalColumn: "PaymentSourceId");
        }
    }
}
