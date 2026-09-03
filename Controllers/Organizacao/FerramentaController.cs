using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services.Manutencao;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace app_pm_back.Controllers.PlanoManutencao;

/// <summary>
/// Endpoints de ferramentas de tipo de manutencao.
/// </summary>
[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/ferramentas")]
[Authorize(Policy = "View")]
public class FerramentaController : ControllerBase
{
    private readonly FerramentaService _service;
    private readonly IMapper _mapper;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="FerramentaController"/>.
    /// </summary>
    public FerramentaController(FerramentaService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

        /// <summary>
    /// Cria um novo registro.
    /// </summary>
    /// <param name="dto">DTO contendo os dados da requisição.</param>
    /// <returns>O objeto criado com status 201 (Created).</returns>
    /// <response code="201">Registro criado com sucesso.</response>
    /// <response code="400">Erros na validação do modelo ou dados fornecidos inválidos.</response>
    [HttpPost]
    [Authorize(Policy = "Edit")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] FerramentaTipoDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var model = _mapper.Map<FerramentaModel>(dto);
        var criado = await _service.CreateAsync(model);
        return CreatedAtAction(nameof(BuscarPorId), new { id = criado.IdFerramenta }, criado);
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
    public async Task<IActionResult> Listar([FromQuery] long? tipoManutencaoId)
    {
        if (tipoManutencaoId.HasValue)
        {
            var filtrado = await _service.BuscarPorTipoManutencaoAsync(tipoManutencaoId.Value);
            return Ok(filtrado);
        }

        return Ok(await _service.GetAllNotasAsync());
    }

        /// <summary>
    /// Obtém um registro específico.
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
    public async Task<IActionResult> BuscarPorId([FromRoute] long id)
    {
        var obj = await _service.GetByIdAsync(id);
        if (obj == null) return NotFound(new { message = "Ferramenta nao encontrada." });
        return Ok(obj);
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
    public async Task<IActionResult> Atualizar([FromRoute] long id, [FromBody] FerramentaTipoDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (id <= 0) return BadRequest(new { message = "Id invalido na rota." });

        if (dto.IdFerramenta <= 0) dto.IdFerramenta = id;
        if (dto.IdFerramenta != id) return BadRequest(new { message = "Id da rota deve ser igual ao Id do corpo." });

        var model = _mapper.Map<FerramentaModel>(dto);
        var atualizado = await _service.UpdateNotaAsync(model);
        return Ok(atualizado);
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
        var deleted = await _service.DeleteNotaAsync(id);
        if (!deleted) return NotFound(new { message = "Ferramenta nao encontrada." });
        return Ok(new { message = "Ferramenta removida com sucesso." });
    }
}
