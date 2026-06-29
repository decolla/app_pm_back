using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services.Manutencao;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

/// <summary>
/// Classe VariaveisController.
/// </summary>
[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/variaveis")]
public class VariaveisController : ControllerBase
{
    private readonly VariavelNotaService _variavelNotaService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="VariaveisController"/>.
    /// </summary>
    public VariaveisController(VariavelNotaService variavelNotaService, IMapper mapper)
    {
        _variavelNotaService = variavelNotaService;
        _mapper = mapper;
    }

    /// <summary>
    /// Cria um novo registro de variável.
    /// </summary>
    /// <param name="dto">DTO contendo os dados da requisição.</param>
    /// <returns>O objeto criado com status 201 (Created).</returns>
    /// <response code="201">Registro criado com sucesso.</response>
    /// <response code="400">Erros na validação do modelo ou dados fornecidos inválidos.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CriarVariavel([FromBody] VariavelNotaDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var variavelModel = _mapper.Map<VariavelNotaModel>(dto);
            if (!variavelModel.IdNota.HasValue || variavelModel.IdNota <= 0)
            {
                return BadRequest(new { message = "IdNota invalido. Envie IdNota maior que zero." });
            }

            if (string.IsNullOrWhiteSpace(variavelModel.NomeVariavel))
            {
                return BadRequest(new { message = "O nome da variavel é obrigatório." });
            }

            var variavel = await _variavelNotaService.CreateAsync(variavelModel);
            return CreatedAtAction(nameof(ObterPorId), new { id = variavel.IdVariavel }, variavel);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar a variável: {e.Message}." });
        }
    }

    /// <summary>
    /// Lista os registros de variáveis cadastrados com paginação.
    /// </summary>
    /// <param name="pagina">Número da página (1-based) para paginação.</param>
    /// <param name="tamanho">Quantidade de itens por página.</param>
    /// <returns>Uma lista de registros cadastrados ou um objeto paginado contendo total e resultados.</returns>
    /// <response code="200">Sucesso ao listar registros.</response>
    /// <response code="400">Parâmetros de paginação inválidos (menores ou iguais a zero).</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListarVariaveis([FromQuery] int? pagina, [FromQuery] int? tamanho)
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
                var paginados = await _variavelNotaService.GetNotasPaginadasAsync(p, t);
                return Ok(paginados);
            }

            var todos = await _variavelNotaService.GetAllNotasAsync();
            return Ok(todos);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar variáveis: {e.Message}" });
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

            var variavel = await _variavelNotaService.GetByIdAsync(id);
            if (variavel == null)
            {
                return NotFound(new { message = "Variável não encontrada." });
            }

            return Ok(variavel);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar variável: {e.Message}." });
        }
    }

    /// <summary>
    /// Obtém o total de registros cadastrados.
    /// </summary>
    /// <returns>Resultado da operação.</returns>
    [HttpGet("count")]
    public async Task<IActionResult> ContarVariaveis()
    {
        try
        {
            var count = await _variavelNotaService.CountAsync();
            return Ok(new { count });
        }
        catch (Exception)
        {
            return NotFound(new { message = "Erro ao procurar variáveis." });
        }
    }

    /// <summary>
    /// Atualiza um registro de variável existente.
    /// </summary>
    /// <param name="id">Identificador exclusivo na rota (deve ser maior que zero).</param>
    /// <param name="dto">DTO contendo os novos dados para atualização.</param>
    /// <returns>Mensagem de confirmação da atualização.</returns>
    /// <response code="200">Sucesso ao atualizar o registro.</response>
    /// <response code="400">Erros na validação do modelo ou ID divergente entre a rota e o corpo.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AtualizarVariavel([FromRoute] long id, [FromBody] VariavelNotaDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "Id da variável invalido na rota. Envie id maior que zero." });
            }

            if (dto.IdVariavel <= 0)
            {
                dto.IdVariavel = id;
            }

            if (dto.IdVariavel != id)
            {
                return BadRequest(new { message = "Id da rota e IdVariavel do corpo da requisição devem ser iguais." });
            }

            var variavel = _mapper.Map<VariavelNotaModel>(dto);
            var result = await _variavelNotaService.UpdateNotaAsync(variavel);
            return Ok(new { message = "Variável atualizada com sucesso: " + result.IdVariavel });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao atualizar variável." });
        }
    }

    /// <summary>
    /// Exclui um registro pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador exclusivo do registro a ser excluído.</param>
    /// <returns>Mensagem de confirmação da exclusão.</returns>
    /// <response code="200">Sucesso ao deletar o registro.</response>
    /// <response code="404">Registro não encontrado no banco de dados.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletarVariavel(long id)
    {
        try
        {
            var deleted = await _variavelNotaService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = "Variável não encontrada." });

            return Ok(new { message = "Variável deletada com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao deletar variável." });
        }
    }

    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }
}
