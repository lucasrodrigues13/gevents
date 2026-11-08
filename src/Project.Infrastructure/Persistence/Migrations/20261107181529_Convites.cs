using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Convites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "convites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventoId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Link = table.Column<Guid>(type: "uuid", nullable: false),
                    AcessadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RespondidaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_convites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_convites_eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "grupos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grupos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "convidados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConviteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Situacao = table.Column<int>(type: "integer", nullable: false),
                    Faixa = table.Column<int>(type: "integer", nullable: true),
                    Custo = table.Column<int>(type: "integer", nullable: true),
                    Genero = table.Column<int>(type: "integer", nullable: true),
                    Celular = table.Column<string>(type: "text", nullable: true),
                    Observacao = table.Column<string>(type: "text", nullable: true),
                    GrupoId = table.Column<Guid>(type: "uuid", nullable: true),
                    Transfer = table.Column<bool>(type: "boolean", nullable: false),
                    Origem = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_convidados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_convidados_convites_ConviteId",
                        column: x => x.ConviteId,
                        principalTable: "convites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_convidados_grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_convidados_ConviteId",
                table: "convidados",
                column: "ConviteId");

            migrationBuilder.CreateIndex(
                name: "IX_convidados_GrupoId",
                table: "convidados",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_convites_AccountId",
                table: "convites",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_convites_EventoId",
                table: "convites",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_convites_Link",
                table: "convites",
                column: "Link",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_grupos_AccountId",
                table: "grupos",
                column: "AccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "convidados");

            migrationBuilder.DropTable(
                name: "convites");

            migrationBuilder.DropTable(
                name: "grupos");
        }
    }
}
