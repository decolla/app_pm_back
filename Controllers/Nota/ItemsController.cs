using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services.Manutencao;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

/// <summary>
/// Classe ItemsController.
/// </summary>
[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/items")]
public class ItemsController : ControllerBase
{
    private readonly ItemNotaService _itemNotaService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="ItemsController"/>.
    /// </summary>
    public ItemsController(ItemNotaService itemNotaService, IMapper mapper)
    {
        _itemNotaService = itemNotaService;
        _mapper = mapper;
    }

    /// <summary>
    /// Cria um novo registro.
    /// </summary>
    /// <param name="dto">DTO contendo os dados da requisição.</param>
    /// <returns>Resultado da operação.</returns>
    /// <exception cref="System.Exception">Lançada em caso de erro interno ou falha no processamento.</exception>
    [HttpPost]
    public async Task<IActionResult> CriarItem([FromBody] ItemNotaManutencaoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var itemModel = _mapper.Map<ItemNotaModel>(dto);
            if (itemModel.IdNota <= 0)
            {
                return BadRequest(new { message = "IdNota invalido. Envie IdNota maior que zero." });
            }

            var item = await _itemNotaService.CreateAsync(itemModel);
            return new ObjectResult(item);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar o item: {e.Message}." });
        }
    }

    /// <summary>
    /// Lista os registros cadastrados com paginação.
    /// </summary>
    /// <returns>Resultado da operação.</returns>
    /// <exception cref="System.Exception">Lançada em caso de erro interno ou falha no processamento.</exception>
    [HttpGet]
    public async Task<IActionResult> ListarItens([FromQuery] int? pagina, [FromQuery] int? tamanho)
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
                var paginados = await _itemNotaService.GetNotasPaginadasAsync(p, t);
                return Ok(paginados);
            }

            var todos = await _itemNotaService.GetAllNotasAsync();
            return Ok(todos);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar itens: {e.Message}" });
        }
    }

    /// <summary>
    /// Obtém um registro pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador do registro.</param>
    /// <returns>Resultado da operação.</returns>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> ObterPorId([FromRoute] long id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Id invalido. Envie id maior que zero." });
            }

            var item = await _itemNotaService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound(new { message = "Item nao encontrado." });
            }

            return Ok(item);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar item: {e.Message}." });
        }
    }

    /// <summary>
    /// Obtém o total de registros cadastrados.
    /// </summary>
    /// <returns>Resultado da operação.</returns>
    /// <exception cref="System.Exception">Lançada em caso de erro interno ou falha no processamento.</exception>
    [HttpGet("count")]
    public async Task<IActionResult> ContarItens()
    {
        try
        {
            var count = await _itemNotaService.CountNotasAsync();
            return Ok(new { count });
        }
        catch (Exception)
        {
            return NotFound(new { message = "Erro ao procurar items." });
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
    public async Task<IActionResult> AtualizarItem([FromRoute] int id, [FromBody] ItemNotaManutencaoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "Id do item invalido na rota. Envie id maior que zero." });
            }

            if (dto.IdItemNota <= 0)
            {
                dto.IdItemNota = id;
            }

            if (dto.IdItemNota != id)
            {
                return BadRequest(new { message = "Id da rota e IdItemNota do corpo da requisição devem ser iguais." });
            }

            var item = _mapper.Map<ItemNotaModel>(dto);
            var result = await _itemNotaService.UpdateNotaAsync(item);
            return Ok(new { message = "Item atualizado com sucesso: " + result.IdItemNota });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao atualizar item." });
        }
    }

    /// <summary>
    /// Exclui um registro pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador do registro.</param>
    /// <returns>Resultado da operação.</returns>
    /// <exception cref="System.Exception">Lançada em caso de erro interno ou falha no processamento.</exception>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletarItem(int id)
    {
        try
        {
            var deleted = await _itemNotaService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = "Item não encontrado." });

            return Ok(new { message = "Item deletado com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao deletar item." });
        }
    }

    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }
}