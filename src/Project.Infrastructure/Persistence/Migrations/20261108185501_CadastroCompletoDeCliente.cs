using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CadastroCompletoDeCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ArquivoId",
                table: "clientes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bairro",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Celular",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cep",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cnpj",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Complemento",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cpf",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ddi",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Facebook",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InscricaoMunicipal",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instagram",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Logradouro",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Nascimento",
                table: "clientes",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Numero",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pais",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Papel",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Profissao",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Religiao",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rg",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "links_cadastro_cliente",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    EventoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_links_cadastro_cliente", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_clientes_ArquivoId",
                table: "clientes",
                column: "ArquivoId");

            migrationBuilder.CreateIndex(
                name: "IX_links_cadastro_cliente_AccountId_EventoId",
                table: "links_cadastro_cliente",
                columns: new[] { "AccountId", "EventoId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_clientes_arquivos_ArquivoId",
                table: "clientes",
                column: "ArquivoId",
                principalTable: "arquivos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_clientes_arquivos_ArquivoId",
                table: "clientes");

            migrationBuilder.DropTable(
                name: "links_cadastro_cliente");

            migrationBuilder.DropIndex(
                name: "IX_clientes_ArquivoId",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "ArquivoId",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "Bairro",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "Celular",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "Cep",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "Cnpj",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "Complemento",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "Cpf",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "Ddi",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "Facebook",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "InscricaoMunicipal",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "Instagram",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "Logradouro",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "Nascimento",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "Pais",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "Papel",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "Profissao",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "Religiao",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "Rg",
                table: "clientes");
        }
    }
}
