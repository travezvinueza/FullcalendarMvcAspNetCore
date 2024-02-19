using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fullcalendar.Migrations
{
    /// <inheritdoc />
    public partial class PrimeraMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvents_AspNetUsers_UsuarioId",
                table: "CalendarEvents");

            migrationBuilder.AddColumn<string>(
                name: "ClassName",
                table: "CalendarEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvents_AspNetUsers_UsuarioId",
                table: "CalendarEvents",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvents_AspNetUsers_UsuarioId",
                table: "CalendarEvents");

            migrationBuilder.DropColumn(
                name: "ClassName",
                table: "CalendarEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvents_AspNetUsers_UsuarioId",
                table: "CalendarEvents",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
