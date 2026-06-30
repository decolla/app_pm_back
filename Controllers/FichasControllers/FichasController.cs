using app_pm_back.DTO;
using app_pm_back.Models;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using app_pm_back.Services.ExternIntegationsServices;
using app_pm_back.Services.FichasServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers.FichasControllers;

// <summary>
/// Endpoints de Fichas
/// </summary>
[ApiController]
[Route(Config.RouteConfig.BaseRoute + "/fichas")]
public class FichasController : ControllerBase
{
    private readonly SynchronizeAPMService _synchronizeApmService;
    private readonly ILogger<FichasController> _logger;
    private readonly ConfiguracaoGlobalService _configuracaoGlobalService;
    private readonly GenericSearchService _searchService;
    private readonly ClassificationService<NotaModel> _notaService;
    private readonly ClassificationService<OrdemModel> _ordemService;
    private readonly ImportSheetService _spreadsheetOrchestrator;

    public FichasController(SynchronizeAPMService synchronizeApmService, ILogger<FichasController> logger,
        GenericSearchService searchService, 
        ConfiguracaoGlobalService configuracaoGlobalService,
        ClassificationService<NotaModel> notaService,
        ClassificationService<OrdemModel> ordemService, ImportSheetService spreadsheetOrchestrator)

    {
        _configuracaoGlobalService = configuracaoGlobalService;
        _searchService = searchService;
        _synchronizeApmService = synchronizeApmService;
        _logger = logger;
        _notaService = notaService;
        _ordemService = ordemService;
        _spreadsheetOrchestrator = spreadsheetOrchestrator;
    }

    /// <summary>
    /// Sincroniza as fichas do PM com os ativos do APM
    /// </summary>
    [HttpGet("synchronize-apm")]
    public async Task<IActionResult> SynchronizeApm(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recebida requisição GET em /synchronize-apm.");
        try
        {
            var result = await _synchronizeApmService.SynchronizeAPMAsync(cancellationToken);

            if (result)
            {
                _logger.LogInformation("Sincronização executada e alterações salvas no banco de dados.");
                return Ok(new { message = "Sincronização concluída com sucesso." });
            }
            else
            {
                _logger.LogWarning("Sincronização executada, mas não resultou em alterações no banco de dados.");
                return Ok(new { message = "Sincronização não obteve retorno." });
            }
        }
        catch (TimeoutException ex)
        {
            _logger.LogError(ex, "Timeout ocorrido ao sincronizar APM.");
            return StatusCode(504, new { message = $"Sincronização falhou devido a timeout: {ex.Message}." });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao processar sincronização.");
            return BadRequest(new { message = $"Erro ao iniciar sincronização: {e.Message}." });
        }
    }

    /// <summary>
    /// Cria uma regra de classificação por palavras para Ambas, Fichas ou Ordens
    /// </summary>
    /// <param name="regra">Regra a ser aplicada</param>
    /// <returns>Regra criada</returns>
    [HttpPost("classificacao/regras")]
    public async Task<IActionResult> CriarRegraCongas([FromBody] RuleClassificationDto regra,
        [FromQuery] Fichas buscaTabela)
    {
        try
        {
            var regrasCriadas = new List<RuleClassificationDto>();
            var message = "";
            if (buscaTabela == Fichas.Ambas || buscaTabela == Fichas.Notas)
            {
                var resultNota = await _notaService.CreateRule(regra);
                regrasCriadas.Add(resultNota);
                message = "Notas classificadas com sucesso";
            }

            if (buscaTabela == Fichas.Ambas || buscaTabela == Fichas.Ordens)
            {
                var resultOrdem = await _ordemService.CreateRule(regra);
                regrasCriadas.Add(resultOrdem);
                message += "| Ordens classificadas com sucesso";
            }

            if (!regrasCriadas.Any())
            {
                return BadRequest(new { message = $"Resultado da busca nulo" });
            }

            var idsGerados = string.Join(", ", regrasCriadas.Select(r => r.IdRegra));

            return Ok(new { message = $"{message} ! IDs: {idsGerados}" });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar regra {e.Message}" });
        }
    }

