using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services.Manutencao;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace app_pm_back.Controllers;

/// <summary>
/// Classe CentrosTrabalhoController.
/// </summary>
[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/centros-trabalho")]
[Authorize(Policy = "View")]
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
    /// Cria um novo Centro de Trabalho.
    /// </summary>
    /// <param name="dto">DTO contendo os dados da requisição.</param>
    /// <returns>O objeto criado com status 201 (Created).</returns>
    /// <response code="201">Registro criado com sucesso.</response>
    /// <response code="400">Erros na validação do modelo ou dados fornecidos inválidos.</response>
    [HttpPost]
    [Authorize(Policy = "Edit")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CentroTrabalhoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var centro = _mapper.Map<CtmModel>(dto);

            var criado = await _service.CreateAsync(centro);
            return CreatedAtAction(nameof(ObterPorId), new { id = criado.IdCTM }, criado);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar {_operador}: {e.Message}." });
        }
    }

    /// <summary>
    /// Lista os registros cadastrados com paginação.
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

            var item = await _service.GetByIdAsync(id);
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
    /// Lista os registros cadastrados com paginação.
    /// </summary>
    /// <returns>Uma lista de registros cadastrados ou um objeto paginado contendo total e resultados.</returns>
    /// <response code="200">Sucesso ao listar registros.</response>
    /// <response code="400">Parâmetros de paginação inválidos (menores ou iguais a zero).</response>
    [HttpGet("{id:long}/usuarios")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListaUsuarios([FromRoute] long id)
    {
        try
        {
            var todos = await _service.GetUsuariosCtmAsync(id);
            var dto = _mapper.Map<List<UsuarioDto>>(todos);
            return Ok(dto);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar {_operador}: {e.Message}." });
        }
    }

    /// <summary>
    /// Atualiza os usuários de um centro de trabalho
    /// </summary>
    /// <param name="id">Identificador exclusivo na rota (deve ser maior que zero).</param>
    /// <param name="idsUsuarios">Ids dos usuários a serem adicionados.</param>
    /// <returns>Mensagem de confirmação da atualização.</returns>
    /// <response code="200">Sucesso ao atualizar o registro.</response>
    /// <response code="400">Erros na validação do modelo ou ID divergente entre a rota e o corpo.</response>
    [HttpPut("{id:long}/usuarios")]
    [Authorize(Policy = "Edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AdicionarUsuarios([FromRoute] long id, [FromQuery] List<long> idsUsuarios)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(new
                    { message = "IdCentroTrabalho/IdGPM invalido na rota. Envie id maior que zero." });
            }

            var result = await _service.AdicionarUsuariosCtmAsync(id, idsUsuarios);
            
            var dto = _mapper.Map<List<UsuarioDto>>(result);
            return Ok(dto);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao atualizar {_operador}: {e}." });
        }
    }

    /// <summary>
    /// Atribui usuários de um centro de trabalho a uma ficha específica
    /// </summary>
    /// <param name="tipoFicha">Nota ou ordem</param>
    /// <param name="idFicha">Id da nota/ordem a ser atribuída</param>
    /// <param name="idGrupo">Id do grupo nos quais os usuários são pertencentes</param>
    /// <param name="idsUsuarios">Lista de usuários</param>
    /// <returns></returns>
    [HttpPut("{idGrupo:long}/usuarios/atribuir")]
    public async Task<IActionResult> AtribuirUsuariosFicha([FromQuery] Fichas tipoFicha, [FromQuery] long idFicha,
        [FromRoute] long idGrupo,
        [FromQuery] List<long> idsUsuarios)
    {
        try
        {
            var result = await _service.AtribuirFichas(tipoFicha, idFicha, idGrupo, idsUsuarios);
            return Ok(result);
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
            var count = await _service.CountAsync();
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
    /// <param name="id">Identificador exclusivo na rota (deve ser maior que zero).</param>
    /// <param name="dto">DTO contendo os novos dados para atualização.</param>
    /// <returns>Mensagem de confirmação da atualização.</returns>
    /// <response code="200">Sucesso ao atualizar o registro.</response>
    /// <response code="400">Erros na validação do modelo ou ID divergente entre a rota e o corpo.</response>
    [HttpPut("{id:long}")]
    [Authorize(Policy = "Edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Atualizar([FromRoute] long id, [FromBody] CentroTrabalhoDto dto)
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
    /// <param name="id">Identificador exclusivo do registro a ser excluído.</param>
    /// <returns>Mensagem de confirmação da exclusão.</returns>
    /// <response code="200">Sucesso ao deletar o registro.</response>
    /// <response code="404">Registro não encontrado no banco de dados.</response>
    [HttpDelete("{id:long}")]
    [Authorize(Policy = "Edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar([FromRoute] long id)
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