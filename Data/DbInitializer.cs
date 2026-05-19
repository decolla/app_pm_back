using app_pm_back.Models;
using app_pm_back.Models.Domain;
using app_pm_back.Services;

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
        ClassificationService<NotaManutencaoModel> classificationService = new(app.Services.GetRequiredService<AppDbContext>());
        
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AppDbContext>();
            await context.Database.EnsureCreatedAsync();
            if(context.PM_Regras.Any() == false)            {
                Console.WriteLine($"A base de dados já contém {context.PM_Regras.Count()} regra(s) cadastrada(s).");
            }
            else // adiciona as regras padrões ao banco de dados
            {
                try
                {
                    Console.WriteLine("Nenhuma regra encontrada na base de dados ao iniciar.\n" +
                                      "Inicializando regras padrões.");
                    
                    var regras = new List<RuleClassificationModel>();

                    RuleClassificationModel regraZM01 = new RuleClassificationModel()
                    {
                        CampoAlvo = "TipoNota",
                        PalavraChave = "MG",
                        ClassificacaoAplicada = "PREVENTIVA",
                        OrdemPrioridade = 1
                    };
                    
                    // ADICIONAR MAIS DPS 
                    regras.Add(regraZM01);
                    
                    // adiciona as regras ao banco de dados
                    context.PM_Regras.AddRange(regras);
                    await context.SaveChangesAsync();
                }                
                catch (Exception e)
                {
                    Console.WriteLine($"Erro ao inicializar regras padrões: {e.Message}");
                }
            }
        }
    }
}