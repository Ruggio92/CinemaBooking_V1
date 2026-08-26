using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sale",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sale", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Posti",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDSala = table.Column<int>(type: "int", nullable: false),
                    Fila = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Numero = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posti", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Posti_Sale_IDSala",
                        column: x => x.IDSala,
                        principalTable: "Sale",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Spettacoli",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titolo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IDSala = table.Column<int>(type: "int", nullable: false),
                    DataOra = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spettacoli", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Spettacoli_Sale_IDSala",
                        column: x => x.IDSala,
                        principalTable: "Sale",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posti_IDSala_Fila_Numero",
                table: "Posti",
                columns: new[] { "IDSala", "Fila", "Numero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spettacoli_IDSala",
                table: "Spettacoli",
                column: "IDSala");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posti");

            migrationBuilder.DropTable(
                name: "Spettacoli");

            migrationBuilder.DropTable(
                name: "Sale");
        }
    }
}