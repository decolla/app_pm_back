using app_pm_back.Data;
using app_pm_back.Models.Domain;
using app_pm_back.Services.ExternIntegationsServices;
using app_pm_back.Services.Manutencao;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.FichasServices;

public class ImportSheetService
{
    private readonly AppDbContext _dbContext;
    private readonly SpreadsheetService _spreadsheetService;
    private readonly NotaService _notaService;
    private readonly OrdemService _ordemService;
    private readonly ItemNotaService _itemNotaService;
    private readonly DadosRenovacaoService _dadosRenovacaoService;
    private readonly SynchronizeAPMService _synchronizeApmService;
    private readonly ILogger<ImportSheetService> _logger;


    public ImportSheetService(
        ILogger<ImportSheetService> logger,
        AppDbContext dbContext,
        SpreadsheetService spreadsheetService,
        NotaService notaService,
        OrdemService ordemService,
        ItemNotaService itemNotaService,
        DadosRenovacaoService dadosRenovacaoService,
        SynchronizeAPMService synchronizeApmService)
    {
        _dbContext = dbContext;
        _spreadsheetService = spreadsheetService;
        _notaService = notaService;
        _ordemService = ordemService;
        _itemNotaService = itemNotaService;
        _dadosRenovacaoService = dadosRenovacaoService;
        _synchronizeApmService = synchronizeApmService;
        _logger = logger;
    }

