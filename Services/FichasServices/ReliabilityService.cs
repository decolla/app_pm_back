using System.Text.Json;
using app_pm_back.Data;
using app_pm_back.DTO.Restauracao;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using AutoMapper.Internal;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.DTO;

public class ReliabilityService
{
    private readonly AppDbContext _context;
    private readonly DbSet<NotaModel> _notaModels;
    private readonly DbSet<OrdemModel> _ordemModels;
    private readonly ILogger<ReliabilityService> _logger;
    private readonly ExternIntegrationService _externIntegrationService;
    
    public ReliabilityService(AppDbContext context, ExternIntegrationService externIntegrationService, ILogger<ReliabilityService> logger)
    {
        _context = context;
        _notaModels = context.PM_Notas;
        _ordemModels = context.PM_Ordens;
        _externIntegrationService = externIntegrationService;
        _logger = logger;
    }
    
    public async Task<object?> DatasReliability(string idLi)
    {
        //Busca o registro de confiabilidade base para pegar as listas de IDs
        var reliability = await _context.PM_Reliability
            .AsNoTracking() 
            .FirstOrDefaultAsync(r => r.IdLi == idLi);

        //Variáveis para guardar as datas (inicializam vazias)
        List<DateTime> datasNotas = new List<DateTime>();
        List<DateTime> datasOrdens = new List<DateTime>();

        //Se achou o Li, processa as Notas apenas se a lista tiver IDs
        if (reliability?.IdsNotas != null && reliability.IdsNotas.Any())
        {
            datasNotas = await _notaModels.AsNoTracking()
                .Where(n => reliability.IdsNotas.Contains(n.IdNota) && n.DataNota.HasValue)
                .Select(n => n.DataNota!.Value)
                .ToListAsync();
        }

        //Processa as Ordens apenas se a lista tiver IDs
        if (reliability?.IdsOrdens != null && reliability.IdsOrdens.Any())
        {
            datasOrdens = await _ordemModels.AsNoTracking()
                .Where(o => reliability.IdsOrdens.Contains(o.IdOrdem) && o.DataCriacao.HasValue)
                .Select(o => o.DataCriacao!.Value)
                .ToListAsync();
        }

        //Junta tudo, remove duplicadas e ordena cronologicamente
        var todasDatas = datasNotas
            .Concat(datasOrdens)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        return new
        {
            SapName = idLi,
            Datas = todasDatas
        };
    }
    
