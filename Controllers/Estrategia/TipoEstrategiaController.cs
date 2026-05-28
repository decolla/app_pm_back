using app_pm_back.DTO;
using app_pm_back.Data;
using app_pm_back.Models.Domain;
using app_pm_back.Services.Manutencao;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Controllers.Estrategia;

/// <summary>
/// Endpoints de tipo de estrategia .
/// </summary>
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/tipo-estrategia")]
[ApiController]
public class TipoEstrategiaController : ControllerBase
{
    private readonly string _operador = "tipo de estratégia";
    private readonly TipoEstrategiaService _tipoEstrategiaService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="TipoEstrategiaController"/>.
    /// </summary>
    public TipoEstrategiaController(TipoEstrategiaService tipoEstrategiaService, IMapper mapper)
    {
        _tipoEstrategiaService = tipoEstrategiaService;
        _mapper = mapper;
    }

    /// <summary>
    /// Cria uma estratégia 
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] TipoEstrategiaDto dto, [FromServices] AppDbContext context)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (!dto.IdEstrategia.HasValue || dto.IdEstrategia.Value <= 0)
        {
            return BadRequest(new { message = "IdEstrategia obrigatorio. Envie um Id maior que zero." });
        }

        var estrategiaExiste = await context.PM_Estrategias
            .AnyAsync(e => e.IdEstrategia == dto.IdEstrategia.Value);

        if (!estrategiaExiste)
        {
            return BadRequest(new { message = "Estrategia informada nao encontrada." });
        }

        var obj = _mapper.Map<TipoEstrategiaModel>(dto);

        if (obj.IdEstrategia <= 0)
        {
            return BadRequest(new { message = "IdEstrategia invalido. Envie um Id maior que zero." });
        }

        var novoObj = await _tipoEstrategiaService.CreateAsync(obj);
        return new ObjectResult(novoObj);
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
                var acoesPaginadas = await _tipoEstrategiaService.GetNotasPaginadasAsync(p, t);
                return Ok(acoesPaginadas);
            }

            var acoes = await _tipoEstrategiaService.GetAllNotasAsync();
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
            var notas = await _tipoEstrategiaService.CountNotasAsync();
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
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] TipoEstrategiaDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "IdTipoEstrategia invalido na rota. Envie id maior que zero." });
            }

            if (dto.IdTipoEstrategia <= 0)
            {
                dto.IdTipoEstrategia = id;
            }

            if (dto.IdTipoEstrategia != id)
            {
                return BadRequest(new
                    { message = "Id da rota e IdTipoEstrategia do corpo da requisição devem ser iguais." });
            }

            var obj = _mapper.Map<TipoEstrategiaModel>(dto);
            if (obj.IdTipoEstrategia <= 0)
            {
                return BadRequest(new
                    { message = "IdTipoEstrategia invalido. Envie IdTipoEstrategia maior que zero." });
            }

            var result = await _tipoEstrategiaService.UpdateNotaAsync(obj);
            return Ok(new { message = $"{_operador} atualizada com sucesso: " + result.IdTipoEstrategia });
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
            var deleted = await _tipoEstrategiaService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = $"{_operador} não encontrado(a)." });

            return Ok(new { message = $"{_operador} deletado(a) com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao deletar um(a) {_operador}." });
        }
    }

    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }
}