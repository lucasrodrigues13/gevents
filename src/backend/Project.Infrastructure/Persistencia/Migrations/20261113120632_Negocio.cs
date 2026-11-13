using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Infrastructure.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class Negocio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Arquivos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    NomeOriginal = table.Column<string>(type: "text", nullable: false),
                    TipoConteudo = table.Column<string>(type: "text", nullable: false),
                    Tamanho = table.Column<long>(type: "bigint", nullable: false),
                    CaminhoRelativo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arquivos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Papel = table.Column<string>(type: "text", nullable: false),
                    Nascimento = table.Column<DateOnly>(type: "date", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Ddi = table.Column<string>(type: "text", nullable: false),
                    Celular = table.Column<string>(type: "text", nullable: false),
                    TelefoneFixo = table.Column<string>(type: "text", nullable: true),
                    Cpf = table.Column<string>(type: "text", nullable: true),
                    Rg = table.Column<string>(type: "text", nullable: true),
                    Cnpj = table.Column<string>(type: "text", nullable: true),
                    Profissao = table.Column<string>(type: "text", nullable: true),
                    InscricaoMunicipal = table.Column<string>(type: "text", nullable: true),
                    Religiao = table.Column<string>(type: "text", nullable: true),
                    Observacao = table.Column<string>(type: "text", nullable: true),
                    Facebook = table.Column<string>(type: "text", nullable: true),
                    Instagram = table.Column<string>(type: "text", nullable: true),
                    Pais = table.Column<string>(type: "text", nullable: true),
                    Cep = table.Column<string>(type: "text", nullable: true),
                    Endereco = table.Column<string>(type: "text", nullable: true),
                    Numero = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    Cidade = table.Column<string>(type: "text", nullable: true),
                    Bairro = table.Column<string>(type: "text", nullable: true),
                    Complemento = table.Column<string>(type: "text", nullable: true),
                    FotoId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    ModeloId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    PrimeiroNome = table.Column<string>(type: "text", nullable: true),
                    SegundoNome = table.Column<string>(type: "text", nullable: true),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    Situacao = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<DateOnly>(type: "date", nullable: true),
                    Hora = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    OrcamentoEstimado = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    ConvidadosEstimados = table.Column<int>(type: "integer", nullable: true),
                    TempoFesta = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    InicioCerimonia = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    InicioRecepcao = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    QuantidadePadrinhos = table.Column<int>(type: "integer", nullable: true),
                    QuantidadeDamas = table.Column<int>(type: "integer", nullable: true),
                    SenhaSite = table.Column<string>(type: "text", nullable: true),
                    NotificarAssessor = table.Column<bool>(type: "boolean", nullable: false),
                    NotificarAssistentes = table.Column<bool>(type: "boolean", nullable: false),
                    NotificarClientes = table.Column<bool>(type: "boolean", nullable: false),
                    SalvarNaAgenda = table.Column<bool>(type: "boolean", nullable: false),
                    ImagemId = table.Column<Guid>(type: "uuid", nullable: true),
                    TransferAtivo = table.Column<bool>(type: "boolean", nullable: false),
                    Cerimonia_Nome = table.Column<string>(type: "text", nullable: true),
                    Cerimonia_Pais = table.Column<string>(type: "text", nullable: true),
                    Cerimonia_Cep = table.Column<string>(type: "text", nullable: true),
                    Cerimonia_Endereco = table.Column<string>(type: "text", nullable: true),
                    Cerimonia_Numero = table.Column<string>(type: "text", nullable: true),
                    Cerimonia_Estado = table.Column<string>(type: "text", nullable: true),
                    Cerimonia_Cidade = table.Column<string>(type: "text", nullable: true),
                    Cerimonia_Bairro = table.Column<string>(type: "text", nullable: true),
                    Recepcao_Nome = table.Column<string>(type: "text", nullable: true),
                    Recepcao_Pais = table.Column<string>(type: "text", nullable: true),
                    Recepcao_Cep = table.Column<string>(type: "text", nullable: true),
                    Recepcao_Endereco = table.Column<string>(type: "text", nullable: true),
                    Recepcao_Numero = table.Column<string>(type: "text", nullable: true),
                    Recepcao_Estado = table.Column<string>(type: "text", nullable: true),
                    Recepcao_Cidade = table.Column<string>(type: "text", nullable: true),
                    Recepcao_Bairro = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Grupos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modelos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modelos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Perfis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    NomeEmpresa = table.Column<string>(type: "text", nullable: true),
                    Responsavel = table.Column<string>(type: "text", nullable: true),
                    Documento = table.Column<string>(type: "text", nullable: true),
                    Celular = table.Column<string>(type: "text", nullable: true),
                    Nascimento = table.Column<DateOnly>(type: "date", nullable: true),
                    Endereco = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Compromissos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    EventoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<DateOnly>(type: "date", nullable: true),
                    Hora = table.Column<TimeOnly>(type: "time without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compromissos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Compromissos_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocalTransfer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalTransfer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalTransfer_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServicoEvento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    FornecedorContaId = table.Column<string>(type: "text", nullable: true),
                    Contratado = table.Column<bool>(type: "boolean", nullable: false),
                    ValorContratado = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicoEvento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServicoEvento_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tarefas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    Prazo = table.Column<DateOnly>(type: "date", nullable: true),
                    Situacao = table.Column<string>(type: "text", nullable: false),
                    ResponsavelContaId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarefas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tarefas_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vinculos",
                columns: table => new
                {
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vinculos", x => new { x.ClienteId, x.EventoId });
                    table.ForeignKey(
                        name: "FK_Vinculos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vinculos_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Convites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    GrupoId = table.Column<Guid>(type: "uuid", nullable: true),
                    Observacao = table.Column<string>(type: "text", nullable: true),
                    PrincipalId = table.Column<Guid>(type: "uuid", nullable: true),
                    UsaTransfer = table.Column<bool>(type: "boolean", nullable: false),
                    LocalOrigem = table.Column<string>(type: "text", nullable: true),
                    Token = table.Column<string>(type: "text", nullable: false),
                    AcessadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RespondidoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Convites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Convites_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Convites_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ServicoModelo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModeloId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicoModelo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServicoModelo_Modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "Modelos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TarefaModelo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModeloId = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarefaModelo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TarefaModelo_Modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "Modelos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventoTagConta",
                columns: table => new
                {
                    EventosId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoTagConta", x => new { x.EventosId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_EventoTagConta_Eventos_EventosId",
                        column: x => x.EventosId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoTagConta_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParcelaServico",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServicoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Numero = table.Column<int>(type: "integer", nullable: false),
                    Vencimento = table.Column<DateOnly>(type: "date", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    Entrada = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParcelaServico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParcelaServico_ServicoEvento_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "ServicoEvento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Convidados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConviteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Ddi = table.Column<string>(type: "text", nullable: true),
                    Celular = table.Column<string>(type: "text", nullable: true),
                    Situacao = table.Column<string>(type: "text", nullable: false),
                    Mesa = table.Column<string>(type: "text", nullable: true),
                    Genero = table.Column<string>(type: "text", nullable: true),
                    Faixa = table.Column<string>(type: "text", nullable: true),
                    Custo = table.Column<string>(type: "text", nullable: true),
                    Rg = table.Column<string>(type: "text", nullable: true),
                    Cpf = table.Column<string>(type: "text", nullable: true),
                    Observacao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Convidados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Convidados_Convites_ConviteId",
                        column: x => x.ConviteId,
                        principalTable: "Convites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Compromissos_EventoId",
                table: "Compromissos",
                column: "EventoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Convidados_ConviteId",
                table: "Convidados",
                column: "ConviteId");

            migrationBuilder.CreateIndex(
                name: "IX_Convites_EventoId",
                table: "Convites",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Convites_GrupoId",
                table: "Convites",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_Convites_Token",
                table: "Convites",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventoTagConta_TagsId",
                table: "EventoTagConta",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalTransfer_EventoId",
                table: "LocalTransfer",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_ParcelaServico_ServicoId",
                table: "ParcelaServico",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Perfis_ContaId",
                table: "Perfis",
                column: "ContaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServicoEvento_EventoId",
                table: "ServicoEvento",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicoModelo_ModeloId",
                table: "ServicoModelo",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ContaId_Nome",
                table: "Tags",
                columns: new[] { "ContaId", "Nome" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TarefaModelo_ModeloId",
                table: "TarefaModelo",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_EventoId",
                table: "Tarefas",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Vinculos_EventoId",
                table: "Vinculos",
                column: "EventoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arquivos");

            migrationBuilder.DropTable(
                name: "Compromissos");

            migrationBuilder.DropTable(
                name: "Convidados");

            migrationBuilder.DropTable(
                name: "EventoTagConta");

            migrationBuilder.DropTable(
                name: "LocalTransfer");

            migrationBuilder.DropTable(
                name: "ParcelaServico");

            migrationBuilder.DropTable(
                name: "Perfis");

            migrationBuilder.DropTable(
                name: "ServicoModelo");

            migrationBuilder.DropTable(
                name: "TarefaModelo");

            migrationBuilder.DropTable(
                name: "Tarefas");

            migrationBuilder.DropTable(
                name: "Vinculos");

            migrationBuilder.DropTable(
                name: "Convites");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "ServicoEvento");

            migrationBuilder.DropTable(
                name: "Modelos");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Grupos");

            migrationBuilder.DropTable(
                name: "Eventos");
        }
    }
}
