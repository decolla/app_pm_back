using app_pm_back.Services;
using app_pm_back.Models;
using Microsoft.AspNetCore.Mvc;

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
        [FromQuery] int? tamanho = null)
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
                tamanho);

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
    public async Task<IActionResult> BuscaGerald([FromQuery] int ativo)
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
    public async Task<IActionResult> BuscaGeraldList([FromQuery] List<int> ativos)
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

    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }
}