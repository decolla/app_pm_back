using app_pm_back.Data;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using app_pm_back.Services.Manutencao;
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
 OrdemService ordemService,
 ItemNotaService itemNotaService,
 DadosRenovacaoService dadosRenovacaoService
 ) =>
 {
 try
 {
 if (file == null || file.Length == 0)
 return Results.BadRequest(new { message = "Arquivo inválido ou vazio." });

 var resultadoPlanilha = await spreadsheetService.ReadNotasSpreadSheetAsync(file);

 if (resultadoPlanilha.TipoIdentificado == "NOTA")
 {
 if (!resultadoPlanilha.Notas.Any())
 return Results.BadRequest(new { message = "Planilha reconhecida, mas sem dados válidos." });

 var notasDeduplicadas = resultadoPlanilha.Notas
 .DistinctBy(o => o.NumeroNota).ToList();
 
 await notaManutencaoService.ImportarNotasEmLoteSeguroAsync(notasDeduplicadas);
 var inseridas = notasDeduplicadas.Count;

 idNotas = notasDeduplicadas.Select(n => n.IdNota).ToList();
 if (idItens.Count > 1)
 {
 await dadosRenovacaoService.ImportarDadosNotaAsync(idNotas, idItens);
 }


 return Results.Ok(new { message = $"{inseridas} notas processadas com sucesso!" });
 }
 else if (resultadoPlanilha.TipoIdentificado == "ORDEM")
 {
 if (!resultadoPlanilha.Ordens.Any())
 return Results.BadRequest(new { message = "Planilha de ordens reconhecida, mas sem dados válidos." });

 var ordensDeduplicadas = resultadoPlanilha.Ordens
 .DistinctBy(o => o.IdOrdem).ToList();
 
 var idOrdens = ordensDeduplicadas.Select(n => n.IdOrdem).ToList();
 await dadosRenovacaoService.ImportarDadosOrdemAsync(idOrdens);

 await ordemService.ImportarOrdensEmLoteSeguroAsync(ordensDeduplicadas);
 var inseridas = ordensDeduplicadas.Count;

 return Results.Ok(new { message = $"{inseridas} ordens processadas com sucesso!" });
 }
 else if (resultadoPlanilha.TipoIdentificado == "ITEM")
 {
 if (!resultadoPlanilha.Itens.Any())
 return Results.BadRequest(new { message = "Planilha de itens reconhecida, mas sem dados válidos." });

 var sapNumbers = resultadoPlanilha.Itens.Select(i => i.IdNota).Distinct().ToList();

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
 foreach (var item in resultadoPlanilha.Itens)
 {
 if (notasNoBanco.TryGetValue(item.IdNota, out long idInterno))
 {
 item.IdNota = idInterno;
 itensValidos.Add(item);
 }
 }

 if (itensValidos.Any())
 {
 await itemNotaService.ImportarItensEmLoteSeguroAsync(itensValidos);
 
 idItens = itensValidos.Select(i => i.IdItemNota).ToList();
 if (idNotas.Count > 1 )
 {
 await dadosRenovacaoService.ImportarDadosNotaAsync(idNotas, idItens);
 }
 
 return Results.Ok(new { message = $"{itensValidos.Count} itens vinculados e salvos!" });
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