using app_pm_back.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace app_pm_back.Data;


/// <summary>
/// Contexto falso para o terminal
/// </summary>
public class DesignTimeUserContext : IUserContext
{
    public List<long> GetEscoposDoUsuario() => new List<long>();
    public int? GetUsuarioIdAtual() => null;
    public string? GetUsuarioIdentificador() => null; // TODO: identificação futura de requisição de usuários  
}

/// <summary>
/// Fábrica de contexto para o terminal
/// </summary>
public class SqlServerDbContextFactory : IDesignTimeDbContextFactory<SqlServerDbContext>
{
    public SqlServerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SqlServerDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=pm_dummy;User Id=sa;Password=senha;TrustServerCertificate=True;");
        optionsBuilder.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        
        // passa o contexto falso para o terminal
        return new SqlServerDbContext(optionsBuilder.Options, new DesignTimeUserContext()); 
    }
}

/// <summary>
/// Fábrica de contexto para o terminal
/// </summary>
public class PostgresDbContextFactory : IDesignTimeDbContextFactory<PostgresDbContext>
{
    public PostgresDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION")
            ?? "Host=localhost;Database=app_pm;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<PostgresDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));

        // passa o contexto falso para o terminal
        return new PostgresDbContext(optionsBuilder.Options, new DesignTimeUserContext());
    }
}