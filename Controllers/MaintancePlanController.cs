using app_pm_back.Models.CongasModel;
using app_pm_back.Models.Domain;
using app_pm_back.Services.CongasServices;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

[ApiController]
[Route("api/V1/Comgas/maintance-plan")]
public class MaintancePlanController : ControllerBase
{
    private readonly string operador = "plano de mantuenção";
    private readonly PlanoManutencaoService _planoManutencaoService;

    public MaintancePlanController(PlanoManutencaoService planoManutencaoService)
    {
        _planoManutencaoService = planoManutencaoService;
    }

    /// <summary>
    /// Cria um objeto do modelo
    /// </summary>
    /// <param name="obj">Objeto a ser adicionado</param>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] PlanoManutencaoModel obj)
    {
        try
        {
            var novoObj = await _planoManutencaoService.CreateAsync(obj);
            return Created($"api/V1/Comgas/maintance-plan/{novoObj.CodigoPlano}", novoObj);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar {operador}: {e.Message}." });
        }
    }

    /// <summary>
    /// Lista objetos do modelo em formato de página 
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
                var acoesPaginadas = await _planoManutencaoService.GetNotasPaginadasAsync(p, t);
                return Ok(acoesPaginadas);
            }

            var acoes = await _planoManutencaoService.GetAllNotasAsync();
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
            var notas = await _planoManutencaoService.CountNotasAsync();
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
    public async Task<IActionResult> Atualizar([FromBody] PlanoManutencaoModel obj)
    {
        try
        {
            var result = await _planoManutencaoService.UpdateNotaAsync(obj);
            return Ok(new { message = $"{operador} atualizada com sucesso: " + result.CodigoPlano });
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
            var deleted = await _planoManutencaoService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = $"{operador} não encontrado(a)." });

            return Ok(new { message = $"{operador} deletado(a) com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao deletar um(a) {operador}." });
        }
    }

    /// <summary>
    /// Busca um plano de mantuenção associado a um ativo, retorna um objeto com os dados relevantes
    /// </summary>
    /// <param name="ativo">Ativo a ser procurado</param>
    /// <returns>Objeto com informações relevantes</returns>
    [HttpGet("busca-ativo")]
    public async Task<IActionResult> BuscaGerald(string ativo)
    {
        try
        {
            var obj = await _planoManutencaoService.AtivoGerald(ativo);
            if (obj == null) return NotFound(new { message = $"Nenhum {operador} encontrado para o ativo informado." });

            return Ok(obj);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar {operador}: {e.Message}." });
        }
    }
}