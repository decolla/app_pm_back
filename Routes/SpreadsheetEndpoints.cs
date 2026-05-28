using app_pm_back.Data;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using app_pm_back.Services.CongasServices;
using AutoMapper.Internal;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Routes;

/// <summary>
/// Rotas para planilhas
/// </summary>
public static class SpreadsheetEndpoints
{
    public static void MapSpreadsheetEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/V1/planilhas");

        List<long> idNotas = new List<long>();
        List<int> idItens = new List<int>();

        group.MapPost("/upload", async (
            IFormFile file,
            AppDbContext dbContext,
            SpreadsheetService spreadsheetService,
            NotaManutencaoService notaManutencaoService,
            OrdemService orderCongasService,
            ItemNotaService itemNoteCongasService,
            DadosRenovacaoService dadosRenovacaoService
            ) =>
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Results.BadRequest(new { message = "Arquivo inválido ou vazio." });

                var resultadoPlanilha = await spreadsheetService.ReadNotasSpreadSheetAsync(file);

                if (resultadoPlanilha.TipoIdentificado == "NOTA_CONGAS")
                {
                    if (!resultadoPlanilha.NotasCongas.Any())
                        return Results.BadRequest(new { message = "Planilha Congas reconhecida, mas sem dados válidos." });

                    var notasDeduplicadas = resultadoPlanilha.NotasCongas
                        .DistinctBy(o => o.NumeroNota).ToList();
                    
                    await notaManutencaoService.ImportarNotasEmLoteSeguroAsync(notasDeduplicadas);
                    var inseridas = notasDeduplicadas.Count;

                    idNotas = notasDeduplicadas.Select(n => n.IdNota).ToList();
                    if (idItens.Count > 1)
                    {
                        await dadosRenovacaoService.ImportarDadosNotaAsync(idNotas, idItens);
                    }


                    return Results.Ok(new { message = $"{inseridas} notas da CONGAS processadas com sucesso!" });
                }
                else if (resultadoPlanilha.TipoIdentificado == "ORDEM_CONGAS")
                {
                    if (!resultadoPlanilha.OrdensCongas.Any())
                        return Results.BadRequest(new { message = "Planilha de ordens Congas reconhecida, mas sem dados válidos." });

                    var ordensDeduplicadas = resultadoPlanilha.OrdensCongas
                        .DistinctBy(o => o.IdOrdem).ToList();
                    
                    var idOrdens = ordensDeduplicadas.Select(n => n.IdOrdem).ToList();
                    await dadosRenovacaoService.ImportarDadosOrdemAsync(idOrdens);

                    await orderCongasService.ImportarOrdensEmLoteSeguroAsync(ordensDeduplicadas);
                    var inseridas = ordensDeduplicadas.Count;

                    return Results.Ok(new { message = $"{inseridas} ordens da CONGAS processadas com sucesso!" });
                }
                else if (resultadoPlanilha.TipoIdentificado == "ITEM_CONGAS")
                {
                    if (!resultadoPlanilha.ItensCongas.Any())
                        return Results.BadRequest(new { message = "Planilha de itens CONGAS reconhecida, mas sem dados válidos." });

                    var sapNumbers = resultadoPlanilha.ItensCongas.Select(i => i.IdNota).Distinct().ToList();

                    var notasNoBanco = await dbContext.PM_Notas
                        .Where(n => sapNumbers.Contains(n.NumeroNota))
                        .Select(n => new { n.IdNota, n.NumeroNota })
                        .ToDictionaryAsync(n => n.NumeroNota, n => n.IdNota);

                    var notasFaltantes = sapNumbers.Except(notasNoBanco.Keys).ToList();
                    if (notasFaltantes.Any())
                    {
                        await ImportGhostHelper.EnsureNotasAsync(dbContext, notasFaltantes.Select(n => (long?)n));
                        foreach (var nota in notasFaltantes)
                        {
                            notasNoBanco[nota] = nota;
                        }
                    }

                    var itensValidos = new List<ItemNotaModel>();
                    foreach (var item in resultadoPlanilha.ItensCongas)
                    {
                        if (notasNoBanco.TryGetValue(item.IdNota, out long idInterno))
                        {
                            item.IdNota = idInterno;
                            itensValidos.Add(item);
                        }
                    }

                    if (itensValidos.Any())
                    {
                        await itemNoteCongasService.ImportarItensEmLoteSeguroAsync(itensValidos);
                        
                        idItens = itensValidos.Select(i => i.IdItemNota).ToList();
                        if (idNotas.Count > 1 )
                        {
                            await dadosRenovacaoService.ImportarDadosNotaAsync(idNotas, idItens);
                        }
                        
                        return Results.Ok(new { message = $"{itensValidos.Count} itens da CONGAS vinculados e salvos!" });
                    }

                    return Results.BadRequest(new { message = "Nenhum item pôde ser vinculado a uma nota existente no banco." });
                }
                else
                {
                    return Results.BadRequest(new { message = "Formato de planilha não reconhecido. Verifique os cabeçalhos." });
                }
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = $"Erro ao processar planilha: {ex.Message}" });
            }
        }).DisableAntiforgery();
    }
}