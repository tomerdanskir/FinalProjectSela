using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plane",
                columns: table => new
                {
                    PlaneId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Src = table.Column<int>(type: "int", nullable: false),
                    Dst = table.Column<int>(type: "int", nullable: false),
                    SerialID = table.Column<int>(type: "int", nullable: false),
                    PlanesPurpose = table.Column<int>(type: "int", nullable: false),
                    ReceivedOnSystemDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedProcess = table.Column<bool>(type: "bit", nullable: false),
                    FinishedProcess = table.Column<bool>(type: "bit", nullable: false),
                    FinishedOnDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsEmergency = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plane", x => x.PlaneId);
                });

            migrationBuilder.CreateTable(
                name: "Movement",
                columns: table => new
                {
                    MovementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Src = table.Column<int>(type: "int", nullable: true),
                    Dst = table.Column<int>(type: "int", nullable: false),
                    MovementDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlainOnMoveId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movement", x => x.MovementId);
                    table.ForeignKey(
                        name: "FK_Movement_Plane_PlainOnMoveId",
                        column: x => x.PlainOnMoveId,
                        principalTable: "Plane",
                        principalColumn: "PlaneId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movement_PlainOnMoveId",
                table: "Movement",
                column: "PlainOnMoveId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movement");

            migrationBuilder.DropTable(
                name: "Plane");
        }
    }
}
