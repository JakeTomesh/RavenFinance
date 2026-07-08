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
                IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = 1)
                BEGIN
                    SET IDENTITY_INSERT [Users] ON;
                    INSERT INTO [Users] (UserId, Email, FirstName, LastName, Password)
                    VALUES (1, N'jacobtomesh2@gmail.com', N'Jake', N'Tomesh', N'sesame');
                    SET IDENTITY_INSERT [Users] OFF;
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
