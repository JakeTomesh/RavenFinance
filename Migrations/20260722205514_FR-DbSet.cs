using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RavenFinance.Migrations
{
    /// <inheritdoc />
    public partial class FRDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FinanceRecordId",
                table: "Savings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FinanceRecords",
                columns: table => new
                {
                    FinanceRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DateStart = table.Column<DateOnly>(type: "date", nullable: false),
                    DateEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    isComplete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceRecords", x => x.FinanceRecordId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Savings_FinanceRecordId",
                table: "Savings",
                column: "FinanceRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_Savings_FinanceRecords_FinanceRecordId",
                table: "Savings",
                column: "FinanceRecordId",
                principalTable: "FinanceRecords",
                principalColumn: "FinanceRecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Savings_FinanceRecords_FinanceRecordId",
                table: "Savings");

            migrationBuilder.DropTable(
                name: "FinanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_Savings_FinanceRecordId",
                table: "Savings");

            migrationBuilder.DropColumn(
                name: "FinanceRecordId",
                table: "Savings");
        }
    }
}
