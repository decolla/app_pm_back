using System.Text.Json;
using app_pm_back.Models.Domain;
using app_pm_back.DTO;
using app_pm_back.Interfaces;
using app_pm_back.Models.Domain.Diretorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace app_pm_back.Data;

/// <summary>
/// Mapeamento EF Core dinâmico para PostgreSQL ou SQL Server.
/// </summary>
public class AppDbContext : DbContext
{
    private readonly IUserContext _userContext;

    public AppDbContext(DbContextOptions options) : this(options, new DesignTimeUserContext())
    {
    }

    public AppDbContext(DbContextOptions options, IUserContext userContext) : base(options)
    {
        _userContext = userContext;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // configura as chaves primárias
        modelBuilder.Entity<TipoManutencaoModel>().HasKey(e => e.IdTipoManutencao);
        modelBuilder.Entity<TipoEstrategiaModel>().HasKey(e => e.IdTipoEstrategia);
        modelBuilder.Entity<TipoCepModel>().HasKey(e => e.IdTipoCep);
        modelBuilder.Entity<OperacaoModel>().HasKey(e => e.IdOperacao);
        modelBuilder.Entity<TipoPlanoModel>().HasKey(e => e.IdTipoPlano);
        modelBuilder.Entity<EstrategiaModel>().HasKey(e => e.IdEstrategia);
        modelBuilder.Entity<PlanoModel>().HasKey(e => e.IdPlano);
        modelBuilder.Entity<AlmoxarifadoModel>().HasKey(e => e.IdAlmoxarifado);
        modelBuilder.Entity<UsuarioModel>().HasKey(e => e.IdUsuario);
        modelBuilder.Entity<GpmModel>().HasKey(e => e.IdGPM);
        modelBuilder.Entity<CpmModel>().HasKey(e => e.IdCPM);
        modelBuilder.Entity<CtmModel>().HasKey(e => e.IdCTM);
        modelBuilder.Entity<FerramentaModel>().HasKey(e => e.IdFerramenta);
        modelBuilder.Entity<ImobilizadoModel>().HasKey(e => e.IdImobilizado);
        modelBuilder.Entity<NotaModel>().HasKey(e => e.IdNota);
        modelBuilder.Entity<VariavelNotaModel>().HasKey(e => e.IdVariavel);
        modelBuilder.Entity<ItemNotaModel>().HasKey(e => e.IdItemNota);
        modelBuilder.Entity<AuditoriaModel>().HasKey(e => e.IdAuditoria);
        modelBuilder.Entity<AnexoNotaModel>().HasKey(e => e.IdAnexoNota);
        modelBuilder.Entity<OrdemModel>().HasKey(e => e.IdOrdem);
        modelBuilder.Entity<AnexoOrdemModel>().HasKey(e => e.IdAnexoOrdem);
        modelBuilder.Entity<DadosRenovacaoModel>().HasKey(e => e.IdDados);
        modelBuilder.Entity<ConfiguracaoGlobalModel>().HasKey(c => c.Id);
        modelBuilder.Entity<RegraClassificacaoModel>().HasKey(e => e.IdRegra);

        // resolve ambiguidade de relacionamento
        modelBuilder.Entity<NotaModel>()
            .HasOne(n => n.Usuario)
            .WithMany() 
            .HasForeignKey(n => n.IdUsuario)
            .OnDelete(DeleteBehavior.SetNull); 

        // 1. Ferramentas/Operacoes
        modelBuilder.Entity<OperacaoModel>()
            .HasMany(o => o.Ferramentas)
            .WithMany(f => f.Operacoes)
            .UsingEntity<Dictionary<string, object>>(
                "FerramentasOperacoes",
                j => j.HasOne<FerramentaModel>().WithMany().HasForeignKey("IdFerramenta")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<OperacaoModel>().WithMany().HasForeignKey("IdOperacao")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.Property<long>("IdFerramenta");
                    j.Property<long>("IdOperacao");

                    j.HasKey("IdFerramenta", "IdOperacao");
                    j.ToTable("PM_FerramentasOperacoes");
                });

