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
    /// Obtém um registro específico.
    /// </summary>
    /// <param name="tabela">Parâmetro do método.</param>
    /// <param name="campo">Parâmetro do método.</param>
    /// <param name="valor">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    /// <exception cref="System.Exception">Lançada em caso de erro interno ou falha no processamento.</exception>
    [HttpGet]
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
    /// Obtém um registro específico.
    /// </summary>
    /// <param name="tabela">Parâmetro do método.</param>
    /// <param name="nome">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    /// <exception cref="System.Exception">Lançada em caso de erro interno ou falha no processamento.</exception>
    [HttpGet("nome")]
    public async Task<IActionResult> BuscarPorNome(
        [FromQuery] string tabela,
        [FromQuery] string nome,
        [FromQuery] string? campo,
        [FromQuery] int? pagina = null,
        [FromQuery] int? tamanho = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tabela))
            {
                return BadRequest(new { message = "Informe tabela para a busca." });
            }

            if (string.IsNullOrWhiteSpace(nome))
            {
                return BadRequest(new { message = "Informe nome para a busca." });
            }

            if (TemPaginacaoInvalida(pagina, tamanho))
            {
                return BadRequest(new { message = "Os parametros pagina e tamanho devem ser maiores que zero." });
            }

            var campoNome = string.IsNullOrWhiteSpace(campo) ? "Nome" : campo;
            var resultados = await _searchService.BuscarPorNomeAsync(
                tabela,
                campoNome,
                nome,
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
    /// Busca notas ou ordens de manutenção por diversos critérios, retorna uma lista de objetos com os dados relevantes
    /// </summary>
    /// <param name="busca">Modelo de busca com campos comparáveis</param>
    /// <param name="pagina">Página a ser exibida</param>
    /// <param name="tamanho">Número de elementos presente na página</param>
    /// <returns>Lista de Nota e Ordens com base nos filtros</returns>
    [HttpPost("fichas")]
    public async Task<IActionResult> BuscarNotaOrdem(
        [FromBody] BuscaOrdemNotaDto busca,
        [FromQuery] int? pagina = null,
        [FromQuery] int? tamanho = null,
        [FromQuery] Fichas? Tfichas = Fichas.Ambas)
    {
        try
        {
            if (TemPaginacaoInvalida(pagina, tamanho))
            {
                return BadRequest(new { message = "Os parametros pagina e tamanho devem ser maiores que zero." });
            }

            var resultados = await _searchService.BuscarNotaOrdemAsync(
                busca,
                pagina,
                tamanho,
                Tfichas);

            return Ok(resultados);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar nota/ordem: {e.Message}." });
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
    /// Busca as datas de renovação associadas ao LI, retorna um objeto com os dados relevantes
    /// </summary>
    /// <param name="idLi">LI (Local de Instalação) a ser procurada</param>
    /// <param name="tipoDados">Tipo de dados da renovação</param>
    /// <returns>Objeto com informações relevantes</returns>
    [HttpGet("ativo/datas")]
    public async Task<IActionResult> BuscaAtivoDatas([FromQuery] string idLi, [FromQuery] Classificacao tipoDados)
    {
        try
        {

            if (string.IsNullOrWhiteSpace(idLi))
            {
                return BadRequest(new { message = "Parametro idLi obrigatorio." });
            }

            if (!Enum.IsDefined(typeof(Classificacao), tipoDados))
            {
                return BadRequest(new { message = "Parametro tipoDados invalido." });
            }

            var obj = await _searchService.AtivoDatas(idLi, tipoDados);
            if (obj == null) return NotFound(new { message = $"Nenhuma busca realizada para o LI informado." });

            return Ok(obj);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar datas para este ativo: {e.Message}." });
        }
    }
    
    /// <summary>
    /// Busca Todas as datas de notas e ordens daquele LI
    /// </summary>
    /// <param name="idLi">LI (Local de Instalação) a ser procurada</param>
    /// <returns>idLi+ lista de datas</returns>
    [HttpGet("reliability/LiData")]
    public async Task<IActionResult> BuscaReliability([FromQuery] string idLi)
    {
        try
        {

            if (string.IsNullOrWhiteSpace(idLi))
            {
                return BadRequest(new { message = "Parametro idLi obrigatorio." });
            }

            var obj = await _searchService.DatasReliability(idLi);
            if (obj == null) return NotFound(new { message = $"Nenhuma busca realizada para o LI informado." });

            return Ok(obj);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar datas para este ativo: {e.Message}." });
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