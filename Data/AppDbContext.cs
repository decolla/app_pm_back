using app_pm_back.Models;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Data;

// MAPEAMENTO EF CORE PARA POSTGRESQL
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<NotaPetroModel> NotasPetro { get; set; }
    
    public DbSet<OrdemPetroModel> OrdensPetro { get; set; }
}