using Microsoft.EntityFrameworkCore;
using Project.Domain.Clientes;
using Project.Domain.Convidados;
using Project.Domain.Eventos;
using Project.Domain.Fornecedores;
using Project.Domain.Funil;
using Project.Domain.Modelos;
using Project.Domain.Perfil;
using Project.Domain.Tarefas;
using Project.Infrastructure.Conta;

namespace Project.Infrastructure.Persistencia;

public static class Mapa
{
    public static void Aplicar(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>()
            .HasIndex(usuario => usuario.OrganizerId);

        modelBuilder.Entity<PerfilAssessoria>().HasIndex(perfil => perfil.ContaId).IsUnique();

        var modelo = modelBuilder.Entity<ModeloEvento>();
        modelo.Property(item => item.Tipo).HasConversion<string>();
        modelo.HasMany(item => item.Servicos).WithOne().HasForeignKey(servico => servico.ModeloId).OnDelete(DeleteBehavior.Cascade);
        modelo.HasMany(item => item.Tarefas).WithOne().HasForeignKey(tarefa => tarefa.ModeloId).OnDelete(DeleteBehavior.Cascade);
        modelo.HasMany(item => item.Vinculos).WithOne().HasForeignKey(vinculo => vinculo.ModeloId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ServicoModelo>().Property(servico => servico.ValorEstimado).HasPrecision(12, 2);
        modelBuilder.Entity<ServicoModelo>().HasMany(servico => servico.Itens).WithOne().HasForeignKey(item => item.ServicoId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ItemServicoModelo>().Property(item => item.Tipo).HasConversion<string>();
        modelBuilder.Entity<VinculoModeloEvento>().Property(vinculo => vinculo.Tipo).HasConversion<string>();

        var evento = modelBuilder.Entity<Evento>();
        evento.Property(item => item.Tipo).HasConversion<string>();
        evento.Property(item => item.Situacao).HasConversion<string>();
        evento.Property(item => item.OrcamentoEstimado).HasPrecision(12, 2);
        evento.OwnsOne(item => item.Cerimonia, local => local.Property<bool>("Persistido").HasDefaultValue(true));
        evento.OwnsOne(item => item.Recepcao, local => local.Property<bool>("Persistido").HasDefaultValue(true));
        evento.HasMany(item => item.Servicos).WithOne().HasForeignKey(servico => servico.EventoId).OnDelete(DeleteBehavior.Cascade);
        evento.HasMany(item => item.LocaisTransfer).WithOne().HasForeignKey(local => local.EventoId).OnDelete(DeleteBehavior.Cascade);
        evento.HasMany(item => item.Tags).WithMany(tag => tag.Eventos);

        var servicoEvento = modelBuilder.Entity<ServicoEvento>();
        servicoEvento.Property(servico => servico.ValorEstimado).HasPrecision(12, 2);
        servicoEvento.Property(servico => servico.ValorContratado).HasPrecision(12, 2);
        servicoEvento.HasMany(servico => servico.Parcelas).WithOne().HasForeignKey(parcela => parcela.ServicoId).OnDelete(DeleteBehavior.Cascade);
        servicoEvento.HasMany(servico => servico.Subitens).WithOne().HasForeignKey(item => item.ServicoId).OnDelete(DeleteBehavior.Cascade);
        servicoEvento.HasMany(servico => servico.Fornecedores).WithOne().HasForeignKey(item => item.ServicoId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SubitemServico>().Property(item => item.ValorUnitario).HasPrecision(12, 2);
        modelBuilder.Entity<FornecedorNoServico>().Property(item => item.Etapa).HasConversion<string>();
        modelBuilder.Entity<FornecedorNoServico>().Property(item => item.ValorOrcamento).HasPrecision(12, 2);
        modelBuilder.Entity<ParcelaServico>().Property(parcela => parcela.Valor).HasPrecision(12, 2);
        modelBuilder.Entity<ParcelaServico>().Property(parcela => parcela.Situacao).HasConversion<string>();

        modelBuilder.Entity<TagConta>().HasIndex(tag => new { tag.ContaId, tag.Nome }).IsUnique();

        var tarefa = modelBuilder.Entity<Tarefa>();
        tarefa.Property(item => item.Situacao).HasConversion<string>();
        tarefa.HasOne<Evento>().WithMany().HasForeignKey(item => item.EventoId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Cliente>().Property(cliente => cliente.Papel).HasConversion<string>();
        modelBuilder.Entity<Cliente>().HasIndex(cliente => cliente.UsuarioId);
        modelBuilder.Entity<VinculoCliente>().HasKey(vinculo => new { vinculo.ClienteId, vinculo.EventoId });
        modelBuilder.Entity<VinculoCliente>().HasOne<Cliente>().WithMany(cliente => cliente.Vinculos).HasForeignKey(vinculo => vinculo.ClienteId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<VinculoCliente>().HasOne<Evento>().WithMany().HasForeignKey(vinculo => vinculo.EventoId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Convite>().HasIndex(convite => convite.Token).IsUnique();
        modelBuilder.Entity<Convite>().HasOne<Evento>().WithMany().HasForeignKey(convite => convite.EventoId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Convite>().HasOne<GrupoConvidados>().WithMany().HasForeignKey(convite => convite.GrupoId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Convite>().HasMany(convite => convite.Convidados).WithOne().HasForeignKey(convidado => convidado.ConviteId).OnDelete(DeleteBehavior.Cascade);

        var convidado = modelBuilder.Entity<Convidado>();
        convidado.Property(item => item.Situacao).HasConversion<string>();
        convidado.Property(item => item.Genero).HasConversion<string>();
        convidado.Property(item => item.Faixa).HasConversion<string>();
        convidado.Property(item => item.Custo).HasConversion<string>();

        var compromisso = modelBuilder.Entity<CompromissoAgenda>();
        compromisso.Property(item => item.Descricao).HasMaxLength(500);
        compromisso.HasIndex(item => item.EventoId).IsUnique().HasFilter("\"EventoId\" IS NOT NULL");
        compromisso.HasOne<Evento>().WithMany().HasForeignKey(item => item.EventoId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ArquivoRegistro>().Property(arquivo => arquivo.CaminhoRelativo).IsRequired();

        MapearBibliotecas(modelBuilder);
        MapearFornecedores(modelBuilder);
        MapearFunil(modelBuilder);
    }

    private static void MapearFunil(ModelBuilder modelBuilder)
    {
        var oportunidade = modelBuilder.Entity<Oportunidade>();
        oportunidade.HasIndex(item => item.ContaId);
        oportunidade.Property(item => item.Estagio).HasConversion<string>();
        oportunidade.Property(item => item.TipoEvento).HasConversion<string>();
        oportunidade.Property(item => item.ValorProposta).HasPrecision(12, 2);
        oportunidade.HasMany(item => item.Contatos).WithOne().HasForeignKey(c => c.OportunidadeId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LeadRsvp>().HasIndex(item => item.ContaId);
        modelBuilder.Entity<LeadRsvp>().Property(item => item.Situacao).HasConversion<string>();
        modelBuilder.Entity<MetricasRsvpFunil>().HasIndex(item => item.ContaId).IsUnique();

        var contrato = modelBuilder.Entity<ContratoFunil>();
        contrato.HasIndex(item => item.ContaId);
        contrato.Property(item => item.Situacao).HasConversion<string>();

        var orcamento = modelBuilder.Entity<OrcamentoFunil>();
        orcamento.HasIndex(item => item.ContaId);
        orcamento.Property(item => item.Situacao).HasConversion<string>();
        orcamento.Property(item => item.Valor).HasPrecision(12, 2);

        var tarefa = modelBuilder.Entity<TarefaFunil>();
        tarefa.HasIndex(item => item.ContaId);
        tarefa.Property(item => item.Tipo).HasConversion<string>();

        modelBuilder.Entity<ModeloServicoFunil>().HasIndex(item => item.ContaId);
        modelBuilder.Entity<ModeloServicoFunil>().Property(item => item.Descricao).HasMaxLength(200);
        modelBuilder.Entity<ModeloServicoFunil>().Property(item => item.Valor).HasPrecision(12, 2);
        modelBuilder.Entity<ModeloOrcamentoFunil>().HasIndex(item => item.ContaId);
        modelBuilder.Entity<ModeloContratoFunil>().HasIndex(item => item.ContaId);
    }

    private static void MapearBibliotecas(ModelBuilder modelBuilder)
    {
        var briefing = modelBuilder.Entity<ModeloBriefing>();
        briefing.HasIndex(item => item.ContaId);
        briefing.HasMany(item => item.Secoes).WithOne().HasForeignKey(secao => secao.ModeloId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SecaoBriefing>().HasMany(secao => secao.Perguntas).WithOne().HasForeignKey(pergunta => pergunta.SecaoId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PerguntaBriefing>().Property(pergunta => pergunta.Tipo).HasConversion<string>();

        var roteiro = modelBuilder.Entity<ModeloRoteiro>();
        roteiro.HasIndex(item => item.ContaId);
        roteiro.HasMany(item => item.Secoes).WithOne().HasForeignKey(secao => secao.ModeloId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<SecaoRoteiro>().HasMany(secao => secao.Itens).WithOne().HasForeignKey(item => item.SecaoId).OnDelete(DeleteBehavior.Cascade);

        var cronograma = modelBuilder.Entity<ModeloCronograma>();
        cronograma.HasIndex(item => item.ContaId);
        cronograma.HasMany(item => item.Itens).WithOne().HasForeignKey(item => item.ModeloId).OnDelete(DeleteBehavior.Cascade);

        var inspiracao = modelBuilder.Entity<ModeloInspiracao>();
        inspiracao.HasIndex(item => item.ContaId);
        inspiracao.HasMany(item => item.Itens).WithOne().HasForeignKey(item => item.GaleriaId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ModeloMensagem>().HasIndex(item => item.ContaId);
    }

    private static void MapearFornecedores(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServicoConta>().HasIndex(servico => new { servico.ContaId, servico.Nome }).IsUnique();
        modelBuilder.Entity<ServicoConta>().Property(servico => servico.Nome).HasMaxLength(200);

        var fornecedor = modelBuilder.Entity<Fornecedor>();
        fornecedor.HasIndex(item => item.ContaId);
        fornecedor.Property(item => item.PrecoDe).HasPrecision(12, 2);
        fornecedor.Property(item => item.PrecoAte).HasPrecision(12, 2);
        fornecedor.HasMany(item => item.Telefones).WithOne().HasForeignKey(telefone => telefone.FornecedorId).OnDelete(DeleteBehavior.Cascade);
        fornecedor.HasMany(item => item.Emails).WithOne().HasForeignKey(email => email.FornecedorId).OnDelete(DeleteBehavior.Cascade);
        fornecedor.HasMany(item => item.Orcamentos).WithOne().HasForeignKey(orcamento => orcamento.FornecedorId).OnDelete(DeleteBehavior.Cascade);
        fornecedor.HasMany(item => item.Servicos).WithOne().HasForeignKey(ligacao => ligacao.FornecedorId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FornecedorServico>().HasKey(ligacao => new { ligacao.FornecedorId, ligacao.ServicoContaId });
        modelBuilder.Entity<FornecedorServico>().HasOne<ServicoConta>().WithMany().HasForeignKey(ligacao => ligacao.ServicoContaId).OnDelete(DeleteBehavior.Restrict);
    }
}