    /// <summary>
    /// Busca a lista de Li do APM e preenche a tabela
    /// </summary>
    /// <returns> Bool se a operação ocorreu corretamente</returns>
    public async Task<bool> SyncLi()
    {
        _logger.LogInformation("Iniciando a sincronização dos dados do APM");

        // Define um timeout de 30 segundos para a requisição da API e do banco
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        try
        {
            _logger.LogInformation("Chamando API APM: GET /api/V1/Asset/Components/All com timeout de 30s.");
            
            var jsonResponse = await _externIntegrationService
                .GetDataAsync("/api/V1/Asset/Components/All", cts.Token);
            
            _logger.LogInformation("Resposta da API APM recebida com sucesso. Tamanho: {Length} caracteres.", jsonResponse?.Length ?? 0);

            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                _logger.LogWarning("Resposta vazia retornada pela API APM.");
                return false;
            }

            using var jsonDoc = JsonDocument.Parse(jsonResponse);
        
            //Pega todos os nomes válidos da API
            var nomesRecebidos = jsonDoc.RootElement.EnumerateArray()
                .Select(elemento => elemento.GetProperty("name").GetString())
                .Where(nome => !string.IsNullOrWhiteSpace(nome))
                .Distinct()
                .ToList();

            if (!nomesRecebidos.Any())
            {
                return false;
            }

            // Busca no banco QUAIS desses nomes já existem lá
            var nomesExistentesNoBanco = await _context.PM_Reliability
                .Where(r => nomesRecebidos.Contains(r.IdLi))
                .Select(r => r.IdLi)
                .ToListAsync(cts.Token);

            // Compara as duas listas e separa APENAS os nomes que ainda não estão no banco
            var nomesParaInserir = nomesRecebidos
                .Except(nomesExistentesNoBanco)
                .ToList();

            // Se não tiver ninguém novo para inserir, encerra com sucesso (true) pois já está tudo sincronizado
            if (!nomesParaInserir.Any())
            {
                _logger.LogInformation("Nenhum LI novo para inserir. Sincronização concluída.");
                return true; 
            }

            //Prepara a lista para o banco usando APENAS os nomes novos
            var novosRegistros = nomesParaInserir.Select(nome => new ReliabilityModel
            {
                IdLi = nome
            }).ToList();

            //Salva no banco 
            await _context.PM_Reliability.AddRangeAsync(novosRegistros, cts.Token);
            var alteracoes = await _context.SaveChangesAsync(cts.Token);

            return alteracoes > 0;
        }
        catch (OperationCanceledException ex) when (cts.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout na sincronização.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado.");
            throw;
        }
    }
    
    public async Task<bool> FiltraFichas(
    List<Classificacao> classificacoes, 
    List<MetodoClassificacao> metodos)
{
    // Previne erros caso a lista venha nula
    classificacoes ??= new List<Classificacao>();
    metodos ??= new List<MetodoClassificacao>();

    try
    {
        // Busca e agrupa as Notas por IdLi
        var notasQuery = _context.PM_Notas.AsQueryable();

        if (classificacoes.Any())
            notasQuery = notasQuery.Where(n => classificacoes.Contains(n.Classificacao));
        
        if (metodos.Any())
            notasQuery = notasQuery.Where(n => metodos.Contains(n.MetodoClassificacao));

        // Agrupa as notas pelo IdLi e cria um dicionário onde a Chave é o IdLi e o Valor é a lista de Ids das Notas
        var dicionarioNotas = await notasQuery
            .Where(n => n.IdLi != null)
            .GroupBy(n => n.IdLi)
            .ToDictionaryAsync(
                grupo => grupo.Key, 
                grupo => grupo.Select(n => n.IdNota).ToList()
            );

        // 2. Busca e agrupa as Ordens por IdLi (mesma lógica)
        var ordensQuery = _context.PM_Ordens.AsQueryable();

        if (classificacoes.Any())
            ordensQuery = ordensQuery.Where(o => classificacoes.Contains(o.Classificacao));
        
        if (metodos.Any())
            ordensQuery = ordensQuery.Where(o => metodos.Contains(o.MetodoClassificacao));

        var dicionarioOrdens = await ordensQuery
            .Where(o => o.IdLi != null) 
            .GroupBy(o => o.IdLi)
            .ToDictionaryAsync(
                grupo => grupo.Key, 
                grupo => grupo.Select(o => o.IdOrdem).ToList()
            );

        // Descobre quais IdLis foram afetados para não buscar a tabela Reliability inteira à toa
        var idsLiAfetados = dicionarioNotas.Keys
            .Union(dicionarioOrdens.Keys)
            .Distinct()
            .ToList();

        if (!idsLiAfetados.Any())
        {
            // Não encontrou nenhuma nota ou ordem com esses filtros, então não há o que atualizar
            return false; 
        }

        // Busca os registros do Reliability que correspondem a esses IdLis
        var registrosReliability = await _context.PM_Reliability
            .Where(r => idsLiAfetados.Contains(r.IdLi))
            .ToListAsync();

        //Preenche as listas IdsNotas e IdsOrdens no Reliability
        foreach (var reliability in registrosReliability)
        {
            // Se o IdLi existir no dicionário de notas, ele joga a lista de Ids na propriedade IdsNotas
            if (dicionarioNotas.TryGetValue(reliability.IdLi, out var idsNotas))
            {
                reliability.IdsNotas = idsNotas;
            }

            // Faz o mesmo para as ordens
            if (dicionarioOrdens.TryGetValue(reliability.IdLi, out var idsOrdens))
            {
                reliability.IdsOrdens = idsOrdens;
            }
        }

        //Salva as alterações no banco de dados
        var alteracoes = await _context.SaveChangesAsync();
        return alteracoes > 0;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro ao tentar sincronizar notas e ordens no Reliability.");
        throw;
    }
}
}