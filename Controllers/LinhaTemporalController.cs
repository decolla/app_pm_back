using app_pm_back.Services.Manutencao;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/LinhaTemporal")]
public class LinhaTemporalController : ControllerBase
{
    private readonly LinhaTemporalService _linhaTemporalService;

    public LinhaTemporalController(LinhaTemporalService linhaTemporalService)
    {
        _linhaTemporalService = linhaTemporalService;
    }

    /// <summary>
    /// Obtém a linha temporal paginada das LIs.
    /// Primeira chamada busca do APM (asset/installation/date) e cacheia.
    /// Chamadas subsequentes usam cache + paginação local.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> ObterLinhaTemporal(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanho = 20)
    {
        if (pagina <= 0) pagina = 1;
        if (tamanho <= 0) tamanho = 20;
        if (tamanho > 100) tamanho = 100;

        try
        {
            // 1. Busca todas as LIs do APM (cache ou API)
            var todasLis = await _linhaTemporalService.ObterTodasLisDoApmAsync();
            
            if (!todasLis.Any())
            {
                return Ok(new
                {
                    itens = new List<object>(),
                    pagina,
                    tamanho,
                    totalItens = 0,
                    totalPaginas = 0,
                    temProxima = false,
                    temAnterior = false
                });
            }

            // 2. Paginação no nível das LIs
            var totalItens = todasLis.Count;
            var totalPaginas = (int)Math.Ceiling((double)totalItens / tamanho);
            var lisPagina = todasLis
                .Skip((pagina - 1) * tamanho)
                .Take(tamanho)
                .ToList();

            // 3. Para cada LI da página, busca notas/ordens no banco
            var resultado = new List<object>();

            foreach (var li in lisPagina)
            {
                var itens = await _linhaTemporalService.ObterItensDaLiAsync(li.IdLi);
                
                resultado.Add(new
                {
                    idLi = li.IdLi,
                    dnsIot = li.DnsIot,
                    data = li.Data,
                    eventos = itens.Eventos.Select(e => new
                    {
                        numero = e.Numero,
                        data = e.Data,
                        tipo = e.Tipo,
                        idReferencia = e.IdReferencia,
                        descricao = e.Descricao,
                        classificacao = e.Classificacao,
                        confiabilidadeRestaurada = e.ConfiabilidadeRestaurada
                    })
                });
            }

            return Ok(new
            {
                itens = resultado,
                pagina,
                tamanho,
                totalItens,
                totalPaginas,
                temProxima = pagina < totalPaginas,
                temAnterior = pagina > 1
            });
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, 
                $"Erro ao comunicar com APM: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                $"Erro interno: {ex.Message}");
        }
    }

    /// <summary>
    /// Força refresh do cache do APM
    /// </summary>
    [HttpPost("refresh-cache")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AtualizarCache()
    {
        try
        {
            var qtd = await _linhaTemporalService.AtualizarCacheAsync();
            return Ok(new { message = "Cache atualizado", totalLis = qtd });
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, $"Erro no APM: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}