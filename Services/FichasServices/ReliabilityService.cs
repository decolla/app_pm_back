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

    public ReliabilityService(AppDbContext context, ExternIntegrationService externIntegrationService,
        ILogger<ReliabilityService> logger)
    {
        _context = context;
        _notaModels = context.PM_Notas;
        _ordemModels = context.PM_Ordens;
        _externIntegrationService = externIntegrationService;
        _logger = logger;
    }

    public async Task<object?> DatasReliability(string idLi, string? nomeAlternativo)
    {
        //Monta uma lista com os nomes que queremos procurar
        var nomesBusca = new List<string> { idLi };

        if (!string.IsNullOrWhiteSpace(nomeAlternativo))
        {
            nomesBusca.Add(nomeAlternativo);
        }

        //Busca no banco TODOS os registros que derem match com os nomes da lista
        var reliabilities = await _context.PM_Reliability
            .AsNoTracking()
            .Where(r => nomesBusca.Contains(r.IdLi))
            .ToListAsync();

        //Junta todos os IdsNotas encontrados (de 1 ou mais registros) em uma lista única e sem duplicatas
        var todosIdsNotas = reliabilities
            .Where(r => r.IdsNotas != null)
            .SelectMany(r => r.IdsNotas)
            .Distinct()
            .ToList();

        //Junta todos os IdsOrdens encontrados
        var todosIdsOrdens = reliabilities
            .Where(r => r.IdsOrdens != null)
            .SelectMany(r => r.IdsOrdens)
            .Distinct()
            .ToList();

        // Variáveis para guardar as datas
        List<DateTime> datasNotas = new List<DateTime>();
        List<DateTime> datasOrdens = new List<DateTime>();

        //Processa as Notas usando a lista unificada de IDs
        if (todosIdsNotas.Any())
        {
            datasNotas = await _notaModels.AsNoTracking()
                .Where(n => todosIdsNotas.Contains(n.IdNota) && n.DataNota.HasValue)
                .Select(n => n.DataNota!.Value)
                .ToListAsync();
        }

        //Processa as Ordens usando a lista unificada de IDs
        if (todosIdsOrdens.Any())
        {
            datasOrdens = await _ordemModels.AsNoTracking()
                .Where(o => todosIdsOrdens.Contains(o.IdOrdem) && o.DataCriacao.HasValue)
                .Select(o => o.DataCriacao!.Value)
                .ToListAsync();
        }

        //Junta tudo, remove duplicadas e ordena cronologicamente
        var todasDatas = datasNotas
            .Concat(datasOrdens)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        //Cria uma string mostrando quais nomes realmente foram encontrados (Ex: "LI_123 / ALTERNATIVO_123" ou só "LI_123")
        var nomesEncontrados = reliabilities.Any()
            ? string.Join(" / ", reliabilities.Select(r => r.IdLi))
            : idLi;

        return new
        {
            SapName = nomesEncontrados,
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

            _logger.LogInformation("Resposta da API APM recebida com sucesso. Tamanho: {Length} caracteres.",
                jsonResponse?.Length ?? 0);

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

public async Task<(List<string> IdsAfetados, bool HouveMudanca)> FiltraFichas(
    List<Classificacao> classificacoes,
    List<MetodoClassificacao> metodos)
{
    classificacoes ??= new List<Classificacao>();
    metodos ??= new List<MetodoClassificacao>();

    var idsComMudanca = new HashSet<string>();

    try
    {
        // --- Notas ---
        var notasQuery = _context.PM_Notas.AsQueryable();
        if (classificacoes.Any())
            notasQuery = notasQuery.Where(n => classificacoes.Contains(n.Classificacao));
        if (metodos.Any())
            notasQuery = notasQuery.Where(n => metodos.Contains(n.MetodoClassificacao));

        var dicionarioNotas = await notasQuery
            .Where(n => n.IdLi != null)
            .GroupBy(n => n.IdLi)
            .ToDictionaryAsync(
                grupo => grupo.Key!,
                grupo => grupo.Select(n => n.IdNota).ToList()
            );

        // --- Ordens ---
        var ordensQuery = _context.PM_Ordens.AsQueryable();
        if (classificacoes.Any())
            ordensQuery = ordensQuery.Where(o => classificacoes.Contains(o.Classificacao));
        if (metodos.Any())
            ordensQuery = ordensQuery.Where(o => metodos.Contains(o.MetodoClassificacao));

        var dicionarioOrdens = await ordensQuery
            .Where(o => o.IdLi != null)
            .GroupBy(o => o.IdLi)
            .ToDictionaryAsync(
                grupo => grupo.Key!,
                grupo => grupo.Select(o => o.IdOrdem).ToList()
            );

        var idsLiAfetados = dicionarioNotas.Keys
            .Union(dicionarioOrdens.Keys)
            .Distinct()
            .ToList();

        var registrosReliability = await _context.PM_Reliability
            .Where(r => idsLiAfetados.Contains(r.IdLi))
            .ToListAsync();

        foreach (var reliability in registrosReliability)
        {
            var idLi = reliability.IdLi;

            var novasNotas  = dicionarioNotas.TryGetValue(idLi, out var n)  ? n : new List<long>();
            var novasOrdens = dicionarioOrdens.TryGetValue(idLi, out var o) ? o : new List<long>();

            var notasAlteradas  = !new HashSet<long>(reliability.IdsNotas  ?? new()).SetEquals(novasNotas);
            var ordensAlteradas = !new HashSet<long>(reliability.IdsOrdens ?? new()).SetEquals(novasOrdens);

            if (notasAlteradas || ordensAlteradas)
            {
                reliability.IdsNotas  = novasNotas;
                reliability.IdsOrdens = novasOrdens;
                idsComMudanca.Add(idLi);
            }
        }

        // IdLis que vieram nos filtros mas não existem no Reliability (adições)
        var idsExistentes = registrosReliability.Select(r => r.IdLi).ToHashSet();
        foreach (var idLi in idsLiAfetados.Where(id => !idsExistentes.Contains(id)))
            idsComMudanca.Add(idLi);

        if (idsComMudanca.Any())
            await _context.SaveChangesAsync();

        return (idsComMudanca.ToList(), idsComMudanca.Any());
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro ao tentar sincronizar notas e ordens no Reliability.");
        throw;
    }
}
}