using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RavenFinance.Migrations
{
    /// <inheritdoc />
    public partial class Savings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentSources_StatementItem_StatementItemId",
                table: "PaymentSources");

            migrationBuilder.DropIndex(
                name: "IX_PaymentSources_StatementItemId",
                table: "PaymentSources");

            migrationBuilder.DropColumn(
                name: "StatementItemId",
                table: "PaymentSources");

            migrationBuilder.AlterColumn<string>(
                name: "Merchant",
                table: "StatementItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfTransaction",
                table: "StatementItem",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<int>(
                name: "PaymentSourceId",
                table: "StatementItem",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SavingId",
                table: "StatementItem",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Savings",
                columns: table => new
                {
                    SavingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Savings", x => x.SavingId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StatementItem_CategoryId",
                table: "StatementItem",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StatementItem_PaymentSourceId",
                table: "StatementItem",
                column: "PaymentSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_StatementItem_SavingId",
                table: "StatementItem",
                column: "SavingId");

            migrationBuilder.AddForeignKey(
                name: "FK_StatementItem_Categories_CategoryId",
                table: "StatementItem",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatementItem_PaymentSources_PaymentSourceId",
                table: "StatementItem",
                column: "PaymentSourceId",
                principalTable: "PaymentSources",
                principalColumn: "PaymentSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_StatementItem_Savings_SavingId",
                table: "StatementItem",
                column: "SavingId",
                principalTable: "Savings",
                principalColumn: "SavingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StatementItem_Categories_CategoryId",
                table: "StatementItem");

            migrationBuilder.DropForeignKey(
                name: "FK_StatementItem_PaymentSources_PaymentSourceId",
                table: "StatementItem");

            migrationBuilder.DropForeignKey(
                name: "FK_StatementItem_Savings_SavingId",
                table: "StatementItem");

            migrationBuilder.DropTable(
                name: "Savings");

            migrationBuilder.DropIndex(
                name: "IX_StatementItem_CategoryId",
                table: "StatementItem");

            migrationBuilder.DropIndex(
                name: "IX_StatementItem_PaymentSourceId",
                table: "StatementItem");

            migrationBuilder.DropIndex(
                name: "IX_StatementItem_SavingId",
                table: "StatementItem");

            migrationBuilder.DropColumn(
                name: "DateOfTransaction",
                table: "StatementItem");

            migrationBuilder.DropColumn(
                name: "PaymentSourceId",
                table: "StatementItem");

            migrationBuilder.DropColumn(
                name: "SavingId",
                table: "StatementItem");

            migrationBuilder.AlterColumn<string>(
                name: "Merchant",
                table: "StatementItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatementItemId",
                table: "PaymentSources",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSources_StatementItemId",
                table: "PaymentSources",
                column: "StatementItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentSources_StatementItem_StatementItemId",
                table: "PaymentSources",
                column: "StatementItemId",
                principalTable: "StatementItem",
                principalColumn: "StatementItemId");
        }
    }
}
