using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RavenFinance.Migrations
{
    /// <inheritdoc />
    public partial class UserModelFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Only delete if the user exists
            migrationBuilder.Sql("IF EXISTS (SELECT 1 FROM Users WHERE UserId = 1) DELETE FROM Users WHERE UserId = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "FirstName", "LastName", "Password" },
                values: new object[] { 1, "jacobtomesh2@gmail.com", "Jake", "Tomesh", "sesame" });
        }
    }
}
