using app_pm_back.Services;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers.FichasControllers;

[ApiController]
[Route(Config.RouteConfig.BaseRoute + "/diretorios")]
public class DiretoriosController : ControllerBase
{
    private readonly DiretorioService _diretorioService;

    public DiretoriosController(DiretorioService diretorioService)
    {
        _diretorioService = diretorioService;
    }
    
    [HttpGet]
    [Route("arvore")]
    public async Task<IActionResult> ObterArvore()
    {
        try
        {
            var arvore = await _diretorioService.ObterArvoreAsync();
            return Ok(arvore);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro ao obter a árvore de diretórios.", details = ex.Message });
        }
    }
}