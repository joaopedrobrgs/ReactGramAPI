using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReactGramAPI.Migrations
{
    /// <inheritdoc />
    public partial class EstabelecendorelaçãoentreUsuarioeFotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Photo",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Photo_UserId",
                table: "Photo",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photo_AspNetUsers_UserId",
                table: "Photo",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photo_AspNetUsers_UserId",
                table: "Photo");

            migrationBuilder.DropIndex(
                name: "IX_Photo_UserId",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Photo");
        }
    }
}
