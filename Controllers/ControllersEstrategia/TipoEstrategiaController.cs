using app_pm_back.DTO.ComgasDTO;
using app_pm_back.Models.CongasModel;
using app_pm_back.Services.CongasServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers.ControllersEstrategia;

/// <summary>
/// Endpoints de tipo de estrategia da Comgas.
/// </summary>
[Route("api/V1/Comgas/tipo-estrategia")]
[ApiController]
public class TipoEstrategiaController: ControllerBase
{
    private readonly string _operador = "tipo de estratégia";
    private readonly TipoEstrategiaService _tipoEstrategiaService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Construtor.
    /// </summary>
    /// <param name="tipoEstrategiaService"></param>
    /// <param name="mapper"></param>
    public TipoEstrategiaController(TipoEstrategiaService tipoEstrategiaService, IMapper mapper)
    {
        _tipoEstrategiaService = tipoEstrategiaService;
        _mapper = mapper;
    }

    /// <summary>
    /// Cria uma estratégia 
    /// </summary>
    /// <param name="dto">Objeto a ser adicionado</param>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] TipoEstrategiaComgasDto dto)
    {

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var obj = _mapper.Map<TipoEstrategiaModel>(dto);
            
            if (obj.EstrategiaId <= 0)
            {
                return BadRequest(new { message = "IdEstrategia invalido. Envie um Id maior que zero." });
            }
            
            if (string.IsNullOrWhiteSpace(obj.NomeTipo))
            {
                return BadRequest(new { message = "NomeTipo obrigatorio." });
            }

            var novoObj = await _tipoEstrategiaService.CreateAsync(obj);
            return Created($"api/V1/Comgas/tipo-estrategia/{novoObj.IdTipoEstrategia}", novoObj);
    }

    /// <summary>
    /// Lista as estratégias em formato de página 
    /// </summary>
    /// <param name="pagina">Página a ser exibida</param>
    /// <param name="tamanho">Quantidade de objetos na página</param>
    /// <returns>Lista de objetos presentes na página</returns>
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
    /// <returns>Quantidade de elementos cadastrados no banco</returns>
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
    /// <param name="id">Id do tipo de estrategia na rota</param>
    /// <param name="dto">Objeto a ser atualizado</param>
    /// <returns>Novo objeto</returns>
    // [HttpPut]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] TipoEstrategiaComgasDto dto)
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
                return BadRequest(new { message = "Id da rota e IdTipoEstrategia do corpo da requisição devem ser iguais." });
            }

            var obj = _mapper.Map<TipoEstrategiaModel>(dto);
            if (obj.IdTipoEstrategia <= 0)
            {
                return BadRequest(new { message = "IdTipoEstrategia invalido. Envie IdTipoEstrategia maior que zero." });
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
    /// <param name="id">Id do objeto a ser deletado</param>
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

    /// <summary>
    /// Valida pagina e tamanho para evitar consultas inconsistentes.
    /// </summary>
    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }
}
