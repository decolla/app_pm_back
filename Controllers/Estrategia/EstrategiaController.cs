using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services.Manutencao;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers.Estrategia;

/// <summary>
/// Endpoints de estrategia de manutencao .
/// </summary>
[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/estrategia")]
public class EstrategiaController : ControllerBase
{
    private readonly string _operador = "estratégia de mantuenção";
    private readonly EstrategiaService _estrategiaService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="EstrategiaController"/>.
    /// </summary>
    public EstrategiaController(EstrategiaService estrategiaService, IMapper mapper)
    {
        _estrategiaService = estrategiaService;
        _mapper = mapper;
    }

    /// <summary>
    /// Cria uma estratégia 
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] EstrategiaManutencaoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var obj = _mapper.Map<EstrategiaModel>(dto);
            if (string.IsNullOrWhiteSpace(obj.Nome))
            {
                return BadRequest(new { message = "Nome da estrategia obrigatorio." });
            }

            var novoObj = await _estrategiaService.CreateAsync(obj);
            return new ObjectResult(novoObj);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar {_operador}: {e.Message}." });
        }
    }

    /// <summary>
    /// Lista as estratégias em formato de página 
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int? pagina, [FromQuery] int? tamanho)
    {
        try
        {
            if (TemPaginacaoInvalida(pagina, tamanho))
            {
                return BadRequest(new { message = "Os parametros pagina e tamanho devem ser maiores que zero." });
            }

            if (pagina.HasValue || tamanho.HasValue)
            {
                int p = pagina ?? 1;
                int t = tamanho ?? 20;
                var acoesPaginadas = await _estrategiaService.GetNotasPaginadasAsync(p, t);
                return Ok(acoesPaginadas);
            }

            var acoes = await _estrategiaService.GetAllNotasAsync();
            return Ok(acoes);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar {_operador}: {e.Message}" });
        }
    }

    /// <summary>
    /// Retorna a quantidade de elementos 
    /// </summary>
    [HttpGet("count")]
    public async Task<IActionResult> Contar()
    {
        try
        {
            var notas = await _estrategiaService.CountNotasAsync();
            return Ok(new { count = notas });
        }
        catch (Exception)
        {
            return NotFound(new { message = $"Erro ao procurar {_operador}." });
        }
    }

    /// <summary>
    /// Atualiza um objeto do modelo
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] EstrategiaManutencaoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "IdEstrategia invalido na rota. Envie id maior que zero." });
            }

            if (dto.IdEstrategia <= 0)
            {
                dto.IdEstrategia = id;
            }

            if (dto.IdEstrategia != id)
            {
                return BadRequest(
                    new { message = "Id da rota e IdEstrategia do corpo da requisição devem ser iguais." });
            }

            var obj = _mapper.Map<EstrategiaModel>(dto);
            if (obj.IdEstrategia <= 0)
            {
                return BadRequest(new { message = "IdEstrategia invalido. Envie IdEstrategia maior que zero." });
            }

            var result = await _estrategiaService.UpdateNotaAsync(obj);
            return Ok(new { message = $"{_operador} atualizada com sucesso: " + result.IdEstrategia });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao atualizar {_operador}." });
        }
    }

    /// <summary>
    /// Deleta um objeto do modelo
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deletar(int id)
    {
        try
        {
            var deleted = await _estrategiaService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = $"{_operador} não encontrado(a)." });

            return Ok(new { message = $"{_operador} deletado(a) com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao deletar um(a) {_operador}." });
        }
    }

    /// <summary>
    /// Pesquisa estrategias com filtros da tela.
    /// </summary>
    [HttpGet("pesquisa")]
    public async Task<IActionResult> Pesquisar(
        [FromQuery] string? nome,
        [FromQuery] int? id,
        [FromQuery] string? tipoAtivo,
        [FromQuery] string? visao,
        [FromQuery] int? pagina,
        [FromQuery] int? tamanho)
    {
        try
        {
            if (id.HasValue && id <= 0)
            {
                return BadRequest(new { message = "Parametro id deve ser maior que zero." });
            }

            if (TemPaginacaoInvalida(pagina, tamanho))
            {
                return BadRequest(new { message = "Os parametros pagina e tamanho devem ser maiores que zero." });
            }

            var resultados = await _estrategiaService.BuscarAsync(id, nome, tipoAtivo, visao, pagina, tamanho);
            return Ok(resultados);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao pesquisar {_operador}: {e.Message}." });
        }
    }

    /// <summary>
    /// Busca por id a estratégia 
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "IdEstrategia invalido na rota. Envie id maior que zero." });
            }

            var obj = await _estrategiaService.GetByIdAsync(id);
            if (obj == null)
            {
                return NotFound(new { message = $"{_operador} não encontrado(a)." });
            }

            return Ok(obj);
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao procurar {_operador}." });
        }
    }

    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }
}