using app_pm_back.Data;
using app_pm_back.Models.CongasModel;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using app_pm_back.Services.CongasServices;
using app_pm_back.Services.PetroServices;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Routes;

/// <summary>
/// Rotas para planilhas
/// </summary>
public static class SpreadsheetEndpoints
{
    /// <summary>
    /// Mapeia as rotas de planilhas
    /// </summary>
    /// <param name="app"></param>
    public static void MapSpreadsheetEndpoints(this IEndpointRouteBuilder app)
    {
        // agrupa as rotas de planilha 
        var group = app.MapGroup("api/V1/planilhas");

        // o endpoint recebe o arquivo, o serviço de leitura e o serviço de banco de dados
        group.MapPost("/upload", async (
            IFormFile file,
            AppDbContext dbContext,
            SpreadsheetService spreadsheetService,
            NotesPetroService notesPetroService,
            NotaManutencaoService notaManutencaoService,
            OrderCongasService orderCongasService,
            OrderPetroService orderPetroService,
            ItemNoteCongasService itemNoteCongasService,
            EquipmentService equipmentService
            // AcaoNotaComgasService acaoNotaComgasService
            ) =>
        {
            try
            {
                // 1 = validação básica do arquivo
                if (file == null || file.Length == 0)
                    return Results.BadRequest(new { message = "Arquivo inválido ou vazio." });

                // 2 = lê a planilha usando o serviço adaptado e retorna uma lista de NotaModel
                var resultadoPlanilha = await spreadsheetService.ReadNotasSpreadSheetAsync(file);
                

                // DECISÃO DE ROTA BASEADA NA IDENTIFICAÇÃO
                if (resultadoPlanilha.TipoIdentificado == "NOTA_PETRO")
                {
                    if (!resultadoPlanilha.NotasPetro.Any())
                        return Results.BadRequest(new
                            { message = "Planilha Petro reconhecida, mas sem dados válidos." });

                    await notesPetroService.CreateNotasEmMassaAsync(resultadoPlanilha.NotasPetro
                        .ToList());
                    var inseridas = resultadoPlanilha.NotasPetro.Count;

                    return Results.Ok(new { message = $"{inseridas} notas da PETRO processadas com sucesso!" });
                }
                else if (resultadoPlanilha.TipoIdentificado == "NOTA_CONGAS")
                {
                    if (!resultadoPlanilha.NotasCongas.Any())
                        return Results.BadRequest(new
                            { message = "Planilha Congas reconhecida, mas sem dados válidos." });

                    var notasDeduplicadas = resultadoPlanilha.NotasCongas
                        .DistinctBy(o => o.NumeroNota).ToList();

                    await notaManutencaoService.ImportarNotasEmLoteSeguroAsync(notasDeduplicadas);
                    var inseridas = notasDeduplicadas.Count;

                    return Results.Ok(new { message = $"{inseridas} notas da CONGAS processadas com sucesso!" });
                }
                else if (resultadoPlanilha.TipoIdentificado == "ORDEM_CONGAS")
                {
                    if (!resultadoPlanilha.OrdensCongas.Any())
                        return Results.BadRequest(new
                            { message = "Planilha de ordens Congas reconhecida, mas sem dados válidos." });

                    var ordensDeduplicadas = resultadoPlanilha.OrdensCongas
                        .DistinctBy(o => o.IdOrdem).ToList();

                    await orderCongasService.CreateNotasEmMassaAsync(ordensDeduplicadas);
                    var inseridas = ordensDeduplicadas.Count;

                    return Results.Ok(new { message = $"{inseridas} ordens da CONGAS processadas com sucesso!" });
                }
                else if (resultadoPlanilha.TipoIdentificado == "ORDEM_PETRO")
                {
                    if (!resultadoPlanilha.OrdensPetro.Any())
                        return Results.BadRequest(new
                            { message = "Planilha de ordens PETRO reconhecida, mas sem dados válidos." });

                    var ordensDeduplicadas = resultadoPlanilha.OrdensPetro
                        .DistinctBy(o => o.IdOrdem).ToList();

                    await orderPetroService.CreateNotasEmMassaAsync(ordensDeduplicadas);
                    var inseridas = ordensDeduplicadas.Count;

                    return Results.Ok(new { message = $"{inseridas} ordens da PETRO processadas com sucesso!" });
                }
                else if (resultadoPlanilha.TipoIdentificado == "ITEM_CONGAS")
                {
                    if (!resultadoPlanilha.ItensCongas.Any())
                        return Results.BadRequest(new
                            { message = "Planilha de itens CONGAS reconhecida, mas sem dados válidos." });

                    //  números SAP dos itens
                    var sapNumbers = resultadoPlanilha.ItensCongas.Select(i => i.NotaId).Distinct().ToList();

                    // busca os IDs das notas no banco de dados
                    var notasNoBanco = await dbContext.NotasManutencao
                        .Where(n => sapNumbers.Contains(n.NumeroNota))
                        .Select(n => new { n.IdNota, n.NumeroNota })
                        .ToDictionaryAsync(n => n.NumeroNota, n => n.IdNota);

                    // atribui o ID correto antes de salvar
                    var itensValidos = new List<ItemNotaManutencaoModel>();
                    foreach (var item in resultadoPlanilha.ItensCongas)
                    {
                        if (notasNoBanco.TryGetValue(item.NotaId, out long idInterno))
                        {
                            item.NotaId = idInterno;
                            itensValidos.Add(item);
                        }
                    }

                    if (itensValidos.Any())
                    {
                        await itemNoteCongasService.CreateNotasEmMassaAsync(itensValidos);
                        return Results.Ok(
                            new { message = $"{itensValidos.Count} itens da CONGAS vinculados e salvos!" });
                    }

                    return Results.BadRequest(new
                        { message = "Nenhum item pôde ser vinculado a uma nota existente no banco." });
                }
                else if (resultadoPlanilha.TipoIdentificado == "EQUIPAMENTO_SAP")
                {
                    if (!resultadoPlanilha.Equipamentos.Any())
                        return Results.BadRequest(new { message = "Planilha de equipamentos reconhecida, mas sem dados válidos." });

                    // Filtra duplicados da própria planilha
                    var listaParaSalvar = resultadoPlanilha.Equipamentos.DistinctBy(e => e.TagEquipamento).ToList();

                    // Guarda na base de dados
                    await equipmentService.CreateEquipamentosEmMassaAsync(listaParaSalvar);

                    int inseridas = listaParaSalvar.Count;

                    return Results.Ok(new { message = $"{inseridas} equipamentos processados e salvos com sucesso!" });
                }// Adicione este bloco no seu if/else das rotas
                // else if (resultadoPlanilha.TipoIdentificado == "ACAO_NOTA_CONGAS")
                // {
                //     if (!resultadoPlanilha.AcoesCongas.Any())
                //         return Results.BadRequest(new { message = "Planilha de Ações reconhecida, mas sem dados válidos." });
                //
                //     // Grava as ações no banco
                //     await acaoNotaComgasService.CreateAcoesEmMassaAsync(resultadoPlanilha.AcoesCongas);
                //     
                //     int inseridas = resultadoPlanilha.AcoesCongas.Count;
                //
                //     return Results.Ok(new { message = $"{inseridas} ações de Notas da CONGAS processadas com sucesso!" });
                // }
                
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
        }).DisableAntiforgery();
    }
}