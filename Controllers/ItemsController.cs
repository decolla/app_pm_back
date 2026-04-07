using app_pm_back.Models.CongasModel;
using app_pm_back.Services.CongasServices;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

/// <summary>
/// Controller para itens de notas e ordens 
/// </summary>
[ApiController]
[Route("api/items")]
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
    [HttpPost("congas")]
    public async Task<IActionResult> CriarItemCongas([FromBody] ItemNotaCongasModel novaNota)
    {
        try
        {
            var item = await _itemNoteCongasService.CreateNotaAsync(novaNota);
            return Created($"api/notes/congas/{item.IdItem}", item);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar o item de nota Congas: {e.Message}." });
        }
    }

    /// <summary>
    /// Retorna uma lista todos os itens da Congas com sistema de páginas
    /// </summary>
    [HttpGet("congas")]
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
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao processar itens de notas Congas." });
        }
    }

    /// <summary>
    /// Conta o número total de itens Congas salvas no banco
    /// </summary>
    [HttpGet("congas/count")]
    public async Task<IActionResult> ContarItensCongas()
    {
        try
        {
            var notas = await _itemNoteCongasService.CountNotasAsync();
            return Ok(new { count = notas });
        }
        catch (Exception)
        {
            return NotFound(new { message = "Erro ao procurar notas Congas." });
        }
    }

    /// <summary>
    /// Atualiza uma nota Congas de mesmo Id
    /// </summary>
    [HttpPut("congas")]
    public async Task<IActionResult> AtualizarNotaCongas([FromBody] ItemNotaCongasModel item)
    {
        try
        {
            var result = await _itemNoteCongasService.UpdateNotaAsync(item);
            return Ok(new { message = "Nota Congas atualizada com sucesso: " + result.IdItem });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao atualizar nota Congas." });
        }
    }

    /// <summary>
    /// Delete uma nota Congas de mesmo Id
    /// </summary>
    [HttpDelete("congas/{id:int}")]
    public async Task<IActionResult> DeletarNotaCongas(int id)
    {
        try
        {
            await _itemNoteCongasService.DeleteNotaAsync(id);
            return Ok(new { message = "Nota Congas deletada com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao deletar nota Congas." });
        }
    }
}