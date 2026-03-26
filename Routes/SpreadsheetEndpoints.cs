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
                NoteCongasService noteCongasService,
                OrderCongasService orderCongasService) =>
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

                        await notesPetroService.CreateNotasEmMassaAsync(resultadoPlanilha.NotasPetro.DistinctBy(o => o.IdNota).ToList());
                        inseridas++;

                        return Results.Ok(new { message = $"{inseridas} notas da PETRO processadas com sucesso!" });
                    }
                    else if (resultadoPlanilha.TipoIdentificado == "CONGAS")
                    {
                        if (!resultadoPlanilha.NotasCongas.Any())
                            return Results.BadRequest(new
                                { message = "Planilha Congas reconhecida, mas sem dados válidos." });

                        await noteCongasService.CreateNotasEmMassaAsync(resultadoPlanilha.NotasCongas.DistinctBy(o => o.IdNota).ToList());
                        inseridas++;

                        return Results.Ok(new { message = $"{inseridas} notas da CONGAS processadas com sucesso!" });
                    }
                    else if (resultadoPlanilha.TipoIdentificado == "ORDEM_CONGAS")
                    {
                        if (!resultadoPlanilha.OrdensCongas.Any())
                            return Results.BadRequest(new
                                { message = "Planilha de ordens Congas reconhecida, mas sem dados válidos." });


                        await orderCongasService.CreateNotasEmMassaAsync(resultadoPlanilha.OrdensCongas.DistinctBy(o => o.IdOrdem).ToList());

                        return Results.Ok(new { message = $"{inseridas} ordens da CONGAS processadas com sucesso!" });
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