using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConvitesEConvidadosCompleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TransferAtivo",
                table: "eventos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ConvidadoPrincipalId",
                table: "convites",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GrupoId",
                table: "convites",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LocalTransferId",
                table: "convites",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "convites",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UsaTransfer",
                table: "convites",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Cpf",
                table: "convidados",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ddi",
                table: "convidados",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mesa",
                table: "convidados",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rg",
                table: "convidados",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "locais_transfer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventoId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locais_transfer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_locais_transfer_eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_convites_GrupoId",
                table: "convites",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_convites_LocalTransferId",
                table: "convites",
                column: "LocalTransferId");

            migrationBuilder.CreateIndex(
                name: "IX_locais_transfer_EventoId",
                table: "locais_transfer",
                column: "EventoId");

            migrationBuilder.AddForeignKey(
                name: "FK_convites_grupos_GrupoId",
                table: "convites",
                column: "GrupoId",
                principalTable: "grupos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_convites_locais_transfer_LocalTransferId",
                table: "convites",
                column: "LocalTransferId",
                principalTable: "locais_transfer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_convites_grupos_GrupoId",
                table: "convites");

            migrationBuilder.DropForeignKey(
                name: "FK_convites_locais_transfer_LocalTransferId",
                table: "convites");

            migrationBuilder.DropTable(
                name: "locais_transfer");

            migrationBuilder.DropIndex(
                name: "IX_convites_GrupoId",
                table: "convites");

            migrationBuilder.DropIndex(
                name: "IX_convites_LocalTransferId",
                table: "convites");

            migrationBuilder.DropColumn(
                name: "TransferAtivo",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "ConvidadoPrincipalId",
                table: "convites");

            migrationBuilder.DropColumn(
                name: "GrupoId",
                table: "convites");

            migrationBuilder.DropColumn(
                name: "LocalTransferId",
                table: "convites");

            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "convites");

            migrationBuilder.DropColumn(
                name: "UsaTransfer",
                table: "convites");

            migrationBuilder.DropColumn(
                name: "Cpf",
                table: "convidados");

            migrationBuilder.DropColumn(
                name: "Ddi",
                table: "convidados");

            migrationBuilder.DropColumn(
                name: "Mesa",
                table: "convidados");

            migrationBuilder.DropColumn(
                name: "Rg",
                table: "convidados");
        }
    }
}
