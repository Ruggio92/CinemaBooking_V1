using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prenotazioni",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDSpettacolo = table.Column<int>(type: "int", nullable: false),
                    NomeCliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataCreazione = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Stato = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prenotazioni", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PostiPrenotati",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDPrenotazione = table.Column<int>(type: "int", nullable: false),
                    IDSpettacolo = table.Column<int>(type: "int", nullable: false),
                    IDPosto = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostiPrenotati", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PostiPrenotati_Prenotazioni_IDPrenotazione",
                        column: x => x.IDPrenotazione,
                        principalTable: "Prenotazioni",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostiPrenotati_IDPrenotazione",
                table: "PostiPrenotati",
                column: "IDPrenotazione");

            migrationBuilder.CreateIndex(
                name: "IX_PostiPrenotati_IDSpettacolo_IDPosto",
                table: "PostiPrenotati",
                columns: new[] { "IDSpettacolo", "IDPosto" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostiPrenotati");

            migrationBuilder.DropTable(
                name: "Prenotazioni");
        }
    }
}
