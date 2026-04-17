using app_pm_back.Models.CongasModel;
using app_pm_back.Services.CongasServices;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

[Route("api/V1/Comgas/tipo-estrategia")]
[ApiController]
public class TipoEstrategiaController: ControllerBase
{
    private readonly string operador = "tipo de estratégia";
    private readonly TipoEstrategiaService _tipoEstrategiaService;

    public TipoEstrategiaController(TipoEstrategiaService tipoEstrategiaService)
    {
        _tipoEstrategiaService = tipoEstrategiaService;
    }

    /// <summary>
    /// Cria uma estratégia 
    /// </summary>
    /// <param name="obj">Objeto a ser adicionado</param>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] TipoEstrategiaModel obj)
    {
        try
        {
            var novoObj = await _tipoEstrategiaService.CreateAsync(obj);
            return Created($"api/V1/Comgas/tipo-estrateggia/{novoObj.IdTipoEstrategia}", novoObj);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar {operador}: {e.Message}." });
        }
    }

    /// <summary>
    /// Lista as estratégias em formato de página 
    /// </summary>
    /// <param name="pagina">Página a ser exibida</param>
    /// <param name="tamanho">Quantidade de objetos na página</param>
    /// <returns>Lista de objetos presentes na página</returns>
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int? pagina, [FromQuery] int? tamanho)
    {
        try
        {
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
            return BadRequest(new { message = $"Erro ao processar {operador}: {e.Message}" });
        }
    }

    /// <summary>
    /// Retorna a quantidade de elementos 
    /// </summary>
    /// <returns>Quantidade de elementos cadastrados no banco</returns>
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
            return NotFound(new { message = $"Erro ao procurar {operador}." });
        }
    }

    /// <summary>
    /// Atualiza um objeto do modelo
    /// </summary>
    /// <param name="obj">Objeto a ser atualizado</param>
    /// <returns>Novo objeto</returns>
    [HttpPut]
    public async Task<IActionResult> Atualizar([FromBody] TipoEstrategiaModel obj)
    {
        try
        {
            var result = await _tipoEstrategiaService.UpdateNotaAsync(obj);
            return Ok(new { message = $"{operador} atualizada com sucesso: " + result.IdTipoEstrategia });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao atualizar {operador}." });
        }
    }

    /// <summary>
    /// Deleta um objeto do modelo
    /// </summary>
    /// <param name="id">Id do objeto a ser deletado</param>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deletar(int id)
    {
        try
        {
            var deleted = await _tipoEstrategiaService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = $"{operador} não encontrado(a)." });

            return Ok(new { message = $"{operador} deletado(a) com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao deletar um(a) {operador}." });
        }
    }
    
}
