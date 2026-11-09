using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CadastroCompletoDeEvento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServicoEventoId",
                table: "tarefas",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CerimoniaBairro",
                table: "eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CerimoniaCep",
                table: "eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CerimoniaCidade",
                table: "eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CerimoniaEstado",
                table: "eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CerimoniaLogradouro",
                table: "eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CerimoniaNumero",
                table: "eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CerimoniaPais",
                table: "eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DamasEPajens",
                table: "eventos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "InicioCerimonia",
                table: "eventos",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "InicioRecepcao",
                table: "eventos",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModeloId",
                table: "eventos",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotificarAssessor",
                table: "eventos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotificarAssistentes",
                table: "eventos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotificarClientes",
                table: "eventos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Padrinhos",
                table: "eventos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimeiroNome",
                table: "eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecepcaoBairro",
                table: "eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecepcaoCep",
                table: "eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecepcaoCidade",
                table: "eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecepcaoEstado",
                table: "eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecepcaoLogradouro",
                table: "eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecepcaoNumero",
                table: "eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecepcaoPais",
                table: "eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SalvarNaAgenda",
                table: "eventos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SegundoNome",
                table: "eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenhaDoSite",
                table: "eventos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "TempoDeFesta",
                table: "eventos",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "compromissos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    EventoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<DateOnly>(type: "date", nullable: false),
                    Hora = table.Column<TimeOnly>(type: "time without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_compromissos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "modelos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_modelos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "parcelas_evento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    EventoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServicoEventoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Numero = table.Column<int>(type: "integer", nullable: false),
                    Vencimento = table.Column<DateOnly>(type: "date", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Entrada = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parcelas_evento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "servicos_evento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    EventoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Contratado = table.Column<bool>(type: "boolean", nullable: false),
                    Fornecedor = table.Column<string>(type: "text", nullable: true),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servicos_evento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "servicos_modelo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModeloId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servicos_modelo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_servicos_modelo_modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "modelos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tarefas_modelo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServicoModeloId = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tarefas_modelo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tarefas_modelo_servicos_modelo_ServicoModeloId",
                        column: x => x.ServicoModeloId,
                        principalTable: "servicos_modelo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_compromissos_AccountId_EventoId",
                table: "compromissos",
                columns: new[] { "AccountId", "EventoId" });

            migrationBuilder.CreateIndex(
                name: "IX_modelos_AccountId",
                table: "modelos",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_parcelas_evento_AccountId_EventoId",
                table: "parcelas_evento",
                columns: new[] { "AccountId", "EventoId" });

            migrationBuilder.CreateIndex(
                name: "IX_servicos_evento_AccountId_EventoId",
                table: "servicos_evento",
                columns: new[] { "AccountId", "EventoId" });

            migrationBuilder.CreateIndex(
                name: "IX_servicos_modelo_ModeloId",
                table: "servicos_modelo",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_tarefas_modelo_ServicoModeloId",
                table: "tarefas_modelo",
                column: "ServicoModeloId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "compromissos");

            migrationBuilder.DropTable(
                name: "parcelas_evento");

            migrationBuilder.DropTable(
                name: "servicos_evento");

            migrationBuilder.DropTable(
                name: "tarefas_modelo");

            migrationBuilder.DropTable(
                name: "servicos_modelo");

            migrationBuilder.DropTable(
                name: "modelos");

            migrationBuilder.DropColumn(
                name: "ServicoEventoId",
                table: "tarefas");

            migrationBuilder.DropColumn(
                name: "CerimoniaBairro",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "CerimoniaCep",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "CerimoniaCidade",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "CerimoniaEstado",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "CerimoniaLogradouro",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "CerimoniaNumero",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "CerimoniaPais",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "DamasEPajens",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "InicioCerimonia",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "InicioRecepcao",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "ModeloId",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "NotificarAssessor",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "NotificarAssistentes",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "NotificarClientes",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "Padrinhos",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "PrimeiroNome",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "RecepcaoBairro",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "RecepcaoCep",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "RecepcaoCidade",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "RecepcaoEstado",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "RecepcaoLogradouro",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "RecepcaoNumero",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "RecepcaoPais",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "SalvarNaAgenda",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "SegundoNome",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "SenhaDoSite",
                table: "eventos");

            migrationBuilder.DropColumn(
                name: "TempoDeFesta",
                table: "eventos");
        }
    }
}
