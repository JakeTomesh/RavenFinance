using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RavenFinance.Migrations
{
    /// <inheritdoc />
    public partial class SavingsViewModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SavingContributions",
                columns: table => new
                {
                    SavingContributionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SavingId = table.Column<int>(type: "int", nullable: false),
                    FinanceRecordId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DateCommitted = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavingContributions", x => x.SavingContributionId);
                    table.ForeignKey(
                        name: "FK_SavingContributions_FinanceRecords_FinanceRecordId",
                        column: x => x.FinanceRecordId,
                        principalTable: "FinanceRecords",
                        principalColumn: "FinanceRecordId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavingContributions_Savings_SavingId",
                        column: x => x.SavingId,
                        principalTable: "Savings",
                        principalColumn: "SavingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavingContributions_FinanceRecordId",
                table: "SavingContributions",
                column: "FinanceRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_SavingContributions_SavingId",
                table: "SavingContributions",
                column: "SavingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavingContributions");
        }
    }
}
