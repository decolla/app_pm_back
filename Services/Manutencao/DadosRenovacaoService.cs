using System.Net.Http.Headers;
using System.Text.Json;
using app_pm_back.Data;
using app_pm_back.Services.Extensions;
using app_pm_back.Models.Domain;
using app_pm_back.DTO.Restauracao;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe DadosRenovacaoService.
/// </summary>
public class DadosRenovacaoService : BaseCrudService<DadosRenovacaoModel>
{
    private readonly AppDbContext _context;
    private readonly ILogger<DadosRenovacaoService> _logger;
    private List<(string IdLi, long? IdNota, long? IdOrdem, DateTime DataOrigem, OrigemUR Origem, TipoDados TipoDados)> _liParaRestaurar 
        = new List<(string, long?, long?, DateTime, OrigemUR, TipoDados)>();
    private const int TAMANHO_LOTE_LI = 50;
    private readonly HttpClient _httpClient;
    private readonly string _apmBaseUrl;
    private readonly DbSet<DadosRenovacaoModel> _dadosRenovacao;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="DadosRenovacaoService"/>.
    /// </summary>
    public DadosRenovacaoService(
        AppDbContext context, 
        ILogger<DadosRenovacaoService> logger,
        IConfiguration configuration,
        HttpClient httpClient) // Injetar HttpClient
        : base(context)
    {
        _logger = logger;
        _httpClient = httpClient;
        // Lê a URL base da APM do appsettings.json
        _apmBaseUrl = configuration["APIs:APM"] ?? "http://10.3.192.200:60141";
    }

//Faz tratamento em caso de notas +itens
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="idsNotas">Parâmetro do método.</param>
    /// <param name="idsItensNota">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    /// <exception cref="System.Exception">Lançada em caso de erro interno ou falha no processamento.</exception>
    public async Task<int> ImportarDadosNotaAsync(
        List<long> idsNotas,
        List<int> idsItensNota)
    {
        _logger.LogInformation("Iniciando ImportarDadosNotaAsync com {QtdNotas} notas e {QtdItens} itens",
            idsNotas?.Count ?? 0, idsItensNota?.Count ?? 0);

        if (idsNotas == null || idsNotas.Count == 0)
        {
            _logger.LogWarning("Lista de ids notas vazia ou nula");
            return 0;
        }

        try
        {
            //Busca notas e itens no banco
            var notas = await _context.PM_Notas
                .Where(n => idsNotas.Contains(n.IdNota))
                .ToListAsync();

            _logger.LogInformation("Carregadas {QtdNotas} notas do banco", notas.Count);

            var itens = await _context.PM_ItensNota
                .Where(i => idsItensNota.Contains(i.IdItemNota))
                .ToListAsync();

            _logger.LogInformation("Carregados {QtdItens} itens do banco", itens.Count);

            //Identifica as notas que possuem itens com palavras-chave de renovação
            var idsNotasRenovacao = itens
                .Where(i => ContemPalavraChaveItem(i.Causa, i.Sintoma))
                .Select(i => i.IdNota)
                .Distinct()
                .ToList();

            _logger.LogInformation("Identificadas {QtdNotas} notas com itens de renovação", idsNotasRenovacao.Count);

            if (idsNotasRenovacao.Count == 0)
            {
                _logger.LogWarning("Nenhuma nota com palavras-chave de renovação encontrada");
                return 0;
            }

            //Filtra as notas que devem ser marcadas com base nos ids retornados anteriormente
            var notasParaAtualizar = notas
                .Where(n => idsNotasRenovacao.Contains(n.IdNota))
                .ToList();

            //para cada nota indentificada, verifica se ela renova ativo ou LI
            foreach (var nota in notasParaAtualizar)
            {
                if (nota.IdAtivo.HasValue)
                {
                    // Se IdAtivo está preenchido → RestauraAtivo
                    nota.ConfiabilidadeRestaurada = ConfiabilidadeRestaurada.RestauraAtivo;
                    _logger.LogDebug("Nota {IdNota} marcada como RestauraAtivo (IdAtivo={IdAtivo})",
                        nota.IdNota, nota.IdAtivo);
                }
                else if (!string.IsNullOrWhiteSpace(nota.IdLi))
                {
                    // Se IdAtivo vazio mas IdLI preenchido → RestauraLI
                    nota.ConfiabilidadeRestaurada = ConfiabilidadeRestaurada.RestauraLI;
                    _logger.LogDebug("Nota {IdNota} marcada como RestauraLI (IdLI={IdLI})",
                        nota.IdNota, nota.IdLi);
                }
                // Se ambos vazios, não faz nada
            }

            //Agora faz uma nova busca em todas as notas que recebeu, não apenas as que tem item
            var palavrasRenovacao = new[] { "START", "RENOVAÇÃO", "RENOVACAO" };
            var palavrasTroca = new[] { "TROCA", "SUBSTITUIÇÃO", "SUBSTITUICAO" };

            foreach (var nota in notas)
            {
                var textosNota =
                    $"{nota.DescricaoBreve ?? string.Empty} {nota.AnomaliaObservada ?? string.Empty} {nota.ObservacoesComplementares ?? string.Empty}"
                        .ToUpperInvariant();

                bool temRenovacao = palavrasRenovacao.Any(p => textosNota.Contains(p));
                bool temTroca = palavrasTroca.Any(p => textosNota.Contains(p));

                //para cada nota indentificada, verifica se ela renova ativo ou LI
                if (temRenovacao)
                {
                    nota.ConfiabilidadeRestaurada = ConfiabilidadeRestaurada.RestauraLI;
                    _logger.LogDebug("Nota {IdNota} marcada como RestauraLI por texto (renovação)", nota.IdNota);
                }
                else if (temTroca && nota.IdAtivo.HasValue)
                {
                    nota.ConfiabilidadeRestaurada = ConfiabilidadeRestaurada.RestauraAtivo;
                    _logger.LogDebug("Nota {IdNota} marcada como RestauraAtivo por texto (troca)", nota.IdNota);
                }
            }


            //Salva e Cadastra dados de renovação
            await _context.SaveChangesAsync();
            _logger.LogInformation("Alterações salvas no banco de dados");

            var resultado = await CadastraDadosRenovacaoAsync(idsNotas, ehNota: true);
            _logger.LogInformation("ImportarDadosNotaAsync concluído com {QtdRegistros} registros criados/atualizados",
                resultado);

            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar notas em ImportarDadosNotaAsync");
            throw;
        }
    }

// Verifica se Causa ou Sintoma contém palavras-chave
    private static bool ContemPalavraChaveItem(string? causa, string? sintoma)
    {
        var texto = $"{causa ?? string.Empty} {sintoma ?? string.Empty}".ToUpperInvariant();

        return texto.Contains("START")
               || texto.Contains("RENOVAÇÃO")
               || texto.Contains("RENOVACAO");
    }

//Faz tratamento em caso de ordem
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="idsOrdens">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    /// <exception cref="System.Exception">Lançada em caso de erro interno ou falha no processamento.</exception>
    public async Task<int> ImportarDadosOrdemAsync(
        List<long> idsOrdens)
    {
        _logger.LogInformation("Iniciando ImportarDadosOrdemAsync com {QtdOrdens} ordens", idsOrdens?.Count ?? 0);

        if (idsOrdens == null || idsOrdens.Count == 0)
        {
            _logger.LogWarning("Lista de ordens vazia ou nula");
            return 0;
        }

        try
        {
            //Busca ordens no banco
            var ordens = await _context.PM_Ordens
                .Where(n => idsOrdens.Contains(n.IdOrdem))
                .ToListAsync();
            
            ordens = ordens.Where(o => o.Classificacao != "NÃO CLASSIFICADA").ToList();


            _logger.LogInformation("Carregadas {QtdOrdens} ordens do banco", ordens.Count);

            //Classifica as ordens baseado em DescricaoBreve
            var palavrasRenovacao = new[] { "TROCA", "START", "RENOVAÇÃO", "RENOVACAO" };
            var palavrasTroca = new[] { "SUBSTITUIÇÃO", "SUBSTITUICAO" };

            foreach (var ordem in ordens)
            {
                var textoOrdem = (ordem.DescricaoBreve ?? string.Empty).ToUpperInvariant();

                bool temRenovacao = palavrasRenovacao.Any(p => textoOrdem.Contains(p));
                bool temTroca = palavrasTroca.Any(p => textoOrdem.Contains(p));

                if (temRenovacao)
                {
                    ordem.ConfiabilidadeRestaurada = ConfiabilidadeRestaurada.RestauraLI;
                    _logger.LogDebug("Ordem {IdOrdem} marcada como RestauraLI", ordem.IdOrdem);
                }
                else if (temTroca && ordem.IdAtivo.HasValue)
                {
                    ordem.ConfiabilidadeRestaurada = ConfiabilidadeRestaurada.RestauraAtivo;
                    _logger.LogDebug("Ordem {IdOrdem} marcada como RestauraAtivo", ordem.IdOrdem);
                }
            }

            //Salva e Cadastra dados de renovação
            await _context.SaveChangesAsync();
            _logger.LogInformation("Alterações salvas no banco de dados");

            var resultado = await CadastraDadosRenovacaoAsync(idsOrdens, ehNota: false);
            _logger.LogInformation("ImportarDadosOrdemAsync concluído com {QtdRegistros} registros criados/atualizados",
                resultado);

            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar ordens em ImportarDadosOrdemAsync");
            throw;
        }
    }

    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="ids">Parâmetro do método.</param>
    /// <param name="ehNota">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    /// <exception cref="System.Exception">Lançada em caso de erro interno ou falha no processamento.</exception>
    public async Task<int> CadastraDadosRenovacaoAsync(
        List<long> ids,
        bool ehNota) // true = nota, false = ordem
    {
        _logger.LogInformation("Iniciando CadastraDadosRenovacaoAsync para {Tipo} com {QtdIds} IDs", ehNota,
            ids?.Count ?? 0);

        if (ids == null || ids.Count == 0)
        {
            _logger.LogWarning("Lista de IDs vazia ou nula para {Tipo}", ehNota);
            return 0;
        }

        _liParaRestaurar.Clear(); // Limpar lista no início
        var registrosCriados = 0;
        try
        {
            if (ehNota)
            {
                //se for nota, busca o id no banco
                var notas = await _context.PM_Notas
                    .Where(n => ids.Contains(n.IdNota))
                    .ToListAsync();

                notas = notas.Where(n => n.Classificacao != "NÃO CLASSIFICADA").ToList();

                _logger.LogInformation("Carregadas {QtdNotas} notas para cadastro de dados de renovação", notas.Count);

                //Cadastra cada nota 
                foreach (var nota in notas)
                {
                    _logger.LogDebug("Processando nota {IdNota} com ConfiabilidadeRestaurada={Conf}",
                        nota.IdNota, nota.ConfiabilidadeRestaurada);
                    var tipoDados = MapearParaTipoDados(nota.Classificacao, nota.MetodoClassificacao);

                    var inseridos = await ProcessarPorConfiabilidadeAsync(
                        nota.ConfiabilidadeRestaurada,
                        nota.IdAtivo,
                        nota.IdNota,
                        null,
                        nota.DataNota ?? DateTime.UtcNow,
                        OrigemUR.Nota,
                        nota.IdLi,
                        tipoDados);
                    registrosCriados += inseridos;
                    
                    if (_liParaRestaurar.Count >= TAMANHO_LOTE_LI)
                    {
                        _logger.LogInformation("Atingido tamanho de lote ({TamanhoLote}). Processando lote de LI's...", TAMANHO_LOTE_LI);
                        var lotePorProcessar = _liParaRestaurar
                            .Take(TAMANHO_LOTE_LI)
                            .DistinctBy(li => li.IdLi)  // ← Remover duplicatas por IdLi
                            .ToList();
                        registrosCriados += await ProcessarLoteLiAsync(lotePorProcessar);
                        _liParaRestaurar.RemoveRange(0, Math.Min(TAMANHO_LOTE_LI, _liParaRestaurar.Count));
                    }
                }
            }
            else

            {
                var ordens = await _context.PM_Ordens
                    .Where(o => ids.Contains(o.IdOrdem))
                    .ToListAsync();

                //Cadastra cada ordem
                _logger.LogInformation("Carregadas {QtdOrdens} ordens para cadastro de dados de renovação",
                    ordens.Count);

                foreach (var ordem in ordens)
                {
                    _logger.LogDebug("Processando ordem {IdOrdem} com ConfiabilidadeRestaurada={Conf}",
                        ordem.IdOrdem, ordem.ConfiabilidadeRestaurada);

                    var tipoDados = MapearParaTipoDados(ordem.Classificacao, ordem.MetodoClassificacao);

                    var inseridos = await ProcessarPorConfiabilidadeAsync(
                        ordem.ConfiabilidadeRestaurada,
                        ordem.IdAtivo,
                        null,
                        ordem.IdOrdem,
                        ordem.DataCriacao ?? DateTime.UtcNow,
                        OrigemUR.Ordem,
                        ordem.IdLi,
                        tipoDados);
                    registrosCriados += inseridos;
                    
                    if (_liParaRestaurar.Count >= TAMANHO_LOTE_LI)
                    {
                        _logger.LogInformation("Atingido tamanho de lote ({TamanhoLote}). Processando lote de LI's...", TAMANHO_LOTE_LI);
                        var lotePorProcessar = _liParaRestaurar.Take(TAMANHO_LOTE_LI).Distinct().ToList();
                        registrosCriados += await ProcessarLoteLiAsync(lotePorProcessar);
                        _liParaRestaurar.RemoveRange(0, Math.Min(TAMANHO_LOTE_LI, _liParaRestaurar.Count));
                    }
                }
            }
            if (_liParaRestaurar.Any())
            {
                _logger.LogInformation("Processando {QtdLI} LI's restantes", _liParaRestaurar.Count);
                var restantes = _liParaRestaurar
                    .DistinctBy(li => li.IdLi)  // ← Remover duplicatas
                    .ToList();
                registrosCriados += await ProcessarLoteLiAsync(restantes);
                _liParaRestaurar.Clear();
            }

            if (registrosCriados > 0)
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Salvos {QtdRegistros} registros de dados de renovação", registrosCriados);
            }
            else
            {
                _logger.LogWarning("Nenhum registro de dados de renovação foi criado ou atualizado");
            }
            
            return registrosCriados;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cadastrar dados de renovação para {Tipo}", ehNota);
            throw;
        }
    }

    //Trata dois casos, se a nota restaura apenas um ativo especifico ou toda a LI
    private async Task<int> ProcessarPorConfiabilidadeAsync(
        ConfiabilidadeRestaurada? confiabilidade,
        ComponentType? idAtivo,
        long? idNota,
        long? idOrdem,
        DateTime dataSource,
        OrigemUR origem,
        string idLi,
        TipoDados tipoDados)
    {
        if (confiabilidade == null)
        {
            _logger.LogDebug("ConfiabilidadeRestaurada é nula, nenhum processamento realizado");
            return 0;
        }
        

        var inseridos = 0;

        if (confiabilidade == ConfiabilidadeRestaurada.RestauraAtivo)
        {
            if (idAtivo.HasValue)
            {
                _logger.LogDebug("Processando RestauraAtivo para IdAtivo={IdAtivo}", idAtivo.Value);
                inseridos = await InserirOuAtualizarPorAtivoAsync(
                    idAtivo.Value,
                    idNota,
                    idOrdem,
                    dataSource,
                    origem,
                    idLi,
                    tipoDados
                    ); 
            }
            else
            {
                _logger.LogWarning("RestauraAtivo mas IdAtivo inválido ou nulo");
            }
        }
        else if (confiabilidade == ConfiabilidadeRestaurada.RestauraLI)
        {
            if (!string.IsNullOrWhiteSpace(idLi))
            {
                _logger.LogDebug("RestauraLI identificado - adicionando IdLi={IdLi} à fila de processamento", idLi);

                var existente = _liParaRestaurar.FirstOrDefault(li => li.IdLi == idLi);

                if (existente == default)
                {
                    _liParaRestaurar.Add((idLi, idNota, idOrdem, dataSource, origem, tipoDados));  // ← INCLUIR tipoDados
                    _logger.LogDebug("IdLi {IdLi} adicionado à fila com origem. Total na fila: {Total}", 
                        idLi, _liParaRestaurar.Count);
                }
                else
                {
                    _logger.LogDebug("IdLi {IdLi} já existe na fila, mantendo primeira ocorrência", idLi);
                }
            }
            else
            {
                _logger.LogWarning("RestauraLI marcado mas IdLi nulo ou vazio para Nota={IdNota}, Ordem={IdOrdem}", 
                    idNota, idOrdem);
            }
        }

        return inseridos;
    }
    private async Task<int> ProcessarLoteLiAsync(
        List<(string IdLi, long? IdNota, long? IdOrdem, DateTime DataOrigem, OrigemUR Origem, TipoDados TipoDados)> liComContexto)
    {
    if (!liComContexto.Any())
    {
        _logger.LogDebug("Lista de LI's vazia, nenhum processamento realizado");
        return 0;
    }
    var lis = liComContexto.Select(li => li.IdLi).ToList();
    _logger.LogInformation("Iniciando processamento de lote com {QtdLI} LI's", lis.Count);

     try
    {
        // Chama a API da APM para obter os ativos associados às LI's
        var ativosDoLote = await ObterAtivosDoLoteLiAsync(lis);

        if (!ativosDoLote.Any())
        {
            _logger.LogWarning("Nenhum ativo foi retornado para o lote de LI's");
            return 0;
        }

        int registrosProcessados = 0;

        // Para cada LI (Name) e seus ativos (ComponentTypes)
        foreach (var assetComponent in ativosDoLote)
        {
            if (string.IsNullOrWhiteSpace(assetComponent.Name) || assetComponent.ComponentTypes == null)
            {
                _logger.LogWarning("AssetComponent inválido: Name={Name}", assetComponent.Name);
                continue;
            }

            // ← BUSCAR O CONTEXTO ORIGINAL DESSA LI
            var contextoDaLi = liComContexto.FirstOrDefault(li => li.IdLi == assetComponent.Name);

            _logger.LogDebug("Processando {QtdAtivos} ativos da LI {IdLi} (Origem: Nota={IdNota}, Ordem={IdOrdem})", 
                assetComponent.ComponentTypes.Count, 
                assetComponent.Name,
                contextoDaLi.IdNota,
                contextoDaLi.IdOrdem);

            // Para cada ativo associado à LI, inserir/atualizar em DadosRenovacao
            foreach (var idAtivo in assetComponent.ComponentTypes)
            {
                registrosProcessados += await InserirOuAtualizarPorAtivoAsync(
                    idAtivo,
                    idNota: contextoDaLi.IdNota,  // ← USAR DO CONTEXTO
                    idOrdem: contextoDaLi.IdOrdem,  // ← USAR DO CONTEXTO
                    dataSource: contextoDaLi.DataOrigem,  // ← USAR DATA ORIGINAL
                    origem: contextoDaLi.Origem,  // ← USAR ORIGEM ORIGINAL
                    idLi: assetComponent.Name,
                    tipoDados: contextoDaLi.TipoDados);
            }
        }

        _logger.LogInformation("Lote de LI's processado com sucesso. {QtdRegistros} ativos adicionados", 
            registrosProcessados);
        
        return registrosProcessados;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro ao processar lote de LI's");
        throw;
    }
}

