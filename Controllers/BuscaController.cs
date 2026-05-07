using app_pm_back.Services;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

[ApiController]
[Route("api/V1/Comgas/busca")]
public class BuscaController : ControllerBase
{
    private readonly GenericSearchService _searchService;

    public BuscaController(GenericSearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet("tabelas")]
    public IActionResult ListarTabelasCampos()
    {
        var tabelas = _searchService.ListarTabelasCampos();
        return Ok(tabelas);
    }

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

    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }
}
