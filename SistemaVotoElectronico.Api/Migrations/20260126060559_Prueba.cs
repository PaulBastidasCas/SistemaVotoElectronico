using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SistemaVotoElectronico.Api.Migrations
{
    /// <inheritdoc />
    public partial class Prueba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Administradores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cedula = table.Column<string>(type: "text", nullable: false),
                    NombreCompleto = table.Column<string>(type: "text", nullable: false),
                    Correo = table.Column<string>(type: "text", nullable: false),
                    Contrasena = table.Column<string>(type: "text", nullable: true),
                    Fotografia = table.Column<string>(type: "text", nullable: true),
                    Rol = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Elecciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    TipoEleccion = table.Column<string>(type: "text", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaFinalizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Activa = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Elecciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JefesDeMesa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cedula = table.Column<string>(type: "text", nullable: false),
                    NombreCompleto = table.Column<string>(type: "text", nullable: false),
                    Correo = table.Column<string>(type: "text", nullable: false),
                    Contrasena = table.Column<string>(type: "text", nullable: true),
                    Fotografia = table.Column<string>(type: "text", nullable: true),
                    Rol = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JefesDeMesa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesRecuperacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Correo = table.Column<string>(type: "text", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    Expiracion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Usado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesRecuperacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Votantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cedula = table.Column<string>(type: "text", nullable: false),
                    NombreCompleto = table.Column<string>(type: "text", nullable: false),
                    Correo = table.Column<string>(type: "text", nullable: false),
                    Contrasena = table.Column<string>(type: "text", nullable: true),
                    Fotografia = table.Column<string>(type: "text", nullable: true),
                    Rol = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votantes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ListaElectorales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    NumeroLista = table.Column<string>(type: "text", nullable: false),
                    Siglas = table.Column<string>(type: "text", nullable: false),
                    Logotipo = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    EleccionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListaElectorales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListaElectorales_Elecciones_EleccionId",
                        column: x => x.EleccionId,
                        principalTable: "Elecciones",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Votos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EleccionId = table.Column<int>(type: "integer", nullable: true),
                    IdCandidatoSeleccionado = table.Column<int>(type: "integer", nullable: true),
                    IdListaSeleccionada = table.Column<int>(type: "integer", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votos_Elecciones_EleccionId",
                        column: x => x.EleccionId,
                        principalTable: "Elecciones",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Mesas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Ubicacion = table.Column<string>(type: "text", nullable: false),
                    EleccionId = table.Column<int>(type: "integer", nullable: true),
                    JefeDeMesaId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mesas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mesas_Elecciones_EleccionId",
                        column: x => x.EleccionId,
                        principalTable: "Elecciones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Mesas_JefesDeMesa_JefeDeMesaId",
                        column: x => x.JefeDeMesaId,
                        principalTable: "JefesDeMesa",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Candidatos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cedula = table.Column<string>(type: "text", nullable: false),
                    NombreCompleto = table.Column<string>(type: "text", nullable: false),
                    Correo = table.Column<string>(type: "text", nullable: false),
                    Contrasena = table.Column<string>(type: "text", nullable: true),
                    Fotografia = table.Column<string>(type: "text", nullable: true),
                    Rol = table.Column<string>(type: "text", nullable: false),
                    OrdenEnLista = table.Column<int>(type: "integer", nullable: false),
                    ListaElectoralId = table.Column<int>(type: "integer", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "PadronElectorales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VotanteId = table.Column<int>(type: "integer", nullable: true),
                    EleccionId = table.Column<int>(type: "integer", nullable: true),
                    MesaId = table.Column<int>(type: "integer", nullable: true),
                    CodigoEnlace = table.Column<string>(type: "text", nullable: true),
                    FechaGeneracionCodigo = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CodigoCanjeado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaVoto = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    VotoPlanchaRealizado = table.Column<bool>(type: "boolean", nullable: false),
                    VotoNominalRealizado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PadronElectorales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PadronElectorales_Elecciones_EleccionId",
                        column: x => x.EleccionId,
                        principalTable: "Elecciones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PadronElectorales_Mesas_MesaId",
                        column: x => x.MesaId,
                        principalTable: "Mesas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PadronElectorales_Votantes_VotanteId",
                        column: x => x.VotanteId,
                        principalTable: "Votantes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administradores_Correo",
                table: "Administradores",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidatos_Correo",
                table: "Candidatos",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidatos_ListaElectoralId",
                table: "Candidatos",
                column: "ListaElectoralId");

            migrationBuilder.CreateIndex(
                name: "IX_JefesDeMesa_Correo",
                table: "JefesDeMesa",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ListaElectorales_EleccionId",
                table: "ListaElectorales",
                column: "EleccionId");

            migrationBuilder.CreateIndex(
                name: "IX_Mesas_EleccionId",
                table: "Mesas",
                column: "EleccionId");

            migrationBuilder.CreateIndex(
                name: "IX_Mesas_JefeDeMesaId",
                table: "Mesas",
                column: "JefeDeMesaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PadronElectorales_CodigoEnlace",
                table: "PadronElectorales",
                column: "CodigoEnlace");

            migrationBuilder.CreateIndex(
                name: "IX_PadronElectorales_EleccionId",
                table: "PadronElectorales",
                column: "EleccionId");

            migrationBuilder.CreateIndex(
                name: "IX_PadronElectorales_MesaId",
                table: "PadronElectorales",
                column: "MesaId");

            migrationBuilder.CreateIndex(
                name: "IX_PadronElectorales_VotanteId",
                table: "PadronElectorales",
                column: "VotanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Votantes_Cedula",
                table: "Votantes",
                column: "Cedula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votantes_Correo",
                table: "Votantes",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votos_EleccionId",
                table: "Votos",
                column: "EleccionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administradores");

            migrationBuilder.DropTable(
                name: "Candidatos");

            migrationBuilder.DropTable(
                name: "PadronElectorales");

            migrationBuilder.DropTable(
                name: "SolicitudesRecuperacion");

            migrationBuilder.DropTable(
                name: "Votos");

            migrationBuilder.DropTable(
                name: "ListaElectorales");

            migrationBuilder.DropTable(
                name: "Mesas");

            migrationBuilder.DropTable(
                name: "Votantes");

            migrationBuilder.DropTable(
                name: "Elecciones");

            migrationBuilder.DropTable(
                name: "JefesDeMesa");
        }
    }
}
