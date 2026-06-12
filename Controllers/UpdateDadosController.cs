using app_pm_back.Services.ExternIntegationsServices;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/update-dados")]
public class UpdateDadosController : ControllerBase
{
    private readonly SynchronizeAPMService _synchronizeApmService;
    private readonly ILogger<UpdateDadosController> _logger;
    
    public UpdateDadosController(SynchronizeAPMService synchronizeApmService, ILogger<UpdateDadosController> logger)
    {
        _synchronizeApmService = synchronizeApmService;
        _logger = logger;
    }
    
    [HttpGet("synchronize-apm")]
    public async Task<IActionResult> SynchronizeAPM()
    {
        _logger.LogInformation("Recebida requisição GET em /synchronize-apm.");
        try
        {
            var result = await _synchronizeApmService.SynchronizeAPMAsync();

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
}