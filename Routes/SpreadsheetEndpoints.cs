using app_pm_back.Models;
using app_pm_back.Services;

namespace app_pm_back.Routes;

public static class SpreadsheetEndpoints
{
    public static void MapSpreadsheetEndpoints(this IEndpointRouteBuilder app)
    {
        // agrupa as rotas de planilha 
        var group = app.MapGroup("/planilhas");

        // o endpoint recebe o arquivo, o serviço de leitura e o serviço de banco de dados
        group.MapPost("/upload", async (IFormFile file, SpreadsheetService spreadsheetService, NotasService notasService) => 
        {
            try
            {
                // 1 = validação básica do arquivo
                if (file == null || file.Length == 0)
                    return Results.BadRequest(new { message = "Arquivo inválido ou vazio." });

                // 2 = lê a planilha usando o serviço adaptado e retorna uma lista de NotaModel
                List<NotaPetroModel> notasLidas = await spreadsheetService.ReadNotasSpreadSheetAsync(file);

                // verifica se existe alguma nota na planilha 
                if (notasLidas == null || !notasLidas.Any())
                    return Results.BadRequest(new { message = "Nenhuma nota válida foi encontrada na planilha." });

                // 3 = insere as notas no banco de dados usando o seu NotasService
                int inseridas = 0;
                foreach (var nota in notasLidas)
                {
                    await notasService.CreateNotaAsync(nota);
                    inseridas++;
                }

                // 4 = retorna sucesso
                return Results.Ok(new { message = $"{inseridas} notas processadas e cadastradas com sucesso!" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = $"Erro ao processar planilha: {ex.Message}" });
            }
        })
        .DisableAntiforgery();
    }
}