using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class Addrelationbetweenuserandtodo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "ToDo",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ToDo_CreatedById",
                table: "ToDo",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDo_AspNetUsers_CreatedById",
                table: "ToDo",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDo_AspNetUsers_CreatedById",
                table: "ToDo");

            migrationBuilder.DropIndex(
                name: "IX_ToDo_CreatedById",
                table: "ToDo");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "ToDo");
        }
    }
}
