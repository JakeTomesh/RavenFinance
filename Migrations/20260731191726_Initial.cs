using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RavenFinance.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

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

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "FinanceRecords",
                columns: table => new
                {
                    FinanceRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DateStart = table.Column<DateOnly>(type: "date", nullable: false),
                    DateEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    IsComplete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceRecords", x => x.FinanceRecordId);
                    table.ForeignKey(
                        name: "FK_FinanceRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentSources",
                columns: table => new
                {
                    PaymentSourceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentSources", x => x.PaymentSourceId);
                    table.ForeignKey(
                        name: "FK_PaymentSources_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
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
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DueDate = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSplit = table.Column<bool>(type: "bit", nullable: false),
                    PaymentSourceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringBills", x => x.RecurringBillId);
                    table.ForeignKey(
                        name: "FK_RecurringBills_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecurringBills_PaymentSources_PaymentSourceId",
                        column: x => x.PaymentSourceId,
                        principalTable: "PaymentSources",
                        principalColumn: "PaymentSourceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatementItems",
                columns: table => new
                {
                    StatementItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Merchant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfTransaction = table.Column<DateOnly>(type: "date", nullable: false),
                    ItemType = table.Column<int>(type: "int", nullable: false),
                    IsMonthlyRecurringBill = table.Column<bool>(type: "bit", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PaymentSourceId = table.Column<int>(type: "int", nullable: false),
                    FinanceRecordId = table.Column<int>(type: "int", nullable: false),
                    SavingId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatementItems", x => x.StatementItemId);
                    table.ForeignKey(
                        name: "FK_StatementItems_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StatementItems_FinanceRecords_FinanceRecordId",
                        column: x => x.FinanceRecordId,
                        principalTable: "FinanceRecords",
                        principalColumn: "FinanceRecordId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StatementItems_PaymentSources_PaymentSourceId",
                        column: x => x.PaymentSourceId,
                        principalTable: "PaymentSources",
                        principalColumn: "PaymentSourceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StatementItems_Savings_SavingId",
                        column: x => x.SavingId,
                        principalTable: "Savings",
                        principalColumn: "SavingId");
                    table.ForeignKey(
                        name: "FK_StatementItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinanceRecords_UserId",
                table: "FinanceRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSources_UserId",
                table: "PaymentSources",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBills_CategoryId",
                table: "RecurringBills",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBills_PaymentSourceId",
                table: "RecurringBills",
                column: "PaymentSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_StatementItems_CategoryId",
                table: "StatementItems",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StatementItems_FinanceRecordId",
                table: "StatementItems",
                column: "FinanceRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_StatementItems_PaymentSourceId",
                table: "StatementItems",
                column: "PaymentSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_StatementItems_SavingId",
                table: "StatementItems",
                column: "SavingId");

            migrationBuilder.CreateIndex(
                name: "IX_StatementItems_UserId",
                table: "StatementItems",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecurringBills");

            migrationBuilder.DropTable(
                name: "StatementItems");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "FinanceRecords");

            migrationBuilder.DropTable(
                name: "PaymentSources");

            migrationBuilder.DropTable(
                name: "Savings");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
