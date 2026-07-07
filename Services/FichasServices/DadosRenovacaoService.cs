using System.Net.Http.Headers;
using System.Text.Json;
using app_pm_back.Data;
using app_pm_back.Services.Extensions;
using app_pm_back.Models.Domain;
using app_pm_back.DTO.Restauracao;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.FichasServices;

/// <summary>
/// Classe DadosRenovacaoService.
/// </summary>
public class DadosRenovacaoService : BaseCrudService<DadosRenovacaoModel>
{
    private readonly ILogger<DadosRenovacaoService> _logger;

    private List<(string IdLi, long? IdNota, long? IdOrdem, DateTime DataOrigem, OrigemUR Origem, Classificacao
        TipoDados)> _liParaRestaurar
        = new List<(string, long?, long?, DateTime, OrigemUR, Classificacao)>();

    private const int TAMANHO_LOTE_LI = 50;
    private readonly HttpClient _httpClient;
    private readonly DbSet<PlanoModel> _planoModels;
    private readonly ConfiguracaoGlobalService _configuracaoGlobalService;
    private readonly string _apmBaseUrl;


    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="DadosRenovacaoService"/>.
    /// </summary>
    public DadosRenovacaoService(
        AppDbContext context,
        ILogger<DadosRenovacaoService> logger,
        IConfiguration configuration,
        ConfiguracaoGlobalService configuracaoGlobalService,
        HttpClient httpClient)
        : base(context)
    {
        _logger = logger;
        _httpClient = httpClient;
        _planoModels = context.PM_Planos;
        _configuracaoGlobalService = configuracaoGlobalService;
        _apmBaseUrl = Config.RouteConfig.GetApmBaseUrl(configuration);
    }

    /// <summary>
    /// Importa, analisa e atualiza o status de confiabilidade restaurada para um lote de Notas de Manutenção com base em seus itens e textos.
    /// </summary>
    /// <param name="idsNotas">Lista de identificadores das Notas a serem processadas</param>
    /// <param name="idsItensNota">Lista de identificadores dos Itens associados a essas notas</param>
    /// <param name="cancellationToken">Token de cancelamento para interromper a operação assíncrona caso necessário</param>
    /// <returns>A quantidade de registros atualizados e cadastrados na tabela de dados de renovação</returns>
    /// <exception cref="System.Exception">Lançada em caso de erro na comunicação com o banco de dados ou regras de negócio</exception>
    public virtual async Task<int> ImportarDadosNotaAsync(
        List<long> idsNotas,
        List<long> idsItensNota,
        CancellationToken cancellationToken)
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
        //Repassando o cancellationToken para os métodos assíncronos
        var notas = await _context.PM_Notas
            .Where(n => idsNotas.Contains(n.IdNota))
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Carregadas {QtdNotas} notas do banco", notas.Count);

        var itens = (idsItensNota == null || idsItensNota.Count == 0)
            ? new List<ItemNotaModel>()
            : await _context.PM_ItensNota
                .Where(i => idsItensNota.Contains(i.IdItemNota))
                .ToListAsync(cancellationToken);

        _logger.LogInformation("Carregados {QtdItens} itens do banco", itens.Count);

        //Transforma em um HashSet de Ids para busca ultrarrápida O(1)
        var notasComItensRenovacao = new HashSet<long>(
            itens.Where(i => ContemPalavraChaveItem(i.Causa, i.Sintoma)).Select(i => i.IdNota)
        );

        if (notasComItensRenovacao.Count == 0)
            _logger.LogWarning("Nenhuma nota com palavras-chave de renovação encontrada relacionada aos itens");
        else
            _logger.LogInformation("Identificadas {QtdNotas} notas com itens de renovação", notasComItensRenovacao.Count);

        // Arrays de palavras-chave (mantidos fora do loop para não recriar na memória)
        var palavrasRenovacao = new[] { "START", "RENOVAÇÃO", "RENOVACAO" };
        var palavrasTroca = new[] { "TROCA", "SUBSTITUIÇÃO", "SUBSTITUICAO" };

        //Um ÚNICO loop para verifcar confiabilidade
        foreach (var nota in notas)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var textosNota = $"{nota.DescricaoBreve} {nota.AnomaliaObservada} {nota.ObservacoesComplementares}".ToUpperInvariant();
            
