using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RavenFinance.Migrations
{
    /// <inheritdoc />
    public partial class ViewModelUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isExpense",
                table: "StatementItem",
                newName: "IsExpense");

            migrationBuilder.RenameColumn(
                name: "isDeposit",
                table: "StatementItem",
                newName: "IsDeposit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsExpense",
                table: "StatementItem",
                newName: "isExpense");

            migrationBuilder.RenameColumn(
                name: "IsDeposit",
                table: "StatementItem",
                newName: "isDeposit");
        }
    }
}
