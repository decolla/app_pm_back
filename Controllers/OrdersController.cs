using app_pm_back.DTO;
using app_pm_back.Models;
using app_pm_back.Models.CongasModel;
using app_pm_back.Services.CongasServices;
using app_pm_back.Services.PetroServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

/// <summary>
/// Controller para ordens
/// </summary>
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderPetroService _petroService;
    private readonly OrderCongasService _congasService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="petroService"></param>
    /// <param name="congasService"></param>
    /// <param name="mapper"></param>
    public OrdersController(OrderPetroService petroService, OrderCongasService congasService, IMapper mapper)
    {
        _petroService = petroService;
        _congasService = congasService;     
        _mapper = mapper;
    }

   // =========================================================================
    // ENDPOINTS DA PETRO (Rotas: api/orders/petro/...)
    // =========================================================================

    /// <summary>
    /// Cadastra uma ordem Petro pelo Dto
    /// </summary>
    [HttpPost("petro")]
    public async Task<IActionResult> CriarOrdemPetro([FromBody] CreateOrdemPetroDto dto)
    {
        try
        {
            var novaNota = _mapper.Map<OrdemPetroModel>(dto);
            var notaCriada = await _petroService.CreateNotaAsync(novaNota);
            return Created($"api/orders/petro/{notaCriada.IdNota}", notaCriada);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar a ordem Petro: {e.Message}." });
        }
    }

    /// <summary>
    /// Retorna uma lista todas as ordens Petro com sistema de páginas
    /// </summary>
    [HttpGet("petro")]
    public async Task<IActionResult> ListarNotasPetro([FromQuery] int? pagina, [FromQuery] int? tamanho)
    {
        try
        {
            if (pagina.HasValue || tamanho.HasValue)
            {
                int p = pagina ?? 1;
                int t = tamanho ?? 20;
                var notasPaginadas = await _petroService.GetNotasPaginadasAsync(p, t);
                return Ok(notasPaginadas);
            }

            var todasNotas = await _petroService.GetAllNotasAsync();
            return Ok(todasNotas);
        }
        catch (Exception )
        {
            return BadRequest(new { message = "Erro ao processar ordens Petro." });
        }
    }

    /// <summary>
    /// Conta o número total de ordens Petro salvas no banco
    /// </summary>
    [HttpGet("petro/count")]
    public async Task<IActionResult> ContarNotasPetro()
    {
        try
        {
            var notas = await _petroService.CountNotasAsync();
            return Ok(new { count = notas });
        }
        catch (Exception )
        {
            return NotFound(new { message = "Erro ao procurar notas Petro." });
        }
    }

    /// <summary>
    /// Retorna as Fichas de uma nota Petro e suas ocorrências de mesmo tipo
    /// </summary>
    [HttpGet("petro/ativo/{dnsIoT}")]
    public async Task<IActionResult> ListarOrdensPorAtivoPetro(string dnsIoT, [FromQuery] string? type)
    {
        try
        {
            // FALTA IMPLEMENTAÇÃO. FAZER: após correção das classes 
            var resultado = await _petroService.GetFichasPorAtivoAsync(dnsIoT, type);
            return Ok(resultado);
        }
        catch (Exception )
        {
            return BadRequest(new { message = "Erro ao buscar fichas do ativo Petro." });
        }
    }

    /// <summary>
    /// Atualiza uma nota Petro de mesmo Id
    /// </summary>
    [HttpPut("petro")]
    public async Task<IActionResult> AtualizarOrdemPetro([FromBody] OrdemPetroModel ordem)
    {
        try
        {
            var result = await _petroService.UpdateNotaAsync(ordem);
            return Ok(new { message = "Nota Petro atualizada com sucesso: " + result.IdNota });
        }
        catch (Exception )
        {
            return BadRequest(new { message = "Erro ao atualizar nota Petro." });
        }
    }

    /// <summary>
    /// Delete uma nota Petro de mesmo Id
    /// </summary>
    [HttpDelete("petro/{id:int}")]
    public async Task<IActionResult> DeletarOrdemPetro(int id)
    {
        try
        {
           await _petroService.DeleteNotaAsync(id);
            return Ok(new { message = "Nota Petro deletada com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao deletar nota Petro." });
        }
    }


    // =========================================================================
    // ENDPOINTS DA CONGAS (Rotas: api/orders/congas/...)
    // =========================================================================

    /// <summary>
    /// Cadastra uma nota Congas
    /// </summary>
    [HttpPost("congas")]
    public async Task<IActionResult> CriarOrdemCongas([FromBody] OrderCongasModel ordem)
    {
        try
        {
            var notaCriada = await _congasService.CreateNotaAsync(ordem);
            return Created($"api/orders/congas/{notaCriada.NumeroNotaOrigem}", notaCriada);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar a ordem Congas: {e.Message}." });
        }
    }

    /// <summary>
    /// Retorna uma lista todas as notas Congas com sistema de páginas
    /// </summary>
    [HttpGet("congas")]
    public async Task<IActionResult> ListarOrdensCongas([FromQuery] int? pagina, [FromQuery] int? tamanho)
    {
        try
        {
            if (pagina.HasValue || tamanho.HasValue)
            {
                int p = pagina ?? 1;
                int t = tamanho ?? 20;
                var notasPaginadas = await _congasService.GetNotasPaginadasAsync(p, t);
                return Ok(notasPaginadas);
            }

            var todasNotas = await _congasService.GetAllNotasAsync();
            return Ok(todasNotas);
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao processar ordens Congas." });
        }
    }

    /// <summary>
    /// Conta o número total de notas Congas salvas no banco
    /// </summary>
    [HttpGet("congas/count")]
    public async Task<IActionResult> ContarNotasCongas()
    {
        try
        {
            var notas = await _congasService.CountNotasAsync();
            return Ok(new { count = notas });
        }
        catch (Exception)
        {
            return NotFound(new { message = "Erro ao procurar ordens Congas." });
        }
    }

    /// <summary>
    /// Retorna as Fichas de uma nota Congas e suas ocorrências de mesmo tipo
    /// </summary>
    [HttpGet("congas/ativo/{dnsIoT}")]
    public async Task<IActionResult> ListarFichasPorAtivoCongas(string dnsIoT, [FromQuery] string? type)
    {
        try
        {
            var resultado = await _congasService.GetFichasPorAtivoAsync(dnsIoT, type);
            return Ok(resultado);
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao buscar ordens do ativo Congas." });
        }
    }

    /// <summary>
    /// Atualiza uma nota Congas de mesmo Id
    /// </summary>
    [HttpPut("congas")]
    public async Task<IActionResult> AtualizarNotaCongas([FromBody] OrderCongasModel ordem)
    {
        try
        {
            var result = await _congasService.UpdateNotaAsync(ordem);
            return Ok(new { message = "Ordem Congas atualizada com sucesso: " + result.NumeroNotaOrigem });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao atualizar ordem Congas." });
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
            await _congasService.DeleteNotaAsync(id);
            return Ok(new { message = "Ordem Congas deletada com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao deletar ordem Congas." });
        }
    }
}
