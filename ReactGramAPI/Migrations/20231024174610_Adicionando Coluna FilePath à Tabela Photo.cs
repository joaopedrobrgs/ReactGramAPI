﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReactGramAPI.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoColunaFilePathàTabelaPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Photo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Photo");
        }
    }
}
