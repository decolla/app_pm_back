using app_pm_back.Models;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

/// <summary>
/// Controller para equipamentos
/// </summary>
[ApiController]
[Route("api/V1/Comgas/equipments")]
public class EquipmentsController : ControllerBase
{
    private readonly EquipmentService _equipmentService;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="equipmentService"></param>
    public EquipmentsController(EquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    /// <summary>
    /// Cadastra um equipamento
    /// </summary>
    /// <param name="novoEquipamento">Equipamento a ser criado</param>
    [HttpPost]
    public async Task<IActionResult> CriarEquipamentoCongas([FromBody] EquipamentoAtivoModel novoEquipamento)
    {
        try
        {
            var equipamento = await _equipmentService.CreateAsync(novoEquipamento);
            return Created($"api/V1/Comgas/equipments/{equipamento.TagEquipamento}", equipamento);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar o equipamento: {e.Message}." });
        }
    }
    
    /// <summary>
    /// Exibe os equipamentos cadastrados em formato de página
    /// </summary>
    /// <param name="pagina">Página de exibição</param>
    /// <param name="tamanho">Tamanho da página</param>
    [HttpGet]
    public async Task<IActionResult> ListarEquipamentoCongas([FromQuery] int? pagina, [FromQuery] int? tamanho)
    {
        try
        {
            if (pagina.HasValue || tamanho.HasValue)
            {
                int p = pagina ?? 1;
                int t = tamanho ?? 20;
                var equipamentosPaginados = await _equipmentService.GetNotasPaginadasAsync(p, t);
                return Ok(equipamentosPaginados);
            }

            var equipamentos = await _equipmentService.GetAllNotasAsync();
            return Ok(equipamentos);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar equipamentos: {e.Message}" });
        }
    }

    /// <summary>
    /// Número total de equipamentos cadastrados
    /// </summary>
    /// <returns>Quantidade de equipamentos cadastrados</returns>
    [HttpGet("count")]
    public async Task<IActionResult> ContarItensCongas()
    {
        try
        {
            var notas = await _equipmentService.CountNotasAsync();
            return Ok(new { count = notas });
        }
        catch (Exception)
        {
            return NotFound(new { message = "Erro ao procurar equipamentos." });
        }
    }

    /// <summary>
    /// Atualiza um equipamento
    /// </summary>
    /// <param name="equipamento">Equipamento a ser atualizado</param>
    /// <returns>Equipamento atualizado</returns>
    [HttpPut]
    public async Task<IActionResult> AtualizarEquipamento([FromBody] EquipamentoAtivoModel equipamento)
    {
        try
        {
            var result = await _equipmentService.UpdateNotaAsync(equipamento);
            return Ok(new { message = "Equipamento atualizado com sucesso: " + result.TagEquipamento });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao atualizar equipamento." });
        }
    }
    
    /// <summary>
    /// Deleta um equipamento
    /// </summary>
    /// <param name="id">Id do equipamento</param>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletarNotaCongas(int id)
    {
        try
        {
            var deleted = await _equipmentService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = "Equipamento não encontrado." });

            return Ok(new { message = "Equipamento deletado com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao deletar equipamento." });
        }
    }
}