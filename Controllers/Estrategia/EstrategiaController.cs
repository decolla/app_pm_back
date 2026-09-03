using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services.Manutencao;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace app_pm_back.Controllers.Estrategia;

/// <summary>
/// Endpoints de estrategia de manutencao .
/// </summary>
[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/estrategias")]
[Authorize(Policy = "View")]
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
    /// <param name="dto">DTO contendo os dados da requisição.</param>
    /// <returns>O objeto criado com status 201 (Created).</returns>
    /// <response code="201">Registro criado com sucesso.</response>
    /// <response code="400">Erros na validação do modelo ou dados fornecidos inválidos.</response>
    [HttpPost]
    [Authorize(Policy = "Edit")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
            return CreatedAtAction(nameof(BuscarPorId), new { id = novoObj.IdEstrategia }, novoObj);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar {_operador}: {e.Message}." });
        }
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
            var notas = await _estrategiaService.CountAsync();
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
    [HttpPut("{id:long}")]
    [Authorize(Policy = "Edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Atualizar([FromRoute] long id, [FromBody] EstrategiaManutencaoDto dto)
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
    /// <param name="id">Identificador exclusivo do registro a ser excluído.</param>
    /// <returns>Mensagem de confirmação da exclusão.</returns>
    /// <response code="200">Sucesso ao deletar o registro.</response>
    /// <response code="404">Registro não encontrado no banco de dados.</response>
    [HttpDelete("{id:long}")]
    [Authorize(Policy = "Edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar(long id)
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
        [FromQuery] long? id,
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
    /// <param name="id">Identificador exclusivo do registro (deve ser maior que zero).</param>
    /// <returns>O objeto correspondente cadastrado.</returns>
    /// <response code="200">Sucesso ao obter o registro.</response>
    /// <response code="400">ID fornecido inválido.</response>
    /// <response code="404">Registro não encontrado no banco de dados.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BuscarPorId(long id)
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
    
    /// <summary>
    /// Cria um tipo estratégia e uma estratégia para cada tipo de LI
    /// </summary>
    /// <param name="id">Identificador exclusivo do registro (deve ser maior que zero).</param>
    /// <returns>O objeto correspondente cadastrado.</returns>
    /// <response code="200">Sucesso ao obter o registro.</response>
    /// <response code="400">ID fornecido inválido.</response>
    /// <response code="404">Registro não encontrado no banco de dados.</response>
    [HttpGet("template")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CriaTemplate()
    {
        try
        {

            var obj = await _estrategiaService.GerarTemplates();
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
