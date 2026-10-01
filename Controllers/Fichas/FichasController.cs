using app_pm_back.DTO;
using app_pm_back.DTO.Buscas;
using app_pm_back.Models;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using app_pm_back.Services.ExternIntegationsServices;
using app_pm_back.Services.FichasServices;
using app_pm_back.Services.GenericServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers.FichasControllers;

// <summary>
/// Endpoints de Fichas
/// </summary>
[ApiController]
[Authorize(Policy = "View")]
[Route(Config.RouteConfig.BaseRoute + "/fichas")]
public class FichasController : ControllerBase
{
    private readonly SynchronizeAPMService _synchronizeApmService;
    private readonly ILogger<FichasController> _logger;
    private readonly GenericSearchService _searchService;
    private readonly ClassificationService<NotaModel> _notaService;
    private readonly ClassificationService<OrdemModel> _ordemService;
    private readonly ImportSheetService _spreadsheetOrchestrator;
    private readonly DashboardService _dashboardService;

    public FichasController(SynchronizeAPMService synchronizeApmService, ILogger<FichasController> logger,
        GenericSearchService searchService,
        ClassificationService<NotaModel> notaService,
        ClassificationService<OrdemModel> ordemService, ImportSheetService spreadsheetOrchestrator,
        DashboardService dashboardService)

    {
        _searchService = searchService;
        _synchronizeApmService = synchronizeApmService;
        _logger = logger;
        _notaService = notaService;
        _ordemService = ordemService;
        _spreadsheetOrchestrator = spreadsheetOrchestrator;
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Sincroniza as fichas do PM com os ativos do APM
    /// </summary>
    [HttpGet("synchronize-apm")]
    [Authorize(Policy = "Edit")]
    public async Task<IActionResult> SynchronizeApm(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recebida requisição GET em /synchronize-apm.");
        try
        {
            var result = await _synchronizeApmService.SynchronizeApmAsync(cancellationToken);

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
    /// Faz o upload e processamento de planilhas de Notas, Ordens ou Itens
    /// </summary>
    [HttpPost("upload")]
    [Authorize(Policy = "Edit")]
    [DisableRequestSizeLimit]
    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    public async Task<IActionResult> UploadPlanilha(
        IFormFile file,
        [FromQuery] bool sobrescrever,
        [FromQuery] bool adicionarNovas,
        [FromQuery] TipoUpload tipoUpload,
        CancellationToken cancellationToken)
    {
        try
        {
            // validações
            if (!sobrescrever && !adicionarNovas)
                return BadRequest(new
                    { message = "Nenhuma ação selecionada. Marque 'sobrescrever' ou 'adicionarNovas'." });

            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Arquivo inválido ou vazio." });

            var inseridas = await _spreadsheetOrchestrator
                .ProcessarPlanilhaAsync(file, sobrescrever, adicionarNovas, tipoUpload, cancellationToken);

            if (inseridas > 0)
            {
                await _notaService.AutoClassify();
                await _ordemService.AutoClassify();
            }

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
        [FromQuery] int pagina,
        [FromQuery] int tamanho,
        [FromQuery] bool? apenasNaoAtribuidas,
        [FromQuery] Fichas? tFichas = Fichas.Ambas)
    {
        try
        {
            if ((pagina <= 0) || (tamanho <= 0))
            {
                return BadRequest(new { message = "Os parametros pagina e tamanho devem ser maiores que zero." });
            }

            var resultados = await _searchService.BuscarNotaOrdemAsync(
                busca,
                pagina,
                tamanho,
                tFichas,
                apenasNaoAtribuidas ?? false);

            return Ok(resultados);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar nota/ordem: {e.Message}." });
        }
    }

    /// <summary>
    /// Busca fichas por dados de renovação
    /// </summary>
    /// <param name="busca">Modelo de busca com campos comparáveis</param>
    /// <param name="pagina">Página a ser exibida</param>
    /// <param name="tamanho">Número de elementos presente na página</param>
    /// <returns>Lista de fichas com base nos filtros</returns>
    /// <response code="200">Retorna a lista de fichas</response>
    /// <response code="400">Retorna um erro se os parâmetros forem inválidos</response>
    [HttpPost("busca/data-renovacao")]
    public async Task<IActionResult> BuscarFichasDataRenovacao(
        [FromBody] BuscaFichasDadoRenovacaoDto busca,
        [FromQuery] int? pagina = null,
        [FromQuery] int? tamanho = null)
    {
        try
        {
            if ((pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0))
            {
                return BadRequest(new { message = "Os parametros pagina e tamanho devem ser maiores que zero." });
            }

            var resultados = await _searchService.BuscarFichasDataRenovacaoAsync(
                busca,
                pagina,
                tamanho);

            return Ok(resultados);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar fichas por dados de renovação: {e.Message}." });
        }
    }
    
    /// <summary>
    /// Extrai os dados necessários para o Dashboard
    /// </summary>
    /// <param name="filtro">Filtro a ser aplicado nas fichas</param>
    /// <returns>Estatísticas do dashboard</returns>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(
        [FromQuery] FiltroDashboardDTO filtro)
    {
        try
        {
            var resultados = await _dashboardService.GetDashboardDataAsync(filtro);

            return Ok(resultados);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar dashboard: {e.Message}." });
        }
    }
}