            bool temRenovacao = palavrasRenovacao.Any(p => textosNota.Contains(p));
            bool temTroca = palavrasTroca.Any(p => textosNota.Contains(p));

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
            else if (notasComItensRenovacao.Contains(nota.IdNota))
            {
                // Se as regras de texto não pegaram, cai na regra de item (fallback)
                if (nota.IdAtivo.HasValue)
                {
                    nota.ConfiabilidadeRestaurada = ConfiabilidadeRestaurada.RestauraAtivo;
                    _logger.LogDebug("Nota {IdNota} marcada como RestauraAtivo pelo Item (IdAtivo={IdAtivo})", nota.IdNota, nota.IdAtivo);
                }
                else if (!string.IsNullOrWhiteSpace(nota.IdLi))
                {
                    nota.ConfiabilidadeRestaurada = ConfiabilidadeRestaurada.RestauraLI;
                    _logger.LogDebug("Nota {IdNota} marcada como RestauraLI pelo Item (IdLI={IdLI})", nota.IdNota, nota.IdLi);
                }
            }
        }

        //CancellationToken repassado
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Alterações salvas no banco de dados");
        
        var resultado = await CadastraDadosRenovacaoAsync(idsNotas, ehNota: true);
        
        _logger.LogInformation("ImportarDadosNotaAsync concluído com {QtdRegistros} registros criados/atualizados", resultado);

        return resultado;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro ao processar notas em ImportarDadosNotaAsync");
        throw; // Esse throw propaga a exceção respeitando sua declaração XML original
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
    public virtual async Task<int> ImportarDadosOrdemAsync(
        List<long> idsOrdens, CancellationToken cancellationToken)
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


            // Não filtrar por ConfiabilidadeRestaurada antes da classificação por descrição de texto, 
            // caso contrário ordens com NaoRestaura nunca seriam classificadas/processadas.


            _logger.LogInformation("Carregadas {QtdOrdens} ordens do banco", ordens.Count);

            //Classifica as ordens baseado em DescricaoBreve
            var palavrasRenovacao = new[] { "TROCA", "START", "RENOVAÇÃO", "RENOVACAO" };
            var palavrasTroca = new[] { "SUBSTITUIÇÃO", "SUBSTITUICAO" };

            foreach (var ordem in ordens)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var textoOrdem = (ordem.DescricaoBreve ?? string.Empty).ToUpperInvariant();

                bool temRenovacao = palavrasRenovacao.Any(p => textoOrdem.Contains(p));
                bool temTroca = palavrasTroca.Any(p => textoOrdem.Contains(p));
                if (ordem.ConfiabilidadeRestaurada == null ||
                    ordem.ConfiabilidadeRestaurada == ConfiabilidadeRestaurada.NaoRestaura)
                {
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
    /// Cadastra os dados de renovação em lote com base em uma lista de IDs de Notas ou Ordens de Manutenção.
    /// </summary>
    /// <param name="ids">Lista de identificadores exclusivos das Notas ou Ordens</param>
    /// <param name="ehNota">Sinalizador lógico indicando o tipo de entidade (true se for Nota, false se for Ordem)</param>
    /// <returns>A quantidade total de registros criados ou modificados e persistidos no banco de dados</returns>
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
        //Uma estrutura de dados comum para evitar duplicar toda a lógica de loops e lotes
        var itensParaProcessar = new List<(ConfiabilidadeRestaurada? Conf, ComponentType? Ativo, long? NotaId, long? OrdemId, DateTime Data, OrigemUR Origem, string Li, Classificacao Classif)>();

        //Chunk para AMBOS os cenários, garantindo segurança contra limites do banco de dados
        foreach (var loteIds in ids.Chunk(2000))
        {
            if (ehNota)
            {
                var notasLote = await _context.PM_Notas
                    .Where(n => loteIds.Contains(n.IdNota) && n.ConfiabilidadeRestaurada != ConfiabilidadeRestaurada.NaoRestaura)
                    .ToListAsync();

                itensParaProcessar.AddRange(notasLote.Select(n => (
                    n.ConfiabilidadeRestaurada, n.IdAtivo, (long?)n.IdNota, (long?)null, n.DataNota ?? DateTime.UtcNow, OrigemUR.Nota, n.IdLi ?? string.Empty, n.Classificacao
                )));
            }
            else
            {
                var ordensLote = await _context.PM_Ordens
                    .Where(o => loteIds.Contains(o.IdOrdem))
                    .ToListAsync();

                itensParaProcessar.AddRange(ordensLote.Select(o => (
                    o.ConfiabilidadeRestaurada, o.IdAtivo, (long?)null, (long?)o.IdOrdem, o.DataCriacao ?? DateTime.UtcNow, OrigemUR.Ordem, o.IdLi ?? string.Empty, o.Classificacao
                )));
            }
        }

        _logger.LogInformation("Carregados {Qtd} registros de {Tipo} para processamento", itensParaProcessar.Count, ehNota ? "Notas" : "Ordens");
        
        //processa ordens e notas
        foreach (var item in itensParaProcessar)
        {
            _logger.LogDebug("Processando item. Nota={IdNota}, Ordem={IdOrdem}, Confiabilidade={Conf}", item.NotaId, item.OrdemId, item.Conf);

            registrosCriados += await ProcessarPorConfiabilidadeAsync(
                item.Conf, item.Ativo, item.NotaId, item.OrdemId, item.Data, item.Origem, item.Li, item.Classif
            );
            
            // Processa o lote inteiro acumulado, limpamos a fila e evitamos bugs de deslocamento de índice.
            if (_liParaRestaurar.Count >= TAMANHO_LOTE_LI)
            {
                _logger.LogInformation("Atingido tamanho de lote ({TamanhoLote}). Processando lote de LI's...", TAMANHO_LOTE_LI);
                
                var lotePorProcessar = _liParaRestaurar
                    .DistinctBy(li => li.IdLi) 
                    .ToList();

                registrosCriados += await ProcessarLoteLiAsync(lotePorProcessar);
                _liParaRestaurar.Clear(); 
            }
        }

        // Processa qualquer LI remanescente que sobrou na fila
        if (_liParaRestaurar.Any())
        {
            _logger.LogInformation("Processando {QtdLI} LI's restantes", _liParaRestaurar.Count);
            var restantes = _liParaRestaurar.DistinctBy(li => li.IdLi).ToList();
            registrosCriados += await ProcessarLoteLiAsync(restantes);
            _liParaRestaurar.Clear();
        }

        // Finaliza persistindo as alterações geradas pelos métodos internos
        if (registrosCriados > 0)
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Salvos {QtdRegistros} registros de dados de renovação com sucesso", registrosCriados);
        }
        else
        {
            _logger.LogWarning("Nenhum registro de dados de renovação foi criado ou atualizado");
        }

        return registrosCriados;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro ao cadastrar dados de renovação para {Tipo}", ehNota ? "Nota" : "Ordem");
        throw;
    }
    }

    /// <summary>
    /// Processa a atualização de registros com base no tipo de confiabilidade restaurada (RestauraAtivo ou RestauraLI).
    /// </summary>
    /// <param name="confiabilidade">O nível de restauração de confiabilidade a ser aplicado</param>
    /// <param name="idAtivo">Identificador opcional do ativo (ComponentType)</param>
    /// <param name="idNota">Identificador opcional da Nota associada</param>
    /// <param name="idOrdem">Identificador opcional da Ordem associada</param>
    /// <param name="dataSource">Data de origem da ocorrência</param>
    /// <param name="origem">Origem do apontamento (OrigemUR)</param>
    /// <param name="idLi">Identificador do Local de Instalação (LI)</param>
    /// <param name="tipoDados">Classificação do tipo de dado (Corretiva, Preventiva, etc.)</param>
    /// <returns>A quantidade de registros inseridos ou atualizados imediatamente</returns>
    private async Task<int> ProcessarPorConfiabilidadeAsync(
        ConfiabilidadeRestaurada? confiabilidade,
        ComponentType? idAtivo,
        long? idNota,
        long? idOrdem,
        DateTime dataSource,
        OrigemUR origem,
        string idLi,
        Classificacao tipoDados)
    {
        if (confiabilidade == null)
    {
        _logger.LogDebug("ConfiabilidadeRestaurada é nula, nenhum processamento realizado");
        return 0;
    }

    var inseridos = 0;

    if (confiabilidade == ConfiabilidadeRestaurada.RestauraAtivo)
    {
        // OTIMIZAÇÃO: Removemos o if/else redundante. 
        // O método já aceita idAtivo nulo, então repassamos o valor diretamente.
        if (idAtivo.HasValue)
            _logger.LogDebug("Processando RestauraAtivo para IdAtivo={IdAtivo}", idAtivo.Value);
        else
            _logger.LogWarning("RestauraAtivo com IdAtivo nulo. Gravando com IdAtivo = null na LI={IdLi}", idLi);

        inseridos = await InserirOuAtualizarPorAtivoAsync(
            idAtivo: idAtivo, // ← Passa o parâmetro anulável direto
            idNota,
            idOrdem,
            dataSource,
            origem,
            idLi,
            tipoDados
        );
    }
    else if (confiabilidade == ConfiabilidadeRestaurada.RestauraLI)
    {
        if (!string.IsNullOrWhiteSpace(idLi))
        {
            _logger.LogDebug("RestauraLI identificado - adicionando IdLi={IdLi} à fila de processamento", idLi);

            // OTIMIZAÇÃO: Uso de .Any() em vez de FirstOrDefault para checagem booleana
            bool jaExisteNaFila = _liParaRestaurar.Any(li => li.IdLi == idLi);

            if (!jaExisteNaFila)
            {
                _liParaRestaurar.Add((idLi, idNota, idOrdem, dataSource, origem, tipoDados));
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

    /// <summary>
    /// Processa um lote de Locais de Instalação (LIs), busca seus ativos associados via API APM e registra as atualizações no banco de dados.
    /// </summary>
    /// <param name="liComContexto">Lista de tuplas contendo o ID do LI e seu contexto operacional (Nota, Ordem, DataOrigem, Origem e TipoDados)</param>
    /// <returns>A quantidade total de registros processados e salvos no banco de dados</returns>
    private async Task<int> ProcessarLoteLiAsync(
        List<(string IdLi, long? IdNota, long? IdOrdem, DateTime DataOrigem, OrigemUR Origem, Classificacao TipoDados)>
            liComContexto)
    {
        if (!liComContexto.Any())
        {
            _logger.LogDebug("Lista de LI's vazia, nenhum processamento realizado");
            return 0;
        }

        //Extrai apenas LIs únicos, caso haja LIs repetidos no lote
        var lis = liComContexto.Select(li => li.IdLi).Distinct().ToList();
        _logger.LogInformation("Iniciando processamento de lote com {QtdLI} LI's únicas", lis.Count);

        //Cria um dicionário para busca instantânea (O(1)). 
        // O GroupBy garante que não dê erro caso o liComContexto tenha IDs duplicados.
        var contextoDict = liComContexto
            .GroupBy(li => li.IdLi)
            .ToDictionary(g => g.Key, g => g.First());
        
        try
    {
        var ativosDoLote = await ObterAtivosDoLoteLiAsync(lis);

        if (!ativosDoLote.Any())
        {
            _logger.LogWarning("Nenhum ativo foi retornado para o lote de LI's");
            return 0;
        }

        int registrosProcessados = 0;

        foreach (var assetComponent in ativosDoLote)
        {
            if (string.IsNullOrWhiteSpace(assetComponent.Name))
            {
                _logger.LogWarning("AssetComponent sem nome retornado pela APM.");
                continue;
            }

            // Busca o contexto pelo Dicionário
            if (!contextoDict.TryGetValue(assetComponent.Name, out var contextoDaLi))
            {
                _logger.LogWarning("LI {IdLi} retornada pela APM não estava no contexto original.", assetComponent.Name);
                continue;
            }

            // Unifica a lógica para não repetir o InserirOuAtualizarPorAtivoAsync duas vezes.
            var idsParaProcessar = (assetComponent.ComponentTypes == null || !assetComponent.ComponentTypes.Any())
                ? new List<dynamic> { null! } 
                : assetComponent.ComponentTypes.Cast<dynamic>().ToList();

            if (idsParaProcessar.Count == 1 && idsParaProcessar[0] == null)
            {
                _logger.LogDebug("LI {IdLi} sem ativos. Inserindo registro com IdAtivo nulo.", assetComponent.Name);
            }

            // O laço vai rodar 1 vez se for nulo, ou N vezes para os ativos que existirem
            foreach (var idAtivo in idsParaProcessar)
            {
                registrosProcessados += await InserirOuAtualizarPorAtivoAsync(
                    idAtivo: idAtivo,
                    idNota: contextoDaLi.IdNota,
                    idOrdem: contextoDaLi.IdOrdem,
                    dataSource: contextoDaLi.DataOrigem,
                    origem: contextoDaLi.Origem,
                    idLi: assetComponent.Name,
                    tipoDados: contextoDaLi.TipoDados
                );
            }
        }

        _logger.LogInformation("Lote de LI's processado com sucesso. {QtdRegistros} registros gravados/atualizados",
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

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenService.GenerateToken());
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
                resultado = singleResult != null
                    ? new List<AssetComponentTypesDTO> { singleResult }
                    : new List<AssetComponentTypesDTO>();
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

    /// <summary>
    /// Dado o ativo que será restaurado, verifica se ele já existe ou não na tabela DadosRenovacao
    /// </summary>
    /// <returns>Inteiro representando sucesso ou falha da operação</returns>
    private async Task<int> InserirOuAtualizarPorAtivoAsync(
        ComponentType? idAtivo,
        long? idNota,
        long? idOrdem,
        DateTime dataSource,
        OrigemUR origem,
        string idLi,
        Classificacao tipoDados)
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
    /// Obtém lista de renovações com paginação
    /// </summary>
    /// <param name="busca">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    public async Task<object> BuscarRenovacaoAsync(
        int? pagina,
        int? tamanho)
    {
        var renovacoesQuery = _dbSet.AsQueryable();

        renovacoesQuery = renovacoesQuery.AplicarOrdenacao(_context);

        //Cálculos da paginação
        int totalGeral = await renovacoesQuery.CountAsync().ConfigureAwait(false);
        int p = pagina ?? 1;
        int t = tamanho ?? 20;
        int totalPaginas = (int)Math.Ceiling((double)totalGeral / t);

        //Pega os 'n' primeiros itens dependendo da página
        var itensPaginados = await renovacoesQuery
            .Skip((p - 1) * t)
            .Take(t)
            .ToListAsync()
            .ConfigureAwait(false);

        return new
        {
            Total = totalGeral,
            TotalPaginas = totalPaginas,
            pagina = p,
            Tamanho = t,
            Result = itensPaginados
        };
    }

    /// <summary>
    /// A partir de um LI, busca a última data de renovação e as próximas renovações definidas em um plano
    /// <param name="idLi">Identificador principal do Local de Instalação (LI)</param>
    /// <param name="nomAlternativo">Nome alternativo opcional associado ao LI</param>
    /// <returns>Objeto anônimo contendo o nome SAP (LIs agrupados), data da última renovação, lista de renovação e tempo de ciclo</returns>
    /// </summary>
    public async Task<object?> AtivoDatas(string idLi, string? nomeAlternativo)
    {
        //Monta a lista com os nomes de busca
        var nomesBusca = new List<string> { idLi };
        if (!string.IsNullOrWhiteSpace(nomeAlternativo))
        {
            nomesBusca.Add(nomeAlternativo);
        }

        //Busca a data da última renovação verificando TODOS os nomes
        var queryRenovacao = _context.PM_DadosRenovacao
            .AsNoTracking()
            .Where(d => nomesBusca.Contains(d.IdLi));

        var config = await _configuracaoGlobalService.ObterConfiguracaoAsync();

        if (config.CriterioDataRenovacao == CriterioDataRenovacao.ApenasFichasCorretivas)
            queryRenovacao = queryRenovacao.Where(d => d.TipoDados == Classificacao.CORRETIVA);
        else if (config.CriterioDataRenovacao == CriterioDataRenovacao.ApenasFichasPreventivas)
            queryRenovacao = queryRenovacao.Where(d => d.TipoDados == Classificacao.PREVENTIVA);

        // Traz APENAS a data do banco, economizando tráfego de dados (não traz a linha inteira)
        var ultimaData = await queryRenovacao
            .OrderByDescending(d => d.UltimaRenovacao)
            .Select(d => (DateTime?)d.UltimaRenovacao)
            .FirstOrDefaultAsync();

        // Busca TODOS os planos relacionados a qualquer um dos LIs
        var planos = await _planoModels
            .Include(p => p.Estrategia)
            .Where(p => nomesBusca.Contains(p.IdLi))
            .ToListAsync();

        // Se nenhum plano for encontrado, retorna o fallback
        if (!planos.Any())
        {
            return new
            {
                SapName = string.Join(" / ", nomesBusca),
                UltimaData = ultimaData,
                ListaRenovacao = new List<object>(),
                TempoCiclo = 120
            };
        }
        
        //se existe plano, busca ele
        List<long> idsPlanos = planos
            .Select(p => p.IdPlano)
            .ToList();

        // Busca apenas UM TipoPlano
        var tipoPlanoRenovacao = await _context.PM_TiposPlano
            .Include(tp => tp.TipoManutencao)
            .AsNoTracking()
            .Where(tp => tp.IdPlano != null &&
                         idsPlanos.Contains(tp.IdPlano.Value) &&
                         tp.Categoria == CategoriaTipoManutencao.Renovacao)
            .FirstOrDefaultAsync();

        var listaRenovacao = tipoPlanoRenovacao != null
            ? new List<object>
            {
                new
                {
                    Dias = tipoPlanoRenovacao.Dias,
                    Continua = tipoPlanoRenovacao.Continua,
                    Confiabilidade = tipoPlanoRenovacao.TipoManutencao?.Confiabilidade
                }
            }
            : new List<object>();

        // Extrai o tempo de ciclo
        var tempoCiclo = planos
            .Where(p => p.Estrategia?.Ciclos != null)
            .Select(p => p.Estrategia!.Ciclos)
            .FirstOrDefault();

        return new
        {
            SapName = string.Join(" / ", nomesBusca),
            UltimaData = ultimaData,
            ListaRenovacao = listaRenovacao,
            TempoCiclo = tempoCiclo
        };
    }
}