/// <summary>
/// Chama a API da APM (PUT) para obter os ativos (ComponentTypes) associados às LI's.
/// </summary>
/// <param name="lis">Lista de LocalInstalacoes (LI's)</param>
/// <returns>Lista de AssetComponentTypesDTO com os ativos para cada LI</returns>
private async Task<List<AssetComponentTypesDTO>> ObterAtivosDoLoteLiAsync(List<string> lis)
{
    try
    {
        var url = $"{_apmBaseUrl}/api/V1/Asset/Components";
        
        _logger.LogInformation("Chamando API APM: PUT {Url} com {QtdLI} LI's no body", url, lis.Count);

        // Criar o body com a lista de nomes
        var jsonContent = System.Text.Json.JsonSerializer.Serialize(lis);  
        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

        // Fazer requisição PUT com body
        var request = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = content
        };
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenService.GenerateToken());
        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Erro ao chamar API APM. StatusCode: {StatusCode}, ReasonPhrase: {ReasonPhrase}", 
                response.StatusCode, 
                response.ReasonPhrase);
            _logger.LogError("Conteudo enviado: {content}", jsonContent);
            response.EnsureSuccessStatusCode();
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        _logger.LogDebug("Resposta recebida da APM: {Response}", responseContent);

        // Desserializar a resposta
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        
        // A resposta pode ser um array ou um único objeto, vamos tratar como array
        List<AssetComponentTypesDTO> resultado;
        
        try
        {
            // Tentar desserializar como array primeiro
            resultado = JsonSerializer.Deserialize<List<AssetComponentTypesDTO>>(responseContent, options) 
                ?? new List<AssetComponentTypesDTO>();
        }
        catch
        {
            // Se não for array, tentar como objeto único
            var singleResult = JsonSerializer.Deserialize<AssetComponentTypesDTO>(responseContent, options);
            resultado = singleResult != null ? new List<AssetComponentTypesDTO> { singleResult } : new List<AssetComponentTypesDTO>();
        }

        _logger.LogInformation("Obtidos {QtdResultados} conjuntos de ativos da API APM", resultado.Count);
        
        return resultado;
    }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, "Erro na requisição HTTP para API APM");
        throw;
    }
    catch (JsonException ex)
    {
        _logger.LogError(ex, "Erro ao desserializar resposta da API APM");
        throw;
    }
}