        // 2. Almoxarifado/Usuario
        modelBuilder.Entity<AlmoxarifadoModel>()
            .HasMany(a => a.Usuarios)
            .WithMany(u => u.Almoxarifados)
            .UsingEntity<Dictionary<string, object>>(
                "AlmoxarifadoUsuario",
                j => j.HasOne<UsuarioModel>().WithMany().HasForeignKey("IdUsuario").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<AlmoxarifadoModel>().WithMany().HasForeignKey("IdAlmoxarifado")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("IdAlmoxarifado", "IdUsuario");
                    j.ToTable("PM_AlmoxarifadoUsuario");
                });

        // 3. GPM/LI (Apesar do nome referenciar LI, os campos internos apontam Usuário)
        modelBuilder.Entity<GpmModel>()
            .HasMany<UsuarioModel>()
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "GpmLi",
                j => j.HasOne<UsuarioModel>().WithMany().HasForeignKey("IdUsuario").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<GpmModel>().WithMany().HasForeignKey("IdGPM").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("IdGPM", "IdUsuario");
                    j.ToTable("PM_GpmLi");
                });

        // 4. GPM/Usuario
        modelBuilder.Entity<GpmModel>()
            .HasMany(g => g.Usuarios)
            .WithMany(u => u.Gpms)
            .UsingEntity<Dictionary<string, object>>(
                "GpmUsuario",
                j => j.HasOne<UsuarioModel>().WithMany().HasForeignKey("IdUsuario").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<GpmModel>().WithMany().HasForeignKey("IdGPM").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("IdGPM", "IdUsuario");
                    j.ToTable("PM_GpmUsuario");
                });

        // 5. CTM/Usuario
        modelBuilder.Entity<CtmModel>()
            .HasMany(c => c.Usuarios)
            .WithMany(u => u.Ctms)
            .UsingEntity<Dictionary<string, object>>(
                "CtmUsuario",
                j => j.HasOne<UsuarioModel>().WithMany().HasForeignKey("IdUsuario").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<CtmModel>().WithMany().HasForeignKey("IdCTM").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("IdCTM", "IdUsuario");
                    j.ToTable("PM_CtmUsuario");
                });
        
        // 6. Ordem/Usuario
        modelBuilder.Entity<OrdemModel>()
            .HasMany(o => o.Usuarios)
            .WithMany(u => u.Ordens)
            .UsingEntity<Dictionary<string, object>>(
                "OrdemUsuario",
                j => j.HasOne<UsuarioModel>().WithMany().HasForeignKey("IdUsuario").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<OrdemModel>().WithMany().HasForeignKey("IdOrdem").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("IdOrdem", "IdUsuario");
                    j.ToTable("PM_OrdemUsuario");
                });

        // 7. Nota/Usuario
        modelBuilder.Entity<NotaModel>()
            .HasMany(n => n.Usuarios)
            .WithMany(u => u.Notas)
            .UsingEntity<Dictionary<string, object>>(
                "NotaUsuario",
                j => j.HasOne<UsuarioModel>().WithMany().HasForeignKey("IdUsuario").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<NotaModel>().WithMany().HasForeignKey("IdNota").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("IdNota", "IdUsuario");
                    j.ToTable("PM_NotaUsuario");
                });

        // adiciona um valor padrão inicial caso a tabela esteja vazia nas migrations
        modelBuilder.Entity<ConfiguracaoGlobalModel>().HasData(new ConfiguracaoGlobalModel
        {
            Id = 1,
            FichasConfiabilidade = OrigemFichasConfiabilidade.ConsiderarTambemAlgoritmo,
            ClassificacaoAutomatica = ModoClassificacaoAutomatica.NaoClassificar,
            CriterioDataRenovacao = CriterioDataRenovacao.ConsiderarTodasClassificacoes,
            CriterioConfiabilidadeRenovacao = CriterioConfiabilidadeRenovacao.ConsiderarTambemAlgoritmo,
            DataModificacao = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        // FILTROS DE ESCOPO
        //
        // modelBuilder.Entity<UsuarioModel>().HasQueryFilter(e =>
        //     e.Escopos.Any(escopoBanco => _userContext.GetEscoposDoUsuario().Contains(escopoBanco)));
        // modelBuilder.Entity<NotaModel>().HasQueryFilter(e =>
        //     e.Escopo != null && _userContext.GetEscoposDoUsuario().Contains(e.Escopo.Value));
        // modelBuilder.Entity<OrdemModel>().HasQueryFilter(e =>
        //     e.Escopo != null && _userContext.GetEscoposDoUsuario().Contains(e.Escopo.Value));
        // modelBuilder.Entity<CpmModel>().HasQueryFilter(e =>
        //     e.IdEscopo != null && _userContext.GetEscoposDoUsuario().Contains(e.IdEscopo));
        // modelBuilder.Entity<AlmoxarifadoModel>().HasQueryFilter(e =>
        //     e.IdEscopo != null && _userContext.GetEscoposDoUsuario().Contains(e.IdEscopo));
        // modelBuilder.Entity<FerramentaModel>().HasQueryFilter(e =>
        //     e.IdEscopo != null && _userContext.GetEscoposDoUsuario().Contains(e.IdEscopo));

        // relacionamento dos diretorios 
        modelBuilder.Entity<DiretorioModel>()
            .HasOne(d => d.DiretorioPai)
            .WithMany(d => d.Subdiretorios)
            .HasForeignKey(d => d.IdDiretorioPai)
            .OnDelete(DeleteBehavior.Restrict);

        // chaves compostas ára ligação diretorio e nota
        modelBuilder.Entity<DiretorioNotaModel>()
            .HasKey(dn => new { dn.IdDiretorio, dn.IdNota });

        modelBuilder.Entity<DiretorioOrdemModel>()
            .HasKey(do_ => new { do_.IdDiretorio, do_.IdOrdem });

        
        // relacionamento entre diretorio e nota
        modelBuilder.Entity<DiretorioNotaModel>()
            .HasOne(dn => dn.Diretorio)
            .WithMany(d => d.NotasVinculadas)
            .HasForeignKey(dn => dn.IdDiretorio);

        modelBuilder.Entity<DiretorioNotaModel>()
            .HasOne(dn => dn.Nota)
            .WithMany(n => n.DiretoriosVinculados)
            .HasForeignKey(dn => dn.IdNota);

        // relacionamento entre Ordem e Nota com deleção em cascata
        modelBuilder.Entity<OrdemModel>()
            .HasOne(o => o.Nota)
            .WithMany(n => n.Ordens)
            .HasForeignKey(o => o.IdNota)
            .OnDelete(DeleteBehavior.Cascade);
    }

    // ================================ CRIAÇÃO DAS TABELAS ====================================

    public DbSet<TipoManutencaoModel> PM_TiposManutencao { get; set; }
    public DbSet<TipoEstrategiaModel> PM_TiposEstrategia { get; set; }
    public DbSet<TipoCepModel> PM_TiposCep { get; set; }
    public DbSet<OperacaoModel> PM_Operacoes { get; set; }
    public DbSet<ConfiguracaoGlobalModel> PM_ConfiguracoesGlobais { get; set; }
    public DbSet<TipoPlanoModel> PM_TiposPlano { get; set; }
    public DbSet<EstrategiaModel> PM_Estrategias { get; set; }
    public DbSet<PlanoModel> PM_Planos { get; set; }
    public DbSet<AlmoxarifadoModel> PM_Almoxarifados { get; set; }
    public DbSet<UsuarioModel> PM_Usuarios { get; set; }
    public DbSet<GpmModel> PM_Gpms { get; set; }
    public DbSet<CpmModel> PM_Cpms { get; set; }
    public DbSet<CtmModel> PM_Ctms { get; set; }
    public DbSet<FerramentaModel> PM_Ferramentas { get; set; }
    public DbSet<ImobilizadoModel> PM_Imobilizados { get; set; }
    public DbSet<NotaModel> PM_Notas { get; set; }
    public DbSet<VariavelNotaModel> PM_VariaveisNota { get; set; }
    public DbSet<ItemNotaModel> PM_ItensNota { get; set; }
    public DbSet<AnexoNotaModel> PM_AnexosNota { get; set; }
    public DbSet<OrdemModel> PM_Ordens { get; set; }
    public DbSet<AnexoOrdemModel> PM_AnexosOrdem { get; set; }
    public DbSet<DadosRenovacaoModel> PM_DadosRenovacao { get; set; }
    public DbSet<ReliabilityModel> PM_Reliability { get; set; }
    public DbSet<AuditoriaModel> PM_Auditorias { get; set; }
    public DbSet<RegraClassificacaoModel> PM_RuleClassification { get; set; }
    public DbSet<DiretorioModel> PM_Diretorios { get; set; }
}

