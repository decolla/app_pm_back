using app_pm_back.Services.Manutencao;
using app_pm_back.DTO.Restauracao;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/restauracao-externa")]
public class LinhaTemporalController : ControllerBase
{
    private readonly LinhaTemporalService _linhaTemporalService;

    public LinhaTemporalController(LinhaTemporalService linhaTemporalService)
    {
        _linhaTemporalService = linhaTemporalService;
    }

    /// <summary>
    /// Obtém a linha temporal das LIs consultando API externa e cruzando com notas/ordens locais
    /// </summary>
    /// <param name="request">Lista de IDs das LIs (Localizações de Instalação)</param>
    /// <returns>Lista de itens da linha temporal com eventos (notas/ordens)</returns>
    [HttpPost("linha-temporal")]
    [ProducesResponseType(typeof(List<LinhaTemporalDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterLinhaTemporal([FromBody] LinhaTemporalRequestDTO request)
    {
        if (request?.IdsLi == null || request.IdsLi.Count == 0)
        {
            return BadRequest("Lista de IDs das LIs é obrigatória");
        }

        try
        {
            var resultado = await _linhaTemporalService.ObterLinhaTemporalAsync(request.IdsLi);
            
            return Ok(resultado);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, 
                $"Erro ao comunicar com serviço externo: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                $"Erro interno: {ex.Message}");
        }
    }
}