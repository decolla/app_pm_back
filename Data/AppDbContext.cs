using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Data;

/// <summary>
/// Mapeamento EF Core para PostgreSQL.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");   
        base.OnModelCreating(modelBuilder);

        // Configure Primary Keys and Auto-increment
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
        modelBuilder.Entity<CtmModel>().HasKey(e => e.IdGPM);
        modelBuilder.Entity<FerramentaModel>().HasKey(e => e.IdFerramenta);
        modelBuilder.Entity<ImobilizadoModel>().HasKey(e => e.IdImobilizado);
        modelBuilder.Entity<NotaModel>().HasKey(e => e.IdNota);
        modelBuilder.Entity<VariavelNotaModel>().HasKey(e => e.IdVariavel);
        modelBuilder.Entity<ItemNotaModel>().HasKey(e => e.IdItemNota);
        modelBuilder.Entity<AnexoNotaModel>().HasKey(e => e.IdAnexoNota);
        modelBuilder.Entity<OrdemModel>().HasKey(e => e.IdOrdem);
        modelBuilder.Entity<AnexoOrdemModel>().HasKey(e => e.IdAnexoOrdem);
        modelBuilder.Entity<DadosRenovacaoModel>().HasKey(e => e.IdDados);

        // 1-to-1: CTM and GPM share same primary key or relate 1-to-1
        modelBuilder.Entity<CtmModel>()
            .HasOne<GpmModel>()
            .WithOne()
            .HasForeignKey<CtmModel>(c => c.IdGPM)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure many-to-many associations
        // 1. Ferramentas/Operacoes
        modelBuilder.Entity<OperacaoModel>()
            .HasMany(o => o.Ferramentas)
            .WithMany(f => f.Operacoes)
            .UsingEntity<Dictionary<string, object>>(
                "FerramentasOperacoes",
                j => j.HasOne<FerramentaModel>().WithMany().HasForeignKey("IdFerramenta").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<OperacaoModel>().WithMany().HasForeignKey("IdOperacao").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
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
                j => j.HasOne<AlmoxarifadoModel>().WithMany().HasForeignKey("IdAlmoxarifado").OnDelete(DeleteBehavior.Cascade),
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
    }

    public DbSet<TipoManutencaoModel> PM_TiposManutencao { get; set; }
    public DbSet<TipoEstrategiaModel> PM_TiposEstrategia { get; set; }
    public DbSet<TipoCepModel> PM_TiposCep { get; set; }
    public DbSet<OperacaoModel> PM_Operacoes { get; set; }
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
}
