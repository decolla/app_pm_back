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
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/tipos-estrategia")]
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
    /// <param name="dto">DTO contendo os dados da requisição.</param>
    /// <returns>O objeto criado com status 201 (Created).</returns>
    /// <response code="201">Registro criado com sucesso.</response>
    /// <response code="400">Erros na validação do modelo ou dados fornecidos inválidos.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        return CreatedAtAction(nameof(ObterPorId), new { id = novoObj.IdTipoEstrategia }, novoObj);
    }

    /// <summary>
    /// Lista as estratégias em formato de página
    /// </summary>
    /// <param name="pagina">Número da página (1-based) para paginação.</param>
    /// <param name="tamanho">Quantidade de itens por página.</param>
    /// <returns>Uma lista de registros cadastrados ou um objeto paginado contendo total e resultados.</returns>
    /// <response code="200">Sucesso ao listar registros.</response>
    /// <response code="400">Parâmetros de paginação inválidos (menores ou iguais a zero).</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    /// Obtém um registro pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador exclusivo do registro (deve ser maior que zero).</param>
    /// <returns>O objeto correspondente cadastrado.</returns>
    /// <response code="200">Sucesso ao obter o registro.</response>
    /// <response code="400">ID fornecido inválido.</response>
    /// <response code="404">Registro não encontrado no banco de dados.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId([FromRoute] long id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Id invalido. Envie id maior que zero." });
            }

            var item = await _tipoEstrategiaService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound(new { message = $"{_operador} nao encontrado." });
            }

            return Ok(item);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar {_operador}: {e.Message}." });
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
    /// <param name="id">Identificador exclusivo na rota (deve ser maior que zero).</param>
    /// <param name="dto">DTO contendo os novos dados para atualização.</param>
    /// <returns>Mensagem de confirmação da atualização.</returns>
    /// <response code="200">Sucesso ao atualizar o registro.</response>
    /// <response code="400">Erros na validação do modelo ou ID divergente entre a rota e o corpo.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    /// <param name="id">Identificador exclusivo do registro a ser excluído.</param>
    /// <returns>Mensagem de confirmação da exclusão.</returns>
    /// <response code="200">Sucesso ao deletar o registro.</response>
    /// <response code="404">Registro não encontrado no banco de dados.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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