using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RavenFinance.Migrations
{
    /// <inheritdoc />
    public partial class DateTimeToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add new temporary int column
            migrationBuilder.AddColumn<int>(
                name: "DueDate_Temp",
                table: "RecurringBills",
                type: "int",
                nullable: false,
                defaultValue: 1);

            // Step 2: Copy day-of-month from old DueDate to new column
            migrationBuilder.Sql(
                @"UPDATE RecurringBills 
                  SET DueDate_Temp = DAY(DueDate)");

            // Step 3: Drop the old datetime2 column
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "RecurringBills");

            // Step 4: Rename the temporary column to DueDate
            migrationBuilder.RenameColumn(
                name: "DueDate_Temp",
                table: "RecurringBills",
                newName: "DueDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add temporary datetime2 column
            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate_Temp",
                table: "RecurringBills",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 1, 1));

            // Step 2: Convert int day back to datetime (using first of month)
            migrationBuilder.Sql(
                @"UPDATE RecurringBills 
                  SET DueDate_Temp = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), DueDate)");

            // Step 3: Drop the int column
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "RecurringBills");

            // Step 4: Rename back
            migrationBuilder.RenameColumn(
                name: "DueDate_Temp",
                table: "RecurringBills",
                newName: "DueDate");
        }
    }
}
