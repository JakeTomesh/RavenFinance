using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RavenFinance.Migrations
{
    /// <inheritdoc />
    public partial class RecurringBillsModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "StatementItem",
                columns: table => new
                {
                    StatementItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Merchant = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMonthlyRecurringBill = table.Column<bool>(type: "bit", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatementItem", x => x.StatementItemId);
                });

            migrationBuilder.CreateTable(
                name: "PaymentSource",
                columns: table => new
                {
                    PaymentSourceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatementItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentSource", x => x.PaymentSourceId);
                    table.ForeignKey(
                        name: "FK_PaymentSource_StatementItem_StatementItemId",
                        column: x => x.StatementItemId,
                        principalTable: "StatementItem",
                        principalColumn: "StatementItemId");
                });

            migrationBuilder.CreateTable(
                name: "RecurringBills",
                columns: table => new
                {
                    RecurringBillId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSplit = table.Column<bool>(type: "bit", nullable: false),
                    PaymentSourceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringBills", x => x.RecurringBillId);
                    table.ForeignKey(
                        name: "FK_RecurringBills_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecurringBills_PaymentSource_PaymentSourceId",
                        column: x => x.PaymentSourceId,
                        principalTable: "PaymentSource",
                        principalColumn: "PaymentSourceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSource_StatementItemId",
                table: "PaymentSource",
                column: "StatementItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBills_CategoryId",
                table: "RecurringBills",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBills_PaymentSourceId",
                table: "RecurringBills",
                column: "PaymentSourceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecurringBills");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "PaymentSource");

            migrationBuilder.DropTable(
                name: "StatementItem");
        }
    }
}
