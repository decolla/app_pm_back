using app_pm_back.Services;
using app_pm_back.Models;
using app_pm_back.Data;
using app_pm_back.Models.Domain;

using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace app_pm_back.Controllers;

/// <summary>
/// Classe BuscaController.
/// </summary>
[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/busca")]
public class BuscaController : ControllerBase
{
    private readonly GenericSearchService _searchService;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="BuscaController"/>.
    /// </summary>
    public BuscaController(GenericSearchService searchService)
    {
        _searchService = searchService;
    }
    
    /// <summary>
    /// Lista os registros cadastrados com paginação.
    /// </summary>
    /// <returns>Resultado da operação.</returns>
    [HttpGet("tabelas")]
    public IActionResult ListarTabelasCampos()
    {
        var tabelas = _searchService.ListarTabelasCampos();
        return Ok(tabelas);
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
        [FromQuery] string tabela,
        [FromQuery] string campo,
        [FromQuery] string valor,
        [FromQuery] bool parcial = true,
        [FromQuery] int? pagina = null,
        [FromQuery] int? tamanho = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tabela) || string.IsNullOrWhiteSpace(campo))
            {
                return BadRequest(new { message = "Informe tabela e campo para a busca." });
            }

            if (string.IsNullOrWhiteSpace(valor))
            {
                return BadRequest(new { message = "Informe valor para a busca." });
            }

            if (TemPaginacaoInvalida(pagina, tamanho))
            {
                return BadRequest(new { message = "Os parametros pagina e tamanho devem ser maiores que zero." });
            }

            var resultados = await _searchService.BuscarPorCampoAsync(
                tabela,
                campo,
                valor,
                parcial,
                pagina,
                tamanho);

            return Ok(resultados);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar na tabela {tabela}: {e.Message}." });
        }
    }

    /// <summary>
    /// Busca um plano de mantuenção associado a um ativo, retorna um objeto com os dados relevantes
    /// </summary>
    /// <param name="ativo">Ativo a ser procurado</param>
    /// <returns>Objeto com informações relevantes</returns>
    [HttpGet("ativo")]
    public async Task<IActionResult> BuscaGerald([FromQuery] ComponentType ativo)
    {
        try
        {
            if (ativo <= 0)
            {
                return BadRequest(new { message = "Parametro ativo obrigatorio." });
            }

            var obj = await _searchService.AtivoGerald(ativo);
            if (obj == null) return NotFound(new { message = $"Nenhuma busca realizada para o ativo informado." });

            return Ok(obj);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar para este ativo: {e.Message}." });
        }
    }

    /// <summary>
    /// Busca um plano de manuntenção associado a uma lista de ativos, retorna um objeto com os dados relevantes
    /// </summary>
    /// <param name="ativos"></param>
    /// <returns></returns>
    [HttpGet("ativo/list")]
    public async Task<IActionResult> BuscaGeraldList([FromQuery] List<ComponentType> ativos)
    {
        try
        {
            if (ativos == null || ativos.Count == 0)
            {
                return BadRequest(new { message = "Parametro ativos obrigatorio." });
            }

            var resultados = new List<object?>();
            foreach (var ativo in ativos)
            {
                if (ativo <= 0) continue; // Ignora valores inválidos

                var obj = await _searchService.AtivoGerald(ativo);
                resultados.Add(new { Ativo = ativo, Resultado = obj });
            }

            return Ok(resultados);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar para os ativos informados: {e.Message}." });
        }
    }

    /// <summary>
    /// Busca notas e ordens por intervalo de data e IdLi.
    /// </summary>
    /// <param name="dataInicio">Data inicial do intervalo.</param>
    /// <param name="dataFim">Data final do intervalo.</param>
    /// <param name="idLi">Local de instalação.</param>
    /// <returns>Lista com id, classificação, descrição e data de criação.</returns>
    [HttpGet("timeline")]
    public async Task<IActionResult> BuscarTimeline(
        [FromQuery] string dataInicio,
        [FromQuery] string dataFim,
        [FromQuery] string idLi)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(idLi))
            {
                return BadRequest(new { message = "Parametro idLi obrigatorio." });
            }

            if (string.IsNullOrWhiteSpace(dataInicio) || string.IsNullOrWhiteSpace(dataFim))
            {
                return BadRequest(new { message = "Parametros dataInicio e dataFim obrigatorios." });
            }

            if (!TryParseTimelineDate(dataInicio, out var inicio) ||
                !TryParseTimelineDate(dataFim, out var fim))
            {
                return BadRequest(new { message = "Formato de data invalido. Use yyyy-MM-dd ou yyyy-MM-ddTHH:mm:ss." });
            }

            if (inicio > fim)
            {
                return BadRequest(new { message = "Parametro dataInicio deve ser menor ou igual a dataFim." });
            }

            var resultados = await _searchService.BuscarTimelineAsync(inicio, fim, idLi);
            return Ok(resultados);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar timeline: {e.Message}." });
        }
    }

    private static bool TryParseTimelineDate(string value, out DateTime parsed)
    {
        var styles = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;
        var formats = new[]
        {
            "yyyy-MM-dd",
            "yyyy-MM-dd'T'HH:mm:ss",
            "yyyy-MM-dd'T'HH:mm:ss.fff",
            "yyyy-MM-dd'T'HH:mm:ss'Z'",
            "yyyy-MM-dd'T'HH:mm:ss.fff'Z'"
        };

        if (DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, styles, out parsed))
        {
            return true;
        }

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, styles, out parsed))
        {
            return true;
        }

        return DateTime.TryParse(value, CultureInfo.GetCultureInfo("pt-BR"), styles, out parsed);
    }

    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }
}