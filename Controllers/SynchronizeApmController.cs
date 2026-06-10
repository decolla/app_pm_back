using app_pm_back.Services.ExternIntegationsServices;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/synchronize-apm")]
public class SynchronizeApmController
{
    private readonly SynchronizeAPMService _synchronizeApmService;
    
    public SynchronizeApmController(SynchronizeAPMService synchronizeApmService)
    {
        _synchronizeApmService = synchronizeApmService;
    }
    
    [HttpGet]
    public Task<IActionResult> Synchronize()
    {
        try
        {
            var result= _synchronizeApmService.SynchronizeAPMAsync();

            if (result.IsCompletedSuccessfully)
            {
                return new Task<IActionResult>(() => new OkObjectResult(new { message = "Sincronização concluída com sucesso." }));
            }
            else
            {
                return new Task<IActionResult>(() => new OkObjectResult(new { message = "Sincronização não obteve retorno." }));
            }
        }
        catch (Exception e)
        {
            return Task.FromResult<IActionResult>(new BadRequestObjectResult(new { message = $"Erro ao iniciar sincronização: {e.Message}." }));
        }
    }
}