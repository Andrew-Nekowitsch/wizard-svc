using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wizardsvc.Migrations
{
    /// <inheritdoc />
    public partial class spell : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Spells_Player_PlayerId",
                table: "Spells");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Spells",
                table: "Spells");

            migrationBuilder.RenameTable(
                name: "Spells",
                newName: "Spell");

            migrationBuilder.RenameIndex(
                name: "IX_Spells_PlayerId",
                table: "Spell",
                newName: "IX_Spell_PlayerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Spell",
                table: "Spell",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Spell_Player_PlayerId",
                table: "Spell",
                column: "PlayerId",
                principalTable: "Player",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Spell_Player_PlayerId",
                table: "Spell");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Spell",
                table: "Spell");

            migrationBuilder.RenameTable(
                name: "Spell",
                newName: "Spells");

            migrationBuilder.RenameIndex(
                name: "IX_Spell_PlayerId",
                table: "Spells",
                newName: "IX_Spells_PlayerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Spells",
                table: "Spells",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Spells_Player_PlayerId",
                table: "Spells",
                column: "PlayerId",
                principalTable: "Player",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