    /// <summary>
    /// Classifica para Ambas, Fichas ou Ordens com base uma lista de palavras armazenadas no banco
    /// </summary>
    /// <returns>Lista de notas atualizadas</returns>
    [HttpGet("classificacao/filtro")]
    public async Task<IActionResult> ClassificacaoPalavrasNotaCongas(Fichas buscaTabela)
    {
        var config = await _configuracaoGlobalService.ObterConfiguracaoAsync();
        if (config.ClassificacaoAutomatica != ModoClassificacaoAutomatica.ClassificarPalavrasChave)
        {
            return BadRequest(new { message = "Classificação por palavras desativada nas configurações globais." });
        }
        
        try
        {
            // passar uma lista vazia por enquanto 
            var message = "";
            List<NotaModel>? notasClassificadas = null;
            List<OrdemModel>? ordensClassificadas = null;
            if (buscaTabela == Fichas.Ambas || buscaTabela == Fichas.Notas)
            {
                notasClassificadas = await _notaService.WordsClassification([]);
                message = "Notas classificadas com sucesso";
            }

            if (buscaTabela == Fichas.Ambas || buscaTabela == Fichas.Ordens)
            {
                ordensClassificadas = await _ordemService.WordsClassification([]);
                message += "| Ordens classificadas com sucesso";
            }

            if (!message.Any())
            {
                return BadRequest(new { message = "Ficha inválida" });
            }

            return Ok(new { message = $"{message} !" });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao classificar notas: {e.Message}" });
        }
    }

    /// <summary>
    /// Classifica as Ambas, Fichas ou Ordens usando a IA catboost
    /// </summary>
    [HttpGet("classificacao/catboost")]
    public async Task<IActionResult> ClassificacaoCatboostNota(Fichas buscaTabela)
    {
        var config = await _configuracaoGlobalService.ObterConfiguracaoAsync();
        if (config.ClassificacaoAutomatica != ModoClassificacaoAutomatica.ClassificarCatBoost)
        {
            return BadRequest(new { message = "Classificação por catboost desativada nas configurações globais." });
        }
        
        try
        {
            var message = "";
            List<NotaModel>? notasClassificadas = null;
            List<OrdemModel>? ordensClassificadas = null;
            if (buscaTabela == Fichas.Ambas || buscaTabela == Fichas.Notas)
            {
                notasClassificadas = await _notaService.CatboostClassification();
                message = "Notas classificadas com sucesso";
            }

            if (buscaTabela == Fichas.Ambas || buscaTabela == Fichas.Ordens)
            {
                ordensClassificadas = await _ordemService.CatboostClassification();
                message += "| Ordens classificadas com sucesso";
            }

            if (!message.Any())
            {
                return BadRequest(new { message = "Ficha inválida" });
            }

            return Ok(new { message = $"{message} !" });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao classificar notas: {e.Message}" });
        }
    }

    /// <summary>
    /// Faz o upload e processamento de planilhas de Notas, Ordens ou Itens
    /// </summary>
    [HttpPost("upload")]
    public async Task<IActionResult> UploadPlanilha(
        IFormFile file,
        [FromQuery] bool sobrescrever,
        [FromQuery] bool adicionarNovas,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validações básicas e rápidas da requisição
            if (!sobrescrever && !adicionarNovas)
                return BadRequest(new
                    { message = "Nenhuma ação selecionada. Marque 'sobrescrever' ou 'adicionarNovas'." });

            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Arquivo inválido ou vazio." });

            // 3. Toda a mágica pesada acontece dentro do serviço!
            var inseridas = await _spreadsheetOrchestrator
                .ProcessarPlanilhaAsync(file, sobrescrever, adicionarNovas, cancellationToken);

            return Ok(new { message = inseridas });
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Upload cancelado pelo cliente.");
            return StatusCode(499, new { message = "Operação cancelada pelo cliente." });
        }
        catch (ArgumentException ex)
        {
            // erros de negócio (ex: schema cg, formato inválido)
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no upload da planilha.");
            return BadRequest(new { message = $"Erro ao processar planilha: {ex.Message}" });
        }
    }

    /// <summary>
    /// Busca notas ou ordens de manutenção com filtros no body, retorna uma lista de objetos com os dados relevantes
    /// </summary>
    /// <param name="busca">Modelo de busca com campos comparáveis</param>
    /// <param name="pagina">Página a ser exibida</param>
    /// <param name="tamanho">Número de elementos presente na página</param>
    /// <returns>Lista de Nota e Ordens com base nos filtros</returns>
    [HttpPost("busca")]
    public async Task<IActionResult> BuscarNotaOrdem(
        [FromBody] BuscaOrdemNotaDto busca,
        [FromQuery] int? pagina = null,
        [FromQuery] int? tamanho = null,
        [FromQuery] Fichas? tFichas = Fichas.Ambas)
    {
        try
        {
            if ((pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0))
            {
                return BadRequest(new { message = "Os parametros pagina e tamanho devem ser maiores que zero." });
            }

            var resultados = await _searchService.BuscarNotaOrdemAsync(
                busca,
                pagina,
                tamanho,
                tFichas);

            return Ok(resultados);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar nota/ordem: {e.Message}." });
        }
    }
}