    /// <summary>
    /// Processa dados da planilha do excel para verificar se precisa cadastrar nota, ordem ou item
    /// </summary>
    public async Task<int> ProcessarPlanilhaAsync(
        IFormFile file, 
        bool sobrescrever, 
        bool adicionarNovas, 
        CancellationToken cancellationToken = default)
    {
        var dbSchema = Config.RouteConfig.GetDatabaseSchema();
        if (dbSchema == "cg")
            throw new ArgumentException("PEDRO ADVERTE: Importação de planilhas desabilitada para o schema CG.");

        _logger.LogInformation("Baixando arquivo da rede para a memória (RAM)");
        
        //LER PARA MEMÓRIA
        // Isso evita que o leitor de Excel (EPPlus/ClosedXML) trave tentando fazer "seek" na rede.
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0; // Rebobina para o início

        _logger.LogInformation("Arquivo de {Bytes} bytes baixado. Iniciando leitura da planilha...", memoryStream.Length);
        
        // ReadNotasSpreadSheetAsync aceita um 'Stream' ao invés de 'IFormFile' para otimização
        // Adicionando o 'file.FileName' como o segundo parâmetro na chamada
        var resultadoPlanilha = await _spreadsheetService.ReadNotasSpreadSheetAsync(
            memoryStream, 
            file.FileName, 
            cancellationToken
        );
        
        List<long> idNotas = new List<long>();
        List<long> idItens = new List<long>();

        if (resultadoPlanilha.TipoIdentificado == "NOTA")
        {
            _logger.LogInformation("Planilha identificada como nota");
            
            if (!resultadoPlanilha.Notas.Any())
                throw new ArgumentException("Planilha reconhecida, mas sem dados válidos.");

            List<NotaModel> notasDeduplicadas = resultadoPlanilha.Notas.DistinctBy(o => o.NumeroNota).ToList();
            _logger.LogInformation("Planilha lida com sucesso: {Count} notas únicas encontradas. Iniciando gravação otimizada.", notasDeduplicadas.Count);

            // PROCESSAMENTO EM LOTES MAIORES USANDO MÉTODO GENÉRICO
            await ImportarComProgressoAsync(
                notasDeduplicadas, 
                "notas", 
                (lote, ct) => _notaService.ImportarNotasEmLoteSeguroAsync(lote, sobrescrever, adicionarNovas, ct), 
                cancellationToken);
            
            idNotas = notasDeduplicadas.Select(n => n.IdNota).ToList();
            
            if (idNotas.Count > 0) 
            {
                try 
                {
                    _logger.LogInformation("Iniciando sincronização APM...");
                    bool sinc = await _synchronizeApmService.SynchronizeApmAsync(cancellationToken); 
                    _logger.LogInformation($"Sincronização APM: {(sinc ? "OK" : "Falhou")}.");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("A API do APM retornou erro. A importação foi concluída, mas a sincronização APM falhou: {Message}", ex.Message);
                }
                
                await _dadosRenovacaoService.ImportarDadosNotaAsync(idNotas, idItens, cancellationToken);
            }

            _logger.LogInformation("Processamento de NOTAS finalizado com sucesso!");
            return notasDeduplicadas.Count;
        }
        else if (resultadoPlanilha.TipoIdentificado == "ORDEM")
        {
            if (!resultadoPlanilha.Ordens.Any())
                throw new ArgumentException("Planilha de ordens reconhecida, mas sem dados válidos.");

            List<OrdemModel> ordensDeduplicadas = resultadoPlanilha.Ordens.DistinctBy(o => o.NumeroOrdem).ToList();
            _logger.LogInformation("Planilha lida com sucesso: {Count} ordens únicas encontradas. Iniciando gravação otimizada.", ordensDeduplicadas.Count);

            await VincularOrdensNotasAsync(ordensDeduplicadas, cancellationToken);
            
            // PROCESSAMENTO EM LOTES MAIORES USANDO MÉTODO GENÉRICO
            await ImportarComProgressoAsync(
                ordensDeduplicadas, 
                "ordens", 
                (lote, ct) => _ordemService.ImportarOrdensEmLoteSeguroAsync(lote, sobrescrever, adicionarNovas, ct), 
                cancellationToken);
            
            var idOrdens = ordensDeduplicadas.Select(n => n.IdOrdem).ToList();

            if (idOrdens.Count > 0)
            {
                _logger.LogInformation("Planilha identificada como ordem");
                try
                {
                    _logger.LogInformation("Iniciando sincronização APM para ordens...");
                    bool sinc = await _synchronizeApmService.SynchronizeApmAsync(cancellationToken);
                    _logger.LogInformation(
                        $"Sincronização com APM {(sinc ? "bem-sucedida" : "falhou")} antes de importar dados de renovação.");
                }
                catch (Exception ex)
                {
                    // SE O APM FALHAR, CAPTURA O ERRO AQUI E GERA O AVISO!
                    _logger.LogWarning(
                        "A API do APM retornou erro. A importação foi concluída, mas a sincronização APM falhou: {Message}",
                        ex.Message);
                }

                await _dadosRenovacaoService.ImportarDadosOrdemAsync(idOrdens, cancellationToken);
            }

            _logger.LogInformation("Processamento de ORDENS finalizado com sucesso!");
            return ordensDeduplicadas.Count;
        }
        else if (resultadoPlanilha.TipoIdentificado == "ITEM")
        {
            _logger.LogInformation("Planilha identificada como item");
            if (!resultadoPlanilha.Itens.Any())
                throw new ArgumentException("Planilha de itens reconhecida, mas sem dados válidos.");

            var sapNumbers = resultadoPlanilha.Itens.Select(i => i.IdNota).Distinct().ToList();

            var notasNoBancoList = await _dbContext.PM_Notas
                .Where(n => sapNumbers.Contains(n.NumeroNota))
                .Select(n => new { n.IdNota, n.NumeroNota })
                .ToListAsync(cancellationToken);

            var notasNoBanco = notasNoBancoList
                .GroupBy(n => n.NumeroNota) 
                .ToDictionary(g => g.Key, g => g.First().IdNota);

            var notasFaltantes = sapNumbers.Except(notasNoBanco.Keys).ToList();
            if (notasFaltantes.Any())
            {
                await ImportGhostHelper.EnsureNotasAsync(_dbContext, notasFaltantes.Select(n => (long?)n));
                var notasNovas = await _dbContext.PM_Notas
                    .Where(n => notasFaltantes.Contains(n.NumeroNota))
                    .Select(n => new { n.IdNota, n.NumeroNota })
                    .ToListAsync(cancellationToken);
                foreach (var nota in notasNovas)
                {
                    notasNoBanco[nota.NumeroNota] = nota.IdNota;
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
                _logger.LogInformation("Planilha lida com sucesso: {Count} itens válidos vinculados. Iniciando gravação otimizada.", itensValidos.Count);
                
                // PROCESSAMENTO EM LOTES MAIORES USANDO MÉTODO GENÉRICO
                await ImportarComProgressoAsync(
                    itensValidos, 
                    "itens", 
                    (lote, ct) => _itemNotaService.ImportarItensEmLoteSeguroAsync(lote, ct), 
                    cancellationToken);

                idItens = itensValidos.Select(i => i.IdItemNota).ToList();
                idNotas = notasNoBanco.Values.Distinct().ToList(); // Resgatando IDs internos para usar na renovação

                if (idItens.Count > 0 && idNotas.Count > 0)
                {
                    _logger.LogInformation("Iniciando importação de dados de renovação de itens e notas...");
                    await _dadosRenovacaoService.ImportarDadosNotaAsync(idNotas, idItens, cancellationToken);
                }

                _logger.LogInformation("Processamento de ITENS finalizado com sucesso!");
                return itensValidos.Count;
            }

            throw new ArgumentException("Nenhum item pôde ser vinculado a uma nota existente no banco.");
        }
        else
        {
            throw new ArgumentException("Formato de planilha não reconhecido. Verifique os cabeçalhos.");
        }
    }

    /// <summary>
    /// A cada 1000 linhas lidas no excel manda um log, cadastra em batches
    /// </summary>
    private async Task ImportarComProgressoAsync<T>(
        List<T> itens, 
        string nomeEntidade, 
        Func<List<T>, CancellationToken, Task> acaoImportacao, 
        CancellationToken ct)
    {
        int batchSize = 10000; 
        int totalProcessadas = 0;
        int totalItens = itens.Count;

        // Divide a lista genérica em blocos de 'batchSize'
        var lotes = itens.Select((x, i) => new { Index = i, Value = x })
                         .GroupBy(x => x.Index / batchSize)
                         .Select(x => x.Select(v => v.Value).ToList())
                         .ToList();

        foreach (var lote in lotes)
        {
            if (ct.IsCancellationRequested)
            {
                _logger.LogWarning("Operação cancelada na linha {Linha}.", totalProcessadas);
                ct.ThrowIfCancellationRequested();
            }

            // Executa a função específica de salvar no banco passada via parâmetro
            await acaoImportacao(lote, ct);

            totalProcessadas += lote.Count;
            
            // LOG DE PROGRESSO
            _logger.LogInformation("PROGRESSO RÁPIDO: {Processadas} / {Total} {Entidade} processadas...", totalProcessadas, totalItens, nomeEntidade);
        }
    }
    
    /// <summary>
    /// Resolve e prepara os vínculos de chaves estrangeiras entre Ordens e Notas antes da inserção no banco de dados.
    /// </summary>
    private async Task VincularOrdensNotasAsync(List<OrdemModel> ordens, CancellationToken cancellationToken)
    {
        // 1. Extrai os números (assumindo que 0 seria um valor inválido/vazio para a nota)
        var sapNumbers = ordens
            .Where(o => o.Nota != null && o.Nota.NumeroNota > 0)
            .Select(o => o.Nota.NumeroNota)
            .Distinct()
            .ToList();

        if (!sapNumbers.Any()) return;

        // 2. Busca as existentes (agora a chave do dicionário é implicitamente long)
        var notasNoBancoList = await _dbContext.PM_Notas
            .Where(n => sapNumbers.Contains(n.NumeroNota))
            .Select(n => new { n.IdNota, n.NumeroNota })
            .ToListAsync(cancellationToken);

        var notasNoBanco = notasNoBancoList
            .GroupBy(n => n.NumeroNota)
            .ToDictionary(g => g.Key, g => g.First().IdNota);

        // 3. Cria as notas fantasmas
        var notasFaltantes = sapNumbers.Except(notasNoBanco.Keys).ToList();
        if (notasFaltantes.Any())
        {
            // O EnsureNotasAsync pede IEnumerable<long?>, então o cast (long?) é suficiente
            await ImportGhostHelper.EnsureNotasAsync(_dbContext, notasFaltantes.Select(n => (long?)n));
        
            var notasNovas = await _dbContext.PM_Notas
                .Where(n => notasFaltantes.Contains(n.NumeroNota))
                .Select(n => new { n.IdNota, n.NumeroNota })
                .ToListAsync(cancellationToken);

            foreach (var nova in notasNovas) 
            {
                notasNoBanco[nova.NumeroNota] = nova.IdNota;
            }
        }

        // 4. Aplica o De/Para e limpa a navegação
        foreach (var ordem in ordens)
        {
            if (ordem.Nota != null)
            {
                // Tenta achar o ID gerado para vincular a FK
                if (notasNoBanco.TryGetValue(ordem.Nota.NumeroNota, out long idInterno))
                {
                    ordem.IdNota = idInterno;
                }
                
                ordem.Nota = null; 
            }
        }
    }
}