public class SqlServerDbContext : AppDbContext
{
    public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options, IUserContext userContext) : base(options,
        userContext)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // aplica as configurações globais primeiro
        base.OnModelCreating(modelBuilder);

        // aplica as exclusividades do SQL Server
        modelBuilder.HasDefaultSchema("dbo");

        // converter para JSON as ações de falha
        var converter = new ValueConverter<List<AcoesFalha>?, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<List<AcoesFalha>>(v, (JsonSerializerOptions?)null) ?? new List<AcoesFalha>()
        );

        var longListConverter = new ValueConverter<List<long>?, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<List<long>>(v, (JsonSerializerOptions?)null) ?? new List<long>()
        );

        modelBuilder.Entity<TipoCepModel>()
            .Property(e => e.AcoesFalha)
            .HasConversion(converter)
            .HasColumnType("nvarchar(max)");

        modelBuilder.Entity<ReliabilityModel>()
            .Property(e => e.IdsNotas)
            .HasConversion(longListConverter)
            .HasColumnType("nvarchar(max)");

        modelBuilder.Entity<ReliabilityModel>()
            .Property(e => e.IdsOrdens)
            .HasConversion(longListConverter)
            .HasColumnType("nvarchar(max)");

        modelBuilder.Entity<UsuarioModel>()
            .Property(e => e.Escopos)
            .HasConversion(longListConverter)
            .HasColumnType("nvarchar(max)");
    }
}

public class PostgresDbContext : AppDbContext
{
    public PostgresDbContext(DbContextOptions<PostgresDbContext> options, IUserContext userContext) : base(options,
        userContext)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // aplica as configurações globais primeiro
        base.OnModelCreating(modelBuilder);
    }
}