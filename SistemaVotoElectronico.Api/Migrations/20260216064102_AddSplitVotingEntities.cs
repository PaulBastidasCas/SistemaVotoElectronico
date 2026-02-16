using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SistemaVotoElectronico.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSplitVotingEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidatos");

            migrationBuilder.RenameColumn(
                name: "IdListaSeleccionada",
                table: "Votos",
                newName: "ListaPresidenteId");

            migrationBuilder.RenameColumn(
                name: "IdCandidatoSeleccionado",
                table: "Votos",
                newName: "ListaAsambleistaId");

            migrationBuilder.CreateTable(
                name: "Asambleistas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cedula = table.Column<string>(type: "text", nullable: false),
                    NombreCompleto = table.Column<string>(type: "text", nullable: false),
                    Fotografia = table.Column<string>(type: "text", nullable: true),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    ListaElectoralId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asambleistas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asambleistas_ListaElectorales_ListaElectoralId",
                        column: x => x.ListaElectoralId,
                        principalTable: "ListaElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Presidentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cedula = table.Column<string>(type: "text", nullable: false),
                    NombreCompleto = table.Column<string>(type: "text", nullable: false),
                    Fotografia = table.Column<string>(type: "text", nullable: true),
                    ListaElectoralId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presidentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Presidentes_ListaElectorales_ListaElectoralId",
                        column: x => x.ListaElectoralId,
                        principalTable: "ListaElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vicepresidentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cedula = table.Column<string>(type: "text", nullable: false),
                    NombreCompleto = table.Column<string>(type: "text", nullable: false),
                    Fotografia = table.Column<string>(type: "text", nullable: true),
                    ListaElectoralId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vicepresidentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vicepresidentes_ListaElectorales_ListaElectoralId",
                        column: x => x.ListaElectoralId,
                        principalTable: "ListaElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Votos_ListaAsambleistaId",
                table: "Votos",
                column: "ListaAsambleistaId");

            migrationBuilder.CreateIndex(
                name: "IX_Votos_ListaPresidenteId",
                table: "Votos",
                column: "ListaPresidenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Asambleistas_ListaElectoralId",
                table: "Asambleistas",
                column: "ListaElectoralId");

            migrationBuilder.CreateIndex(
                name: "IX_Presidentes_ListaElectoralId",
                table: "Presidentes",
                column: "ListaElectoralId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vicepresidentes_ListaElectoralId",
                table: "Vicepresidentes",
                column: "ListaElectoralId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Votos_ListaElectorales_ListaAsambleistaId",
                table: "Votos",
                column: "ListaAsambleistaId",
                principalTable: "ListaElectorales",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Votos_ListaElectorales_ListaPresidenteId",
                table: "Votos",
                column: "ListaPresidenteId",
                principalTable: "ListaElectorales",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votos_ListaElectorales_ListaAsambleistaId",
                table: "Votos");

            migrationBuilder.DropForeignKey(
                name: "FK_Votos_ListaElectorales_ListaPresidenteId",
                table: "Votos");

            migrationBuilder.DropTable(
                name: "Asambleistas");

            migrationBuilder.DropTable(
                name: "Presidentes");

            migrationBuilder.DropTable(
                name: "Vicepresidentes");

            migrationBuilder.DropIndex(
                name: "IX_Votos_ListaAsambleistaId",
                table: "Votos");

            migrationBuilder.DropIndex(
                name: "IX_Votos_ListaPresidenteId",
                table: "Votos");

            migrationBuilder.RenameColumn(
                name: "ListaPresidenteId",
                table: "Votos",
                newName: "IdListaSeleccionada");

            migrationBuilder.RenameColumn(
                name: "ListaAsambleistaId",
                table: "Votos",
                newName: "IdCandidatoSeleccionado");

            migrationBuilder.CreateTable(
                name: "Candidatos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ListaElectoralId = table.Column<int>(type: "integer", nullable: true),
                    Cedula = table.Column<string>(type: "text", nullable: false),
                    Contrasena = table.Column<string>(type: "text", nullable: true),
                    Correo = table.Column<string>(type: "text", nullable: false),
                    Fotografia = table.Column<string>(type: "text", nullable: true),
                    NombreCompleto = table.Column<string>(type: "text", nullable: false),
                    OrdenEnLista = table.Column<int>(type: "integer", nullable: false),
                    Rol = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidatos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidatos_ListaElectorales_ListaElectoralId",
                        column: x => x.ListaElectoralId,
                        principalTable: "ListaElectorales",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidatos_Correo",
                table: "Candidatos",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidatos_ListaElectoralId",
                table: "Candidatos",
                column: "ListaElectoralId");
        }
    }
}
