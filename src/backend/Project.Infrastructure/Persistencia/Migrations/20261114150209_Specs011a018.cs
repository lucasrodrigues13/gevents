using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Infrastructure.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class Specs011a018 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiasAntes",
                table: "TarefaModelo",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorEstimado",
                table: "ServicoModelo",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OcultoParaClient",
                table: "ServicoEvento",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Ordem",
                table: "ServicoEvento",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ServicoContaId",
                table: "ServicoEvento",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorEstimado",
                table: "ServicoEvento",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Situacao",
                table: "ParcelaServico",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "PermiteConvites",
                table: "Modelos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PermiteGerais",
                table: "Modelos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PermitePlanejamento",
                table: "Modelos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PermiteServicosFornecedores",
                table: "Modelos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PermiteTarefas",
                table: "Modelos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Fornecedores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    FotoId = table.Column<Guid>(type: "uuid", nullable: true),
                    RazaoSocial = table.Column<string>(type: "text", nullable: true),
                    Cnpj = table.Column<string>(type: "text", nullable: true),
                    Cpf = table.Column<string>(type: "text", nullable: true),
                    NomeResponsavel = table.Column<string>(type: "text", nullable: true),
                    PrecoDe = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    PrecoAte = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    Site = table.Column<string>(type: "text", nullable: true),
                    Facebook = table.Column<string>(type: "text", nullable: true),
                    Instagram = table.Column<string>(type: "text", nullable: true),
                    Youtube = table.Column<string>(type: "text", nullable: true),
                    Pinterest = table.Column<string>(type: "text", nullable: true),
                    Pais = table.Column<string>(type: "text", nullable: true),
                    Cep = table.Column<string>(type: "text", nullable: true),
                    Endereco = table.Column<string>(type: "text", nullable: true),
                    Numero = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    Cidade = table.Column<string>(type: "text", nullable: true),
                    Bairro = table.Column<string>(type: "text", nullable: true),
                    Complemento = table.Column<string>(type: "text", nullable: true),
                    Banco = table.Column<string>(type: "text", nullable: true),
                    Agencia = table.Column<string>(type: "text", nullable: true),
                    ContaBancaria = table.Column<string>(type: "text", nullable: true),
                    TitularConta = table.Column<string>(type: "text", nullable: true),
                    TipoConta = table.Column<string>(type: "text", nullable: true),
                    DocumentosBancarios = table.Column<string>(type: "text", nullable: true),
                    PixTipo = table.Column<string>(type: "text", nullable: true),
                    PixChave = table.Column<string>(type: "text", nullable: true),
                    Diferencial = table.Column<string>(type: "text", nullable: true),
                    DiferencialVisivelCliente = table.Column<bool>(type: "boolean", nullable: false),
                    InformacoesGerais = table.Column<string>(type: "text", nullable: true),
                    Portfolio = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fornecedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FornecedorNoServico",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServicoId = table.Column<Guid>(type: "uuid", nullable: false),
                    FornecedorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Etapa = table.Column<string>(type: "text", nullable: false),
                    ValorOrcamento = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    Observacoes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FornecedorNoServico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FornecedorNoServico_ServicoEvento_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "ServicoEvento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemServicoModelo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServicoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    Texto = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemServicoModelo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemServicoModelo_ServicoModelo_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "ServicoModelo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModelosBriefing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Origem = table.Column<string>(type: "text", nullable: false),
                    Padrao = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelosBriefing", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModelosCronograma",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelosCronograma", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModelosInspiracao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelosInspiracao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModelosMensagem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Texto = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelosMensagem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModelosRoteiro",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelosRoteiro", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServicosConta",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContaId = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Padrao = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicosConta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubitemServico",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServicoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubitemServico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubitemServico_ServicoEvento_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "ServicoEvento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VinculoModeloEvento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModeloId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    BibliotecaId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VinculoModeloEvento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VinculoModeloEvento_Modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "Modelos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailFornecedor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FornecedorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Endereco = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailFornecedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailFornecedor_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrcamentoFornecedor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FornecedorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: true),
                    AnexoId = table.Column<Guid>(type: "uuid", nullable: true),
                    VisivelClientes = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrcamentoFornecedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrcamentoFornecedor_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelefoneFornecedor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FornecedorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Responsavel = table.Column<string>(type: "text", nullable: true),
                    Ddi = table.Column<string>(type: "text", nullable: true),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    Movel = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelefoneFornecedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelefoneFornecedor_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SecaoBriefing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModeloId = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    Ordem = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecaoBriefing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecaoBriefing_ModelosBriefing_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "ModelosBriefing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemCronograma",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModeloId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rotulo = table.Column<string>(type: "text", nullable: true),
                    Data = table.Column<DateOnly>(type: "date", nullable: true),
                    Hora = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    Responsavel = table.Column<string>(type: "text", nullable: true),
                    RecadoFornecedores = table.Column<string>(type: "text", nullable: true),
                    Ordem = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCronograma", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemCronograma_ModelosCronograma_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "ModelosCronograma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemInspiracao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GaleriaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ArquivoId = table.Column<Guid>(type: "uuid", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    Ordem = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemInspiracao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemInspiracao_ModelosInspiracao_GaleriaId",
                        column: x => x.GaleriaId,
                        principalTable: "ModelosInspiracao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SecaoRoteiro",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModeloId = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: true),
                    Horario = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    Musica = table.Column<string>(type: "text", nullable: true),
                    Ordem = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecaoRoteiro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecaoRoteiro_ModelosRoteiro_ModeloId",
                        column: x => x.ModeloId,
                        principalTable: "ModelosRoteiro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FornecedorServico",
                columns: table => new
                {
                    FornecedorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServicoContaId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FornecedorServico", x => new { x.FornecedorId, x.ServicoContaId });
                    table.ForeignKey(
                        name: "FK_FornecedorServico_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FornecedorServico_ServicosConta_ServicoContaId",
                        column: x => x.ServicoContaId,
                        principalTable: "ServicosConta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PerguntaBriefing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SecaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Enunciado = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerguntaBriefing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerguntaBriefing_SecaoBriefing_SecaoId",
                        column: x => x.SecaoId,
                        principalTable: "SecaoBriefing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemRoteiro",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SecaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Texto = table.Column<string>(type: "text", nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemRoteiro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemRoteiro_SecaoRoteiro_SecaoId",
                        column: x => x.SecaoId,
                        principalTable: "SecaoRoteiro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailFornecedor_FornecedorId",
                table: "EmailFornecedor",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Fornecedores_ContaId",
                table: "Fornecedores",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_FornecedorNoServico_ServicoId",
                table: "FornecedorNoServico",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_FornecedorServico_ServicoContaId",
                table: "FornecedorServico",
                column: "ServicoContaId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCronograma_ModeloId",
                table: "ItemCronograma",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemInspiracao_GaleriaId",
                table: "ItemInspiracao",
                column: "GaleriaId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemRoteiro_SecaoId",
                table: "ItemRoteiro",
                column: "SecaoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemServicoModelo_ServicoId",
                table: "ItemServicoModelo",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelosBriefing_ContaId",
                table: "ModelosBriefing",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelosCronograma_ContaId",
                table: "ModelosCronograma",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelosInspiracao_ContaId",
                table: "ModelosInspiracao",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelosMensagem_ContaId",
                table: "ModelosMensagem",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelosRoteiro_ContaId",
                table: "ModelosRoteiro",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrcamentoFornecedor_FornecedorId",
                table: "OrcamentoFornecedor",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_PerguntaBriefing_SecaoId",
                table: "PerguntaBriefing",
                column: "SecaoId");

            migrationBuilder.CreateIndex(
                name: "IX_SecaoBriefing_ModeloId",
                table: "SecaoBriefing",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_SecaoRoteiro_ModeloId",
                table: "SecaoRoteiro",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicosConta_ContaId_Nome",
                table: "ServicosConta",
                columns: new[] { "ContaId", "Nome" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubitemServico_ServicoId",
                table: "SubitemServico",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_TelefoneFornecedor_FornecedorId",
                table: "TelefoneFornecedor",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_VinculoModeloEvento_ModeloId",
                table: "VinculoModeloEvento",
                column: "ModeloId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailFornecedor");

            migrationBuilder.DropTable(
                name: "FornecedorNoServico");

            migrationBuilder.DropTable(
                name: "FornecedorServico");

            migrationBuilder.DropTable(
                name: "ItemCronograma");

            migrationBuilder.DropTable(
                name: "ItemInspiracao");

            migrationBuilder.DropTable(
                name: "ItemRoteiro");

            migrationBuilder.DropTable(
                name: "ItemServicoModelo");

            migrationBuilder.DropTable(
                name: "ModelosMensagem");

            migrationBuilder.DropTable(
                name: "OrcamentoFornecedor");

            migrationBuilder.DropTable(
                name: "PerguntaBriefing");

            migrationBuilder.DropTable(
                name: "SubitemServico");

            migrationBuilder.DropTable(
                name: "TelefoneFornecedor");

            migrationBuilder.DropTable(
                name: "VinculoModeloEvento");

            migrationBuilder.DropTable(
                name: "ServicosConta");

            migrationBuilder.DropTable(
                name: "ModelosCronograma");

            migrationBuilder.DropTable(
                name: "ModelosInspiracao");

            migrationBuilder.DropTable(
                name: "SecaoRoteiro");

            migrationBuilder.DropTable(
                name: "SecaoBriefing");

            migrationBuilder.DropTable(
                name: "Fornecedores");

            migrationBuilder.DropTable(
                name: "ModelosRoteiro");

            migrationBuilder.DropTable(
                name: "ModelosBriefing");

            migrationBuilder.DropColumn(
                name: "DiasAntes",
                table: "TarefaModelo");

            migrationBuilder.DropColumn(
                name: "ValorEstimado",
                table: "ServicoModelo");

            migrationBuilder.DropColumn(
                name: "OcultoParaClient",
                table: "ServicoEvento");

            migrationBuilder.DropColumn(
                name: "Ordem",
                table: "ServicoEvento");

            migrationBuilder.DropColumn(
                name: "ServicoContaId",
                table: "ServicoEvento");

            migrationBuilder.DropColumn(
                name: "ValorEstimado",
                table: "ServicoEvento");

            migrationBuilder.DropColumn(
                name: "Situacao",
                table: "ParcelaServico");

            migrationBuilder.DropColumn(
                name: "PermiteConvites",
                table: "Modelos");

            migrationBuilder.DropColumn(
                name: "PermiteGerais",
                table: "Modelos");

            migrationBuilder.DropColumn(
                name: "PermitePlanejamento",
                table: "Modelos");

            migrationBuilder.DropColumn(
                name: "PermiteServicosFornecedores",
                table: "Modelos");

            migrationBuilder.DropColumn(
                name: "PermiteTarefas",
                table: "Modelos");
        }
    }
}
