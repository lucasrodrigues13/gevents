using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Infrastructure.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class Specs019a028 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Compromissos_EventoId",
                table: "Compromissos");

            migrationBuilder.AddColumn<bool>(
                name: "CapturaLeadRsvp",
                table: "Perfis",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "EventoId",
                table: "Compromissos",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Compromissos",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ContratosFunil",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    OportunidadeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Conteudo = table.Column<string>(type: "text", nullable: false),
                    Situacao = table.Column<string>(type: "text", nullable: false),
                    CriadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EnviadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AssinadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModeloId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContratosFunil", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeadsRsvp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    ConviteId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventoId = table.Column<Guid>(type: "uuid", nullable: false),
                    NomeConvite = table.Column<string>(type: "text", nullable: false),
                    NomeEvento = table.Column<string>(type: "text", nullable: false),
                    Telefone = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Situacao = table.Column<string>(type: "text", nullable: false),
                    OportunidadeId = table.Column<Guid>(type: "uuid", nullable: true),
                    ContatadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadsRsvp", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetricasRsvpFunil",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    Visualizacoes = table.Column<int>(type: "integer", nullable: false),
                    CliquesBotao = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetricasRsvpFunil", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModelosContratoFunil",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Texto = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelosContratoFunil", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModelosOrcamentoFunil",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Texto = table.Column<string>(type: "text", nullable: false),
                    CorPrimaria = table.Column<string>(type: "text", nullable: true),
                    CorSecundaria = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelosOrcamentoFunil", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModelosServicoFunil",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Valor = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelosServicoFunil", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Oportunidades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Estagio = table.Column<string>(type: "text", nullable: false),
                    Fonte = table.Column<string>(type: "text", nullable: true),
                    ValorProposta = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    Responsavel = table.Column<string>(type: "text", nullable: false),
                    ServicoProduto = table.Column<string>(type: "text", nullable: true),
                    ModeloServicoFunilId = table.Column<Guid>(type: "uuid", nullable: true),
                    TipoEvento = table.Column<string>(type: "text", nullable: true),
                    QuantidadeConvidados = table.Column<int>(type: "integer", nullable: true),
                    DataPrevista = table.Column<DateOnly>(type: "date", nullable: true),
                    Horarios = table.Column<string>(type: "text", nullable: true),
                    QuantidadePadrinhos = table.Column<int>(type: "integer", nullable: true),
                    QuantidadeDamas = table.Column<int>(type: "integer", nullable: true),
                    LocalCerimonia = table.Column<string>(type: "text", nullable: true),
                    EnderecoCerimonia = table.Column<string>(type: "text", nullable: true),
                    LocalRecepcao = table.Column<string>(type: "text", nullable: true),
                    EnderecoRecepcao = table.Column<string>(type: "text", nullable: true),
                    Uf = table.Column<string>(type: "text", nullable: true),
                    Cidade = table.Column<string>(type: "text", nullable: true),
                    MotivoDescarte = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntradaNoEstagioEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oportunidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrcamentosFunil",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    OportunidadeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Conteudo = table.Column<string>(type: "text", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    Situacao = table.Column<string>(type: "text", nullable: false),
                    CriadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EnviadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RespondidoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModeloId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrcamentosFunil", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TarefasFunil",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    OportunidadeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Observacao = table.Column<string>(type: "text", nullable: true),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    Responsavel = table.Column<string>(type: "text", nullable: false),
                    Prazo = table.Column<DateOnly>(type: "date", nullable: false),
                    Completa = table.Column<bool>(type: "boolean", nullable: false),
                    AnexoArquivoId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarefasFunil", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContatoOportunidade",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OportunidadeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Telefone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContatoOportunidade", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContatoOportunidade_Oportunidades_OportunidadeId",
                        column: x => x.OportunidadeId,
                        principalTable: "Oportunidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Compromissos_EventoId",
                table: "Compromissos",
                column: "EventoId",
                unique: true,
                filter: "\"EventoId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ContatoOportunidade_OportunidadeId",
                table: "ContatoOportunidade",
                column: "OportunidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_ContratosFunil_ContaId",
                table: "ContratosFunil",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadsRsvp_ContaId",
                table: "LeadsRsvp",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_MetricasRsvpFunil_ContaId",
                table: "MetricasRsvpFunil",
                column: "ContaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModelosContratoFunil_ContaId",
                table: "ModelosContratoFunil",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelosOrcamentoFunil_ContaId",
                table: "ModelosOrcamentoFunil",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelosServicoFunil_ContaId",
                table: "ModelosServicoFunil",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_Oportunidades_ContaId",
                table: "Oportunidades",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrcamentosFunil_ContaId",
                table: "OrcamentosFunil",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_TarefasFunil_ContaId",
                table: "TarefasFunil",
                column: "ContaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContatoOportunidade");

            migrationBuilder.DropTable(
                name: "ContratosFunil");

            migrationBuilder.DropTable(
                name: "LeadsRsvp");

            migrationBuilder.DropTable(
                name: "MetricasRsvpFunil");

            migrationBuilder.DropTable(
                name: "ModelosContratoFunil");

            migrationBuilder.DropTable(
                name: "ModelosOrcamentoFunil");

            migrationBuilder.DropTable(
                name: "ModelosServicoFunil");

            migrationBuilder.DropTable(
                name: "OrcamentosFunil");

            migrationBuilder.DropTable(
                name: "TarefasFunil");

            migrationBuilder.DropTable(
                name: "Oportunidades");

            migrationBuilder.DropIndex(
                name: "IX_Compromissos_EventoId",
                table: "Compromissos");

            migrationBuilder.DropColumn(
                name: "CapturaLeadRsvp",
                table: "Perfis");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Compromissos");

            migrationBuilder.AlterColumn<Guid>(
                name: "EventoId",
                table: "Compromissos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Compromissos_EventoId",
                table: "Compromissos",
                column: "EventoId",
                unique: true);
        }
    }
}
