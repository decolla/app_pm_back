using app_pm_back.Models.CongasModel;
using app_pm_back.Models.Domain;
using app_pm_back.Services.CongasServices;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

/// <summary>
/// Controller para itens de notas e ordens 
/// </summary>
[ApiController]
[Route("api/V1/Comgas/items")]
public class ItemsController : ControllerBase
{
    private readonly ItemNoteCongasService _itemNoteCongasService;

    public ItemsController(ItemNoteCongasService itemNoteCongasService)
    {
        _itemNoteCongasService = itemNoteCongasService;
    }

    /// <summary>
    /// Cadastra um item de nota Congas
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CriarItemCongas([FromBody] ItemNotaManutencaoModel novaNota)
    {
        try
        {
            var item = await _itemNoteCongasService.CreateAsync(novaNota);
            return Created($"api/V1/Comgas/items/{item.IdItemNota}", item);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar o item de nota Comgas: {e.Message}." });
        }
    }

    /// <summary>
    /// Retorna uma lista todos os itens da Congas com sistema de páginas
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListarItensCongas([FromQuery] int? pagina, [FromQuery] int? tamanho)
    {
        try
        {
            if (pagina.HasValue || tamanho.HasValue)
            {
                int p = pagina ?? 1;
                int t = tamanho ?? 20;
                var notasPaginadas = await _itemNoteCongasService.GetNotasPaginadasAsync(p, t);
                return Ok(notasPaginadas);
            }

            var todasNotas = await _itemNoteCongasService.GetAllNotasAsync();
            return Ok(todasNotas);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar itens de notas Comgas: {e.Message}" });
        }
    }

    /// <summary>
    /// Conta o número total de itens Congas salvas no banco
    /// </summary>
    [HttpGet("count")]
    public async Task<IActionResult> ContarItensCongas()
    {
        try
        {
            var notas = await _itemNoteCongasService.CountNotasAsync();
            return Ok(new { count = notas });
        }
        catch (Exception)
        {
            return NotFound(new { message = "Erro ao procurar items Comgas." });
        }
    }

    /// <summary>
    /// Atualiza uma nota Congas de mesmo Id
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> AtualizarNotaCongas([FromBody] ItemNotaManutencaoModel item)
    {
        try
        {
            var result = await _itemNoteCongasService.UpdateNotaAsync(item);
            return Ok(new { message = "Nota Comgas atualizada com sucesso: " + result.IdItemNota });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao atualizar item Comgas." });
        }
    }

    /// <summary>
    /// Delete uma nota Congas de mesmo Id
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletarNotaCongas(int id)
    {
        try
        {
            var deleted = await _itemNoteCongasService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = "Item Comgas não encontrado." });

            return Ok(new { message = "Item Comgas deletada com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao deletar item Comgas." });
        }
    }
}