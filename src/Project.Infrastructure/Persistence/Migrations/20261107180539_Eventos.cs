using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Eventos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "arquivos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NomeOriginal = table.Column<string>(type: "text", nullable: false),
                    CaminhoRelativo = table.Column<string>(type: "text", nullable: false),
                    Mime = table.Column<string>(type: "text", nullable: false),
                    Tamanho = table.Column<long>(type: "bigint", nullable: false),
                    Categoria = table.Column<string>(type: "text", nullable: false),
                    GravadoEmUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_arquivos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "eventos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Situacao = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<DateOnly>(type: "date", nullable: true),
                    Hora = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    Tipo = table.Column<int>(type: "integer", nullable: true),
                    Tags = table.Column<List<string>>(type: "text[]", nullable: false),
                    CasalNome = table.Column<string>(type: "text", nullable: true),
                    CasalPapel = table.Column<string>(type: "text", nullable: true),
                    CasalEmail = table.Column<string>(type: "text", nullable: true),
                    CasalCelular = table.Column<string>(type: "text", nullable: true),
                    OrcamentoEstimado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    ConvidadosEstimados = table.Column<int>(type: "integer", nullable: true),
                    LocalCerimonia = table.Column<string>(type: "text", nullable: true),
                    LocalRecepcao = table.Column<string>(type: "text", nullable: true),
                    ArquivoId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_eventos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_eventos_arquivos_ArquivoId",
                        column: x => x.ArquivoId,
                        principalTable: "arquivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_eventos_AccountId_Situacao",
                table: "eventos",
                columns: new[] { "AccountId", "Situacao" });

            migrationBuilder.CreateIndex(
                name: "IX_eventos_ArquivoId",
                table: "eventos",
                column: "ArquivoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "eventos");

            migrationBuilder.DropTable(
                name: "arquivos");
        }
    }
}
