using app_pm_back.Services.FichasServices;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers.FichasControllers;

[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/linha-temporal")]
public class LinhaTemporalController : ControllerBase
{
    private readonly LinhaTemporalService _linhaTemporalService;

    public LinhaTemporalController(LinhaTemporalService linhaTemporalService)
    {
        _linhaTemporalService = linhaTemporalService;
    }

    /// <summary>
    /// Obtém a linha temporal paginada das LIs consultando diretamente a API do APM com paginação.
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
            var apmResultado = await _linhaTemporalService.ObterLisDoApmAsync(pagina, tamanho);

            // Extrai a lista e o total para facilitar o uso
            var lisPagina = apmResultado.Assets;
            var totalRegistros = apmResultado.Total;

            if (!lisPagina.Any())
            {
                return Ok(new
                {
                    pagina,
                    tamanho,
                    total = totalRegistros,
                    itens = new List<object>(),
                    temProxima = false,
                    temAnterior = pagina > 1
                });
            }

            // Para cada LI da página, busca notas/ordens no banco
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

            // Determina se tem próxima página: se veio menos itens que o tamanho, não tem próxima
            var temProxima = lisPagina.Count == tamanho;

            return Ok(new
            {
                pagina,
                tamanho,
                total = totalRegistros,
                itens = resultado,
                temProxima,
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
}