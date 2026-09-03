using app_pm_back.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Controllers;

[ApiController]
[Authorize(Policy = "View")]
[Route(Config.RouteConfig.BaseRoute + "/auditoria")]
public class AuditoriaController : ControllerBase
{
    private readonly AuditoriaService _auditoriaService;
    
    public AuditoriaController(AuditoriaService auditoriaService)
    {
        _auditoriaService = auditoriaService;
    }
    
    /// <summary>
    /// Lista os registros cadastrados com paginação.
    /// </summary>
    /// <param name="pagina">Número da página (1-based) para paginação.</param>
    /// <param name="tamanho">Quantidade de itens por página.</param>
    /// <returns>Uma lista de registros cadastrados ou um objeto paginado contendo total e resultados.</returns>
    /// <response code="200">Sucesso ao listar registros.</response>
    /// <response code="400">Parâmetros de paginação inválidos (menores ou iguais a zero).</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BuscarPorCampo(
        [FromQuery] long? usuario,
        [FromQuery] EntityState? acao, 
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanho = 20)
    {
        try
        {
            if (TemPaginacaoInvalida(pagina, tamanho))
            {
                return BadRequest(new { message = "Os parametros pagina e tamanho devem ser maiores que zero." });
            }

            var resultados = await _auditoriaService.SearchAuditoria(
                usuario,
                acao,
                dataInicio,
                dataFim,
                pagina,
                tamanho);

            return Ok(resultados);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar o histórico: {e.Message}." });
        }
    }
    
    /// <summary>
    /// Gera um objeto com as mensagens de auditoria para um registro específico 
    /// </summary>
    /// <param name="tabela"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("mensagem")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAuditoriaMessages(
        [FromQuery] string tabela,
        [FromQuery] string id)
    {
        try
        {
            var resultados = await _auditoriaService.GetAuditoriaMessageAsync(tabela, id);
            return Ok(resultados);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar o histórico: {e.Message}." });
        }
    }
    
    /// <summary>
    /// Gera um objeto com as mensagens de auditoria para uma nota específica, tanto para ela quanto seus dependentes 
    /// </summary>
    /// <param name="id">Id da nota a ser buscada</param>
    /// <returns>Lista de mensagens</returns>
    [HttpGet("mensagem/nota/{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAuditoriaMessagesNota([FromRoute] long id)
    {
        try
        {
            var resultados = await _auditoriaService.GetAuditoriaMessageNotaAsync(id);
            return Ok(resultados);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar o histórico da nota: {e.Message}." });
        }
    }
    
    /// <summary>
    /// Gera um objeto com as mensagens de auditoria para uma ordem específica, tanto para ela quanto seus dependentes 
    /// </summary>
    /// <param name="id">Id da ordem a ser buscada</param>
    /// <returns>Lista de mensagens</returns>
    [HttpGet("mensagem/ordem/{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAuditoriaMessagesOrdem([FromRoute] long id)
    {
        try
        {
            var resultados = await _auditoriaService.GetAuditoriaMessageOrdemAsync(id);
            return Ok(resultados);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar o histórico da ordem: {e.Message}." });
        }
    }
    
    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }
}