using app_pm_back.DTO.ComgasDTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services.CongasServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers.ControllerPlanoManutencao;

/// <summary>
/// Endpoints de plano de manutencao da Comgas.
/// </summary>
[ApiController]
[Route("api/V1/Comgas/plano-manutencao")]
public class PlanoManutencaoController : ControllerBase
{
    private readonly string _operador = "plano de mantuenção";
    private readonly PlanoManutencaoService _planoManutencaoService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Construtor.
    /// </summary>
    /// <param name="planoManutencaoService"></param>
    /// <param name="mapper"></param>
    public PlanoManutencaoController(PlanoManutencaoService planoManutencaoService, IMapper mapper)
    {
        _planoManutencaoService = planoManutencaoService;
        _mapper = mapper;
    }

    /// <summary>
    /// Cria um objeto do modelo
    /// </summary>
    /// <param name="dto">Objeto a ser adicionado</param>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] PlanoManutencaoComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var obj = _mapper.Map<PlanoModel>(dto);
            if (obj.IdPlano <= 0)
            {
                return BadRequest(new { message = "IdPlano invalido. Envie IdPlano maior que zero." });
            }

            var novoObj = await _planoManutencaoService.CreateComTiposAsync(obj);
            return new ObjectResult(novoObj);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar {_operador}: {e.Message}." });
        }
    }

    /// <summary>
    /// Lista objetos do modelo em formato de página 
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
                var acoesPaginadas = await _planoManutencaoService.GetNotasPaginadasAsync(p, t);
                return Ok(acoesPaginadas);
            }

            var acoes = await _planoManutencaoService.GetAllNotasAsync();
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
            var notas = await _planoManutencaoService.CountNotasAsync();
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
    /// <param name="id">Id do plano na rota</param>
    /// <param name="dto">Objeto a ser atualizado</param>
    /// <returns>Novo objeto</returns>
    // [HttpPut]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Atualizar([FromRoute] long id, [FromBody] PlanoManutencaoComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "CodigoPlano invalido na rota. Envie id maior que zero." });
            }

            if (dto.IdPlano <= 0)
            {
                dto.IdPlano = id;
            }

            if (dto.IdPlano != id)
            {
                return BadRequest(new { message = "Id da rota e IdPlano do corpo da requisição devem ser iguais." });
            }

            var obj = _mapper.Map<PlanoModel>(dto);
            if (obj.IdPlano <= 0)
            {
                return BadRequest(new { message = "IdPlano invalido. Envie IdPlano maior que zero." });
            }

            var result = await _planoManutencaoService.UpdateNotaAsync(obj);
            return Ok(new { message = $"{_operador} atualizada com sucesso: " + result.IdPlano });
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
            var deleted = await _planoManutencaoService.DeleteNotaAsync(id);
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