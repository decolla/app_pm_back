using app_pm_back.Models;
using app_pm_back.Models.CongasModel;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Data;

/// <summary>
/// Mapeamento EF Core para PostgreSQL.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Opcoes do banco de dados.
    /// </summary>
    /// <param name="options">Opcoes do contexto.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configura entidades, chaves e relacionamentos.
    /// </summary>
    /// <param name="modelBuilder">Builder de mapeamento do modelo.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relacionamentos legados COMGAS.
        modelBuilder.Entity<MaintancePlanComgasModel>()
            .HasOne(p => p.Estrategia)
            .WithMany(e => e.Planos)
            .HasForeignKey(p => p.IdEstrategia)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ItemManutencaoComgasModel>()
            .HasOne(i => i.PlanoRelacionado)
            .WithMany(p => p.ItensManutencao)
            .HasForeignKey(i => i.PlanoId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<CicloComgasModel>()
            .HasOne(c => c.Estrategia)
            .WithMany(e => e.Ciclos)
            .HasForeignKey(c => c.IdEstrategia)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<NoteCongasModel>()
            .HasOne<EquipmentComgasModel>()
            .WithMany(e => e.Notas)
            .HasForeignKey(n => n.Equipamento)
            .HasPrincipalKey(e => e.IdEquipamento)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ItemNotaCongasModel>()
            .HasOne(i => i.NoteCongas)
            .WithMany(n => n.Itens)
            .HasForeignKey(i => i.NoteCongasIdNota)
            .OnDelete(DeleteBehavior.Cascade);

        // Dominio normalizado para manutencao.
        modelBuilder.Entity<UsuarioSapModel>()
            .HasIndex(u => new { u.Nome, u.Mandante })
            .IsUnique();

        modelBuilder.Entity<FabricanteModel>()
            .HasIndex(f => f.Nome)
            .IsUnique();

        modelBuilder.Entity<LocalizacaoModel>()
            .HasIndex(l => l.CodigoLocalizacao)
            .IsUnique();

        modelBuilder.Entity<GrupoPlanejamentoManutencaoModel>()
            .HasIndex(g => g.Codigo)
            .IsUnique();

        modelBuilder.Entity<CentroTrabalhoModel>()
            .HasIndex(c => c.Codigo)
            .IsUnique();

        modelBuilder.Entity<PlanoManutencaoModel>()
            .HasOne(p => p.Estrategia)
            .WithMany(e => e.Planos)
            .HasForeignKey(p => p.EstrategiaId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<CicloManutencaoModel>()
            .HasOne(c => c.PlanoManutencao)
            .WithMany(p => p.Ciclos)
            .HasForeignKey(c => c.PlanoManutencaoCodigo)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ItemPlanoManutencaoModel>()
            .HasOne(i => i.PlanoManutencao)
            .WithMany(p => p.Itens)
            .HasForeignKey(i => i.PlanoManutencaoCodigo)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LocalInstalacaoModel>()
            .HasOne(l => l.Fabricante)
            .WithMany(f => f.LocaisInstalacao)
            .HasForeignKey(l => l.FabricanteId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<LocalInstalacaoModel>()
            .HasOne(l => l.Localizacao)
            .WithMany(loc => loc.LocaisInstalacao)
            .HasForeignKey(l => l.LocalizacaoId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<LocalInstalacaoModel>()
            .HasOne(l => l.Endereco)
            .WithMany()
            .HasForeignKey(l => l.EnderecoId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<LocalInstalacaoModel>()
            .HasOne(l => l.Imobilizado)
            .WithMany(i => i.LocaisInstalacao)
            .HasForeignKey(l => l.ImobilizadoTag)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<LocalInstalacaoModel>()
            .HasOne(l => l.Gpm)
            .WithMany(g => g.LocaisInstalacao)
            .HasForeignKey(l => l.GpmId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<LocalInstalacaoModel>()
            .HasOne(l => l.CentroTrabalho)
            .WithMany(c => c.LocaisInstalacao)
            .HasForeignKey(l => l.CentroTrabalhoId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<EquipamentoAtivoModel>()
            .HasOne(e => e.LocalInstalacao)
            .WithMany(l => l.Equipamentos)
            .HasForeignKey(e => e.LocalInstalacaoCodigo)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<EquipamentoAtivoModel>()
            .HasOne(e => e.Fabricante)
            .WithMany(f => f.Equipamentos)
            .HasForeignKey(e => e.FabricanteId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<EquipamentoAtivoModel>()
            .HasOne(e => e.Localizacao)
            .WithMany(l => l.Equipamentos)
            .HasForeignKey(e => e.LocalizacaoId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<EquipamentoAtivoModel>()
            .HasOne(e => e.Imobilizado)
            .WithMany(i => i.Equipamentos)
            .HasForeignKey(e => e.ImobilizadoTag)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<EquipamentoAtivoModel>()
            .HasOne(e => e.Gpm)
            .WithMany(g => g.Equipamentos)
            .HasForeignKey(e => e.GpmId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<EquipamentoAtivoModel>()
            .HasOne(e => e.CentroTrabalho)
            .WithMany(c => c.Equipamentos)
            .HasForeignKey(e => e.CentroTrabalhoId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ItemPlanoManutencaoModel>()
            .HasOne(i => i.Equipamento)
            .WithMany(e => e.ItensPlano)
            .HasForeignKey(i => i.EquipamentoTag)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ItemPlanoManutencaoModel>()
            .HasOne(i => i.LocalInstalacao)
            .WithMany(l => l.ItensPlano)
            .HasForeignKey(i => i.LocalInstalacaoCodigo)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<NotaManutencaoModel>()
            .HasOne(n => n.Equipamento)
            .WithMany(e => e.Notas)
            .HasForeignKey(n => n.EquipamentoTag)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<NotaManutencaoModel>()
            .HasOne(n => n.LocalInstalacao)
            .WithMany(l => l.Notas)
            .HasForeignKey(n => n.LocalInstalacaoCodigo)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<NotaManutencaoModel>()
            .HasOne(n => n.CriadoPorUsuario)
            .WithMany(u => u.NotasCriadas)
            .HasForeignKey(n => n.CriadoPorUsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ItemNotaManutencaoModel>()
            .HasOne(i => i.Nota)
            .WithMany(n => n.Itens)
            .HasForeignKey(i => i.NotaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MedidaNotaModel>()
            .HasOne(i => i.Nota)
            .WithMany(n => n.Medidas)
            .HasForeignKey(i => i.NotaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AtividadeNotaModel>()
            .HasOne(i => i.Nota)
            .WithMany(n => n.Atividades)
            .HasForeignKey(i => i.NotaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrdemManutencaoModel>()
            .HasOne(o => o.NotaRelacionada)
            .WithMany(n => n.Ordens)
            .HasForeignKey(o => o.NotaRelacionadaId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<OrdemManutencaoModel>()
            .HasOne(o => o.PlanoManutencao)
            .WithMany(p => p.Ordens)
            .HasForeignKey(o => o.PlanoManutencaoCodigo)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<OrdemManutencaoModel>()
            .HasOne(o => o.CriadaPorUsuario)
            .WithMany(u => u.OrdensCriadas)
            .HasForeignKey(o => o.CriadaPorUsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<OrdemManutencaoModel>()
            .HasOne(o => o.ModificadaPorUsuario)
            .WithMany(u => u.OrdensModificadas)
            .HasForeignKey(o => o.ModificadaPorUsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<OperacaoOrdemModel>()
            .HasOne(o => o.Ordem)
            .WithMany(o => o.Operacoes)
            .HasForeignKey(o => o.OrdemId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    /// <summary>
    /// Tabela de notas Petro.
    /// </summary>
    public DbSet<NotaPetroModel> NotasPetro { get; set; }

    /// <summary>
    /// Tabela de ordens Petro.
    /// </summary>
    public DbSet<OrdemPetroModel> OrdensPetro { get; set; }

    /// <summary>
    /// Tabela de notas Congas.
    /// </summary>
    public DbSet<NoteCongasModel> NotasCongas { get; set; }

    /// <summary>
    /// Tabela de ordens Congas.
    /// </summary>
    public DbSet<OrderCongasModel> OrdensCongas { get; set; }

    /// <summary>
    /// Tabela de anexos.
    /// </summary>
    public DbSet<AnnexModel> Anexos { get; set; }

    /// <summary>
    /// Tabela de regras para classificacao.
    /// </summary>
    public DbSet<RuleClassificationModel> Regras { get; set; }

    /// <summary>
    /// Tabela de itens de nota Congas.
    /// </summary>
    public DbSet<ItemNotaCongasModel> ItemNotaCongas { get; set; }

    /// <summary>
    /// Tabela de acoes de nota Congas.
    /// </summary>
    public DbSet<AcaoNotaCongasModel> AcaoNotaCongas { get; set; }

    /// <summary>
    /// Tabela de equipamentos legados da Congas.
    /// </summary>
    public DbSet<EquipmentComgasModel> Equipamentos { get; set; }

    /// <summary>
    /// Tabela de planos de manutencao legados da Congas.
    /// </summary>
    public DbSet<MaintancePlanComgasModel> PlanosManutencao { get; set; }

    /// <summary>
    /// Tabela de usuarios SAP.
    /// </summary>
    public DbSet<UsuarioSapModel> UsuariosSap { get; set; }

    /// <summary>
    /// Tabela de fabricantes.
    /// </summary>
    public DbSet<FabricanteModel> Fabricantes { get; set; }

    /// <summary>
    /// Tabela de enderecos.
    /// </summary>
    public DbSet<EnderecoModel> Enderecos { get; set; }

    /// <summary>
    /// Tabela de localizacoes.
    /// </summary>
    public DbSet<LocalizacaoModel> Localizacoes { get; set; }

    /// <summary>
    /// Tabela de imobilizados.
    /// </summary>
    public DbSet<ImobilizadoModel> Imobilizados { get; set; }

    /// <summary>
    /// Tabela de grupos de planejamento de manutencao.
    /// </summary>
    public DbSet<GrupoPlanejamentoManutencaoModel> GruposPlanejamentoManutencao { get; set; }

    /// <summary>
    /// Tabela de centros de trabalho.
    /// </summary>
    public DbSet<CentroTrabalhoModel> CentrosTrabalho { get; set; }

    /// <summary>
    /// Tabela de locais de instalacao.
    /// </summary>
    public DbSet<LocalInstalacaoModel> LocaisInstalacao { get; set; }

    /// <summary>
    /// Tabela de equipamentos normalizados.
    /// </summary>
    public DbSet<EquipamentoAtivoModel> EquipamentosAtivos { get; set; }

    /// <summary>
    /// Tabela de estrategias de manutencao.
    /// </summary>
    public DbSet<EstrategiaManutencaoModel> EstrategiasManutencao { get; set; }

    /// <summary>
    /// Tabela de planos de manutencao normalizados.
    /// </summary>
    public DbSet<PlanoManutencaoModel> PlanosManutencaoNormalizados { get; set; }

    /// <summary>
    /// Tabela de ciclos de manutencao.
    /// </summary>
    public DbSet<CicloManutencaoModel> CiclosManutencao { get; set; }

    /// <summary>
    /// Tabela de itens de plano de manutencao.
    /// </summary>
    public DbSet<ItemPlanoManutencaoModel> ItensPlanoManutencao { get; set; }

    /// <summary>
    /// Tabela de notas de manutencao normalizadas.
    /// </summary>
    public DbSet<NotaManutencaoModel> NotasManutencao { get; set; }

    /// <summary>
    /// Tabela de itens de notas de manutencao.
    /// </summary>
    public DbSet<ItemNotaManutencaoModel> ItensNotaManutencao { get; set; }

    /// <summary>
    /// Tabela de medidas de nota.
    /// </summary>
    public DbSet<MedidaNotaModel> MedidasNota { get; set; }

    /// <summary>
    /// Tabela de atividades de nota.
    /// </summary>
    public DbSet<AtividadeNotaModel> AtividadesNota { get; set; }

    /// <summary>
    /// Tabela de ordens de manutencao normalizadas.
    /// </summary>
    public DbSet<OrdemManutencaoModel> OrdensManutencao { get; set; }

    /// <summary>
    /// Tabela de operacoes de ordens.
    /// </summary>
    public DbSet<OperacaoOrdemModel> OperacoesOrdem { get; set; }
}



