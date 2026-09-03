using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services.FichasServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers.FichasControllers;

[ApiController]
[Authorize(Policy = "View")]
[Route(Config.RouteConfig.BaseRoute + "/reliability")]
public class ReliabilityController : ControllerBase
{
    private readonly ReliabilityService _reliabilityService;
    
    public ReliabilityController(ReliabilityService reliabilityService)
    {
        _reliabilityService = reliabilityService;
    }
    /// <summary>
    /// Busca Todas as datas de notas e ordens de um LI
    /// </summary>
    /// <param name="idLi">LI (Local de Instalação) a ser procurada</param>
    /// <returns>idLi+ lista de datas</returns>
    [HttpGet("Data")]
    public async Task<IActionResult> BuscaReliability([FromQuery] string idLi, [FromQuery] string? SapId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(idLi))
            {
                return BadRequest(new { message = "Parametro idLi obrigatorio." });
            }

            var obj = await _reliabilityService.DatasLIReliability(idLi,SapId);
            if (obj == null) return NotFound(new { message = $"Nenhuma busca realizada para o LI informado." });

            return Ok(obj);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar datas para este ativo: {e.Message}." });
        }
    }
    
    /// <summary>
    /// Sincroniza todos os LI do apm e preenche a tabela de Reliability
    /// </summary>
    [HttpGet("SyncLI")]
    [Authorize(Policy = "Edit")]
    public async Task<IActionResult> SyncReliability()
    {
        try
        {

            var obj = await _reliabilityService.SyncLi();
            if (obj == false) return NotFound(new { message = $"Erro ao buscar LIs para sincronizar." });

            return Ok(obj);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar LIs para sincronizar: {e.Message}." });
        }
    }

    /// <summary>
    /// Relaciona todos os LI sincronizados com as ordens e notas a partir da Classificação e MetodoClassificacao
    /// </summary>
    [HttpGet("filtrar")]
    public async Task<IActionResult> FiltrarOrdens([FromQuery] List<Classificacao> Classificacoes,
        [FromQuery] List<MetodoClassificacao> Metodos)
    {
        try
        { 
            // 1. Começamos com a base da consulta (ainda não foi no banco de dados)
            var (listLI, houveMudanca) = await _reliabilityService.FiltraFichas(Classificacoes, Metodos);

            if (!houveMudanca)
                return Ok(new { mensagem = "Nenhuma alteração encontrada." });
            
            var result = await _reliabilityService.DatasReliability(listLI);
            

            if (result == null)
                    return StatusCode(502, new { mensagem = "A API de Reliability não retornou uma resposta válida." });

            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar datas para este ativo: {e.Message}." });
        }
    }
}