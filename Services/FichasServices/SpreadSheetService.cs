using app_pm_back.Data;
using app_pm_back.Models.Domain;
using app_pm_back.Services.ExternIntegationsServices;
using app_pm_back.Services.Manutencao;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.FichasServices;

public class SpreadSheetService
{
    private readonly AppDbContext _dbContext;
    private readonly SpreadsheetService _spreadsheetService;
    private readonly NotaManutencaoService _notaManutencaoService;
    private readonly OrdemService _ordemService;
    private readonly ItemNotaService _itemNotaService;
    private readonly DadosRenovacaoService _dadosRenovacaoService;
    private readonly SynchronizeAPMService _synchronizeApmService;

    // Construtor injetando tudo que o endpoint antigo precisava
    public SpreadSheetService(
        AppDbContext dbContext,
        SpreadsheetService spreadsheetService,
        NotaManutencaoService notaManutencaoService,
        OrdemService ordemService,
        ItemNotaService itemNotaService,
        DadosRenovacaoService dadosRenovacaoService,
        SynchronizeAPMService synchronizeApmService)
    {
        _dbContext = dbContext;
        _spreadsheetService = spreadsheetService;
        _notaManutencaoService = notaManutencaoService;
        _ordemService = ordemService;
        _itemNotaService = itemNotaService;
        _dadosRenovacaoService = dadosRenovacaoService;
        _synchronizeApmService = synchronizeApmService;
    }

    public async Task<int> ProcessarPlanilhaAsync(IFormFile file, bool sobrescrever, bool adicionarNovas)
    {
        var dbSchema = Environment.GetEnvironmentVariable("DATABASE_SCHEMA");
        if (dbSchema == "cg")
            throw new ArgumentException("PEDRO ADVERTE: Importação de planilhas desabilitada para o schema CG.");

        var resultadoPlanilha = await _spreadsheetService.ReadNotasSpreadSheetAsync(file);
        List<long> idNotas = new List<long>();
        List<int> idItens = new List<int>();

        if (resultadoPlanilha.TipoIdentificado == "NOTA")
        {
            if (!resultadoPlanilha.Notas.Any())
                throw new ArgumentException("Planilha reconhecida, mas sem dados válidos.");

            List<NotaModel> notasDeduplicadas = resultadoPlanilha.Notas.DistinctBy(o => o.NumeroNota).ToList();

            await _notaManutencaoService.ImportarNotasEmLoteSeguroAsync(notasDeduplicadas, sobrescrever, adicionarNovas);
            
            idNotas = notasDeduplicadas.Select(n => n.IdNota).ToList();
            if (idItens.Count > 1) // Mantive a sua lógica original
            {
                bool sinc = await _synchronizeApmService.SynchronizeAPMAsync(); 
                Console.WriteLine($"Sincronização com APM {(sinc ? "bem-sucedida" : "falhou")} antes de importar dados de renovação.");
                await _dadosRenovacaoService.ImportarDadosNotaAsync(idNotas, idItens);
            }

            return notasDeduplicadas.Count;
        }
        else if (resultadoPlanilha.TipoIdentificado == "ORDEM")
        {
            if (!resultadoPlanilha.Ordens.Any())
                throw new ArgumentException("Planilha de ordens reconhecida, mas sem dados válidos.");

            List<OrdemModel> ordensDeduplicadas = resultadoPlanilha.Ordens.DistinctBy(o => o.NumeroOrdem).ToList();

            await _ordemService.ImportarOrdensEmLoteSeguroAsync(ordensDeduplicadas, sobrescrever, adicionarNovas);
            
            var idOrdens = ordensDeduplicadas.Select(n => n.IdOrdem).ToList();
            
            bool sinc = await _synchronizeApmService.SynchronizeAPMAsync(); 
            Console.WriteLine($"Sincronização com APM {(sinc ? "bem-sucedida" : "falhou")} antes de importar dados de renovação.");
            await _dadosRenovacaoService.ImportarDadosOrdemAsync(idOrdens);
            
            return ordensDeduplicadas.Count;
        }
        else if (resultadoPlanilha.TipoIdentificado == "ITEM")
        {
            if (!resultadoPlanilha.Itens.Any())
                throw new ArgumentException("Planilha de itens reconhecida, mas sem dados válidos.");

            var sapNumbers = resultadoPlanilha.Itens.Select(i => i.IdNota).Distinct().ToList();

            var notasNoBanco = await _dbContext.PM_Notas
                .Where(n => sapNumbers.Contains(n.NumeroNota))
                .Select(n => new { n.IdNota, n.NumeroNota })
                .ToDictionaryAsync(n => n.NumeroNota, n => n.IdNota);

            var notasFaltantes = sapNumbers.Except(notasNoBanco.Keys).ToList();
            if (notasFaltantes.Any())
            {
                await ImportGhostHelper.EnsureNotasAsync(_dbContext, notasFaltantes.Select(n => (long?)n));
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
                await _itemNotaService.ImportarItensEmLoteSeguroAsync(itensValidos);

                idItens = itensValidos.Select(i => i.IdItemNota).ToList();
                if (idNotas.Count > 1)
                {
                    await _dadosRenovacaoService.ImportarDadosNotaAsync(idNotas, idItens);
                }

                return itensValidos.Count;
            }

            throw new ArgumentException("Nenhum item pôde ser vinculado a uma nota existente no banco.");
        }
        else
        {
            throw new ArgumentException("Formato de planilha não reconhecido. Verifique os cabeçalhos.");
        }
    }
}