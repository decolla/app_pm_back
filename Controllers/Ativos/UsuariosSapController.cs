using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services.Manutencao;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/usuarios-sap")]
public class UsuariosSapController : ControllerBase
{
    private readonly UsuarioService _service;
    private readonly IMapper _mapper;
    private readonly string _operador = "usuario sap";

    public UsuariosSapController(UsuarioService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] UsuarioSapDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var usuario = _mapper.Map<UsuarioModel>(dto);
            if (usuario.IdUsuario <= 0)
            {
                return BadRequest(new { message = "IdUsuario invalido. Envie IdUsuario maior que zero." });
            }

            var criado = await _service.CreateAsync(usuario);
            return new ObjectResult(criado);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar {_operador}: {e.Message}." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int? pagina, [FromQuery] int? tamanho)
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
                var paginado = await _service.GetNotasPaginadasAsync(p, t);
                return Ok(paginado);
            }

            var todos = await _service.GetAllNotasAsync();
            return Ok(todos);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar {_operador}: {e.Message}." });
        }
    }

    [HttpGet("count")]
    public async Task<IActionResult> Contar()
    {
        try
        {
            var count = await _service.CountNotasAsync();
            return Ok(new { count });
        }
        catch (Exception)
        {
            return NotFound(new { message = $"Erro ao procurar {_operador}." });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] UsuarioSapDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "IdUsuario invalido na rota. Envie id maior que zero." });
            }

            if (dto.IdUsuario <= 0)
            {
                dto.IdUsuario = id;
            }

            if (dto.IdUsuario != id)
            {
                return BadRequest(new { message = "Id da rota e IdUsuario do corpo devem ser iguais." });
            }

            var usuario = _mapper.Map<UsuarioModel>(dto);
            var result = await _service.UpdateNotaAsync(usuario);
            return Ok(new { message = $"{_operador} atualizado com sucesso: {result.IdUsuario}" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao atualizar {_operador}." });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deletar([FromRoute] int id)
    {
        try
        {
            var deleted = await _service.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = $"{_operador} nao encontrado." });

            return Ok(new { message = $"{_operador} deletado com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao deletar {_operador}." });
        }
    }

    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }
}