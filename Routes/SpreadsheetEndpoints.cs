using app_pm_back.Models;
using app_pm_back.Services;
using app_pm_back.Services.CongasServices;

namespace app_pm_back.Routes;

public static class SpreadsheetEndpoints
{
    public static void MapSpreadsheetEndpoints(this IEndpointRouteBuilder app)
    {
        // agrupa as rotas de planilha 
        var group = app.MapGroup("/planilhas");

        // o endpoint recebe o arquivo, o serviço de leitura e o serviço de banco de dados
        group.MapPost("/upload", async (
                IFormFile file,
                SpreadsheetService spreadsheetService,
                NotesPetroService notesPetroService,
                NoteCongasService noteCongasService) =>
            {
                try
                {
                    // 1 = validação básica do arquivo
                    if (file == null || file.Length == 0)
                        return Results.BadRequest(new { message = "Arquivo inválido ou vazio." });

                    // 2 = lê a planilha usando o serviço adaptado e retorna uma lista de NotaModel
                    var resultadoPlanilha = await spreadsheetService.ReadNotasSpreadSheetAsync(file);

                    // 3 = insere as notas no banco de dados usando o seu NotasService
                    int inseridas = 0;
                    
                    // DECISÃO DE ROTA BASEADA NA IDENTIFICAÇÃO
                    if (resultadoPlanilha.TipoIdentificado == "PETRO")
                    {
                        if (!resultadoPlanilha.NotasPetro.Any())
                            return Results.BadRequest(new
                                { message = "Planilha Petro reconhecida, mas sem dados válidos." });

                        foreach (var nota in resultadoPlanilha.NotasPetro)
                        {
                            await notesPetroService.CreateNotaAsync(nota);
                            inseridas++;
                        }

                        return Results.Ok(new { message = $"{inseridas} notas da PETRO processadas com sucesso!" });
                    }
                    else if (resultadoPlanilha.TipoIdentificado == "CONGAS")
                    {
                        if (!resultadoPlanilha.NotasCongas.Any())
                            return Results.BadRequest(new
                                { message = "Planilha Congas reconhecida, mas sem dados válidos." });

                        foreach (var nota in resultadoPlanilha.NotasCongas)
                        {
                            await noteCongasService.CreateNotaAsync(nota);
                            inseridas++;
                        }

                        return Results.Ok(new { message = $"{inseridas} notas da CONGAS processadas com sucesso!" });
                    }
                    else
                    {
                        return Results.BadRequest(new
                            { message = "Formato de planilha não reconhecido. Verifique os cabeçalhos." });
                    }
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = $"Erro ao processar planilha: {ex.Message}" });
                }
            })
            .DisableAntiforgery();
    }
}