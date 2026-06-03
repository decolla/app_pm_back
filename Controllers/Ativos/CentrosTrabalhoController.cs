using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services.Manutencao;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

/// <summary>
/// Classe CentrosTrabalhoController.
/// </summary>
[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/centros-trabalho")]
public class CentrosTrabalhoController : ControllerBase
{
    private readonly CtmService _service;
    private readonly IMapper _mapper;
    private readonly string _operador = "centro de trabalho";

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="CentrosTrabalhoController"/>.
    /// </summary>
    public CentrosTrabalhoController(CtmService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    /// <summary>
    /// Cria um novo registro.
    /// </summary>
    /// <param name="dto">DTO contendo os dados da requisição.</param>
    /// <returns>Resultado da operação.</returns>
    /// <exception cref="System.Exception">Lançada em caso de erro interno ou falha no processamento.</exception>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CentroTrabalhoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var centro = _mapper.Map<CtmModel>(dto);
            if (centro.IdCTM <= 0)
            {
                return BadRequest(new { message = "IdCTM/IdCentroTrabalho invalido. Envie maior que zero." });
            }

            var criado = await _service.CreateAsync(centro);
            return new ObjectResult(criado);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar {_operador}: {e.Message}." });
        }
    }

    /// <summary>
    /// Lista os registros cadastrados com paginação.
    /// </summary>
    /// <returns>Resultado da operação.</returns>
    /// <exception cref="System.Exception">Lançada em caso de erro interno ou falha no processamento.</exception>
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
                var paginado = await _service.GetNotasPaginadasAsync(p, t);
                return Ok(paginado);
            }

            var todos = await _service.GetAllNotasAsync();
            return Ok(todos);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar {_operador}: {e.Message}." });
        }
    }

    /// <summary>
    /// Obtém o total de registros cadastrados.
    /// </summary>
    /// <returns>Resultado da operação.</returns>
    /// <exception cref="System.Exception">Lançada em caso de erro interno ou falha no processamento.</exception>
    [HttpGet("count")]
    public async Task<IActionResult> Contar()
    {
        try
        {
            var count = await _service.CountNotasAsync();
            return Ok(new { count });
        }
        catch (Exception)
        {
            return NotFound(new { message = $"Erro ao procurar {_operador}." });
        }
    }

    /// <summary>
    /// Atualiza um registro existente.
    /// </summary>
    /// <param name="id">Identificador do registro.</param>
    /// <param name="dto">DTO contendo os dados da requisição.</param>
    /// <returns>Resultado da operação.</returns>
    /// <exception cref="System.Exception">Lançada em caso de erro interno ou falha no processamento.</exception>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] CentroTrabalhoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new
                    { message = "IdCentroTrabalho/IdGPM invalido na rota. Envie id maior que zero." });
            }

            if (dto.IdCtm <= 0)
            {
                dto.IdCtm = id;
            }

            if (dto.IdCtm != id)
            {
                return BadRequest(new { message = "Id da rota e IdCentroTrabalho/IdGPM do corpo devem ser iguais." });
            }

            var centro = _mapper.Map<CtmModel>(dto);
            var result = await _service.UpdateNotaAsync(centro);
            return Ok(new { message = $"{_operador} atualizado com sucesso: {result.IdCTM}" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao atualizar {_operador}." });
        }
    }

    /// <summary>
    /// Exclui um registro pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador do registro.</param>
    /// <returns>Resultado da operação.</returns>
    /// <exception cref="System.Exception">Lançada em caso de erro interno ou falha no processamento.</exception>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deletar([FromRoute] int id)
    {
        try
        {
            var deleted = await _service.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = $"{_operador} nao encontrado." });

            return Ok(new { message = $"{_operador} deletado com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao deletar {_operador}." });
        }
    }

    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }
}