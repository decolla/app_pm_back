using app_pm_back.DTO.ComgasDTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services.CongasServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

[ApiController]
[Route("api/V1/Comgas/items")]
public class ItemsController : ControllerBase
{
    private readonly ItemNotaService _itemNoteCongasService;
    private readonly IMapper _mapper;

    public ItemsController(ItemNotaService itemNoteCongasService, IMapper mapper)
    {
        _itemNoteCongasService = itemNoteCongasService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> CriarItemCongas([FromBody] ItemNotaManutencaoComgasDto dto)
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

            var item = await _itemNoteCongasService.CreateAsync(itemModel);
            return new ObjectResult(item);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar o item: {e.Message}." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ListarItensCongas([FromQuery] int? pagina, [FromQuery] int? tamanho)
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
                var paginados = await _itemNoteCongasService.GetNotasPaginadasAsync(p, t);
                return Ok(paginados);
            }

            var todos = await _itemNoteCongasService.GetAllNotasAsync();
            return Ok(todos);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar itens: {e.Message}" });
        }
    }

    [HttpGet("count")]
    public async Task<IActionResult> ContarItensCongas()
    {
        try
        {
            var count = await _itemNoteCongasService.CountNotasAsync();
            return Ok(new { count });
        }
        catch (Exception)
        {
            return NotFound(new { message = "Erro ao procurar items." });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> AtualizarNotaCongas([FromRoute] int id, [FromBody] ItemNotaManutencaoComgasDto dto)
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
            var result = await _itemNoteCongasService.UpdateNotaAsync(item);
            return Ok(new { message = "Item atualizado com sucesso: " + result.IdItemNota });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao atualizar item." });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletarNotaCongas(int id)
    {
        try
        {
            var deleted = await _itemNoteCongasService.DeleteNotaAsync(id);
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