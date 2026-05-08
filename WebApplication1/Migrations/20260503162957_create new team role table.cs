using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class createnewteamroletable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamRoleId",
                table: "TeamUserJunction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TeamRoles",
                columns: table => new
                {
                    TeamRoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamRoles", x => x.TeamRoleId);
                    table.ForeignKey(
                        name: "FK_TeamRoles_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamUserJunction_TeamRoleId",
                table: "TeamUserJunction",
                column: "TeamRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamRoles_TeamId",
                table: "TeamRoles",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamUserJunction_TeamRoles_TeamRoleId",
                table: "TeamUserJunction",
                column: "TeamRoleId",
                principalTable: "TeamRoles",
                principalColumn: "TeamRoleId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamUserJunction_TeamRoles_TeamRoleId",
                table: "TeamUserJunction");

            migrationBuilder.DropTable(
                name: "TeamRoles");

            migrationBuilder.DropIndex(
                name: "IX_TeamUserJunction_TeamRoleId",
                table: "TeamUserJunction");

            migrationBuilder.DropColumn(
                name: "TeamRoleId",
                table: "TeamUserJunction");
        }
    }
}