//Dado o ativo que será restaurado, verifica se ele já existe ou não na tabela DadosRenovacao
    private async Task<int> InserirOuAtualizarPorAtivoAsync(
        ComponentType idAtivo,
        long? idNota,
        long? idOrdem,
        DateTime dataSource,
        OrigemUR origem,
        string idLi,
        TipoDados tipoDados)
    {
        try
        {
            var existente = await _context.PM_DadosRenovacao
                .FirstOrDefaultAsync(d => d.IdAtivo == idAtivo && d.IdLi == idLi);


            //se não existir adiciona
            if (existente == null || dataSource != existente.UltimaRenovacao)
            {
                var novo = new DadosRenovacaoModel
                {
                    UltimaRenovacao = dataSource,
                    OrigemUR = origem,
                    TipoDados = tipoDados,
                    Confiabilidade = 1.0,
                    IdAtivo = idAtivo,
                    IdNota = idNota ?? null,
                    IdOrdem = idOrdem ?? null,
                    IdLi = idLi, 
                    IdUsuario = null
                };
                _context.PM_DadosRenovacao.Add(novo);
                _logger.LogInformation(
                    "Criado novo registro de DadosRenovacao: IdAtivo={IdAtivo}, Origem={Origem}, Data={Data}",
                    idAtivo, origem, dataSource);
                return 1;
            }
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inserir/atualizar DadosRenovacao para IdAtivo={IdAtivo}", idAtivo);
            throw;
        }
    }
    /// <summary>
    /// Mapeia Classificacao + MetodoClassificacao para TipoDados
    /// </summary>
    private TipoDados MapearParaTipoDados(string? classificacao, string? metodoClassificacao)
    {
        // Normalizar valores
        var classif = (classificacao ?? "").ToUpperInvariant().Trim();
        var metodo = (metodoClassificacao ?? "").ToUpperInvariant().Trim();

        // Verificar combinações
        if (classif.Contains("CORRETIVA") || classif.Contains("CORRETIVO"))
        {
            if (metodo.Contains("CATBOOST") || metodo.Contains("IA"))
            {
                return TipoDados.CORRETIVACAT;
            }
            else
            {
                return TipoDados.CORRETIVAHUM;
            }
        }
        else if (classif.Contains("PREVENTIVA") || classif.Contains("PREVENTIVO"))
        {
            if (metodo.Contains("CATBOOST") || metodo.Contains("IA"))
            {
                return TipoDados.PREVENTIVACAT;
            }
            else
            {
                return TipoDados.PREVENTIVAHUM;
            }
        }

        // Fallback padrão
        _logger.LogWarning("Classificacao={Classificacao}, MetodoClassificacao={Metodo} não reconhecidos. Usando padrão NAOCLASSIFICADA",
            classificacao, metodoClassificacao);
        return TipoDados.NAOCLASSIFICADA;
    }
    /// <summary>
    /// Obtém lista de renovações com paginação
    /// </summary>
    /// <param name="busca">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    public async Task<object> BuscarRenovacaoAsync(
        int? pagina,
        int? tamanho)
    {
        var renovacoesQuery = _dadosRenovacao.AsNoTracking().AsQueryable();

        renovacoesQuery = renovacoesQuery.AplicarOrdenacao();
        
        //Cálculos da paginação
        int totalGeral = await renovacoesQuery.CountAsync().ConfigureAwait(false);
        int p = pagina ?? 1;
        int t = tamanho ?? 20;
        int totalPaginas = (int)Math.Ceiling((double)totalGeral / t);

        //Pega os 'n' primeiros itens dependendo da página
        var itensPaginados = renovacoesQuery.Skip((p - 1) * t).Take(t).ToList();

        return new 
        { 
            Total = totalGeral, 
            TotalPaginas = totalPaginas,
            pagina = p, 
            Tamanho = t,
            Result = itensPaginados 
        };
    }

}