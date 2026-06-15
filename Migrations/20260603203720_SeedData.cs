using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RavenFinance.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if user already exists before inserting
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = 1)
                BEGIN
                    INSERT INTO Users (Id, Email, FirstName, LastName, Password)
                    VALUES (1, 'jacobtomesh2@gmail.com', 'Jake', 'Tomesh', 'sesame')
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
