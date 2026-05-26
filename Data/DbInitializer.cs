using app_pm_back.Models.Domain;

namespace app_pm_back.Data;

/// <summary>
/// Classe para inicializar o banco de dados.
/// </summary>
public class DbInitializer
{
    /// <summary>
    /// Inicializa o banco de dados.
    /// </summary>
    /// <param name="app"></param>
    public static async Task SeedAsync (WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AppDbContext>();
            await context.Database.EnsureCreatedAsync();
        }
    }
}