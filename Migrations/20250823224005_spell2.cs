using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wizardsvc.Migrations
{
    /// <inheritdoc />
    public partial class spell2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Spell_Player_PlayerId",
                table: "Spell");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Spell");

            migrationBuilder.AlterColumn<int>(
                name: "PlayerId",
                table: "Spell",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Spell",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Spell_Player_PlayerId",
                table: "Spell",
                column: "PlayerId",
                principalTable: "Player",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Spell_Player_PlayerId",
                table: "Spell");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Spell");

            migrationBuilder.AlterColumn<int>(
                name: "PlayerId",
                table: "Spell",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Spell",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Spell_Player_PlayerId",
                table: "Spell",
                column: "PlayerId",
                principalTable: "Player",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
