using app_pm_back.Models;
using app_pm_back.Models.CongasModel;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Data;

/// <summary>
/// MAPEAMENTO EF CORE PARA POSTGRESQL
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Opções do banco de dados
    /// </summary>
    /// <param name="options"></param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Tabelas de Nota Petro
    /// </summary>
    public DbSet<NotaPetroModel> NotasPetro { get; set; }
    
    /// <summary>
    /// Tabela de Ordem Petro
    /// </summary>
    public DbSet<OrdemPetroModel> OrdensPetro { get; set; }
    
    /// <summary>
    /// Tabela de Nota Congas
    /// </summary>
    public DbSet<NoteCongasModel> NotasCongas { get; set; }
    
    /// <summary>
    /// Tabela de Ordem Congas
    /// </summary>
    public DbSet<OrderCongasModel> OrdensCongas { get; set; }
    
    /// <summary>
    /// Tabela de Anexos
    /// </summary>
    public DbSet<AnnexModel> Anexos { get; set; }
    
    /// <summary>
    /// Tabela de Regras para classificação
    /// </summary>
    public DbSet<RuleClassificationModel> Regras { get; set; }
    
    /// <summary>
    /// Tabela de Itens de Nota Congas
    /// </summary>
    public DbSet<ItemNotaCongasModel> ItemNotaCongas { get; set; }
}