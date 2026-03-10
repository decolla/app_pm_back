using app_pm_back.DTO;
using app_pm_back.Models;
using app_pm_back.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderPetroService _petroService;
    private readonly IMapper _mapper;

    public OrdersController(OrderPetroService petroService, IMapper mapper)
    {
        _petroService = petroService;
        _mapper = mapper;
    }

    /// <summary>
    /// Cria uma ordem a partir de um dto 
    /// </summary>
    /// <param name="dto"></param>
    [HttpPost("dto")]
    public async Task<IActionResult> CreateOrderDto([FromBody] CreateOrdemPetroDto dto)
    {
        try
        {
            var novaOrdem = _mapper.Map<OrdemPetroModel>(dto);

            var notaCriada = await _petroService.CreateOrdemAsync(novaOrdem);
            return Created($"ordem/{notaCriada.IdOrdem}", notaCriada);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar a ordem: {e.Message}." });
        }
    }

    /// <summary>
    /// Criar uma ordem a partir de um objeto
    /// </summary>
    /// <param name="order"></param>
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrdemPetroModel order)
    {
        try
        {
            var newOrder = await _petroService.CreateOrdemAsync(order);
            return Created($"ordem/{newOrder.IdOrdem}", newOrder);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao criar a ordem: {e.Message}");
            return BadRequest(new { message = "Erro ao criar a ordem." });
        }
    }

    /// <summary>
    /// Retorna uma página de ordens com base no parâmetro opcional de página e tamanho
    /// </summary>
    /// <param name="pagina"></param>
    /// <param name="tamanho"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetPage(int? pagina, int? tamanho)
    {
        try
        {
            if (pagina.HasValue || tamanho.HasValue)
            {
                int p = pagina ?? 1;
                int t = tamanho ?? 20;
                var notasPaginadas = await _petroService.GetOrdensPaginadasAsync(p, t);
                return Ok(notasPaginadas);
            }

            var todasNotas = await _petroService.GetAllOrdensAsync();
            return Ok(todasNotas);
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao processar notas." });
        }
    }

    /// <summary>
    /// Busca uma nota por Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> SearchOrder(long id)
    {
        try
        {
            var ordem = await _petroService.GetOrdemByIdAsync(id);

            if (ordem.Equals(null)) return NotFound(new { message = "Ordem não encontrada" });

            return Ok(ordem);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao procurar ordem: {e.Message}");
            return NotFound(new { message = "Erro ao procurar ordem." });
        }
    }

    /// <summary>
    /// Retorna quantidade de ordens no banco
    /// </summary>
    /// <returns></returns>
    [HttpPost("count")]
    public async Task<IActionResult> CountOrders()
    {
        try
        {
            var ordens = await _petroService.CountOrdensAsync();
            return Ok(new { count = ordens });
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao procurar ordens: {e.Message}");
            return BadRequest(new { message = "Erro ao procurar ordens." });
        }
    }

    /// <summary>
    /// Atualiza uma ordem
    /// </summary>
    /// <param name="ordem"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> UpdateOrder(OrdemPetroModel ordem)
    {
        try
        {
            var result = await _petroService.UpdateOrdemAsync(ordem);
            return Ok(new { message = "Ordem atualizada com sucesso: " + result.IdOrdem });
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao atualizar ordem: {e.Message}");
            return BadRequest(new { message = "Erro ao atualizar ordem." });
        }
    }

    /// <summary>
    /// Deleta uma ordem por Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteOrder(long id)
    {
        try
        {
            var result = await _petroService.DeleteOrdemAsync(id);
            return Ok(new { message = "Ordem deletada com sucesso" });
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao deletar ordem: {e.Message}");
            return BadRequest(new { message = "Erro ao deletar ordem." });
        }
    }
}