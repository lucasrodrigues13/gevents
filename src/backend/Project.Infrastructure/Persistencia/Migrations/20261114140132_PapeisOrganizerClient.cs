using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Infrastructure.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class PapeisOrganizerClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizerId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OrganizerId",
                table: "AspNetUsers",
                column: "OrganizerId");

            migrationBuilder.Sql(
                """
                UPDATE "AspNetRoles"
                SET "Name" = 'Organizer', "NormalizedName" = 'ORGANIZER'
                WHERE "NormalizedName" = 'USER';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE "AspNetRoles"
                SET "Name" = 'User', "NormalizedName" = 'USER'
                WHERE "NormalizedName" = 'ORGANIZER';
                """);

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_OrganizerId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OrganizerId",
                table: "AspNetUsers");
        }
    }
}
