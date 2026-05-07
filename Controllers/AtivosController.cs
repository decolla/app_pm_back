using app_pm_back.DTO.ComgasDTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services.CongasServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

[ApiController]
[Route("api/V1/Comgas/imobilizados")]
public class ImobilizadosController : ControllerBase
{
    private readonly ImobilizadoService _service;
    private readonly IMapper _mapper;
    private readonly string _operador = "imobilizado";

    public ImobilizadosController(ImobilizadoService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ImobilizadoComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var imobilizado = _mapper.Map<ImobilizadoModel>(dto);
            if (string.IsNullOrWhiteSpace(imobilizado.TagImobilizado))
            {
                return BadRequest(new { message = "TagImobilizado invalida." });
            }

            var criado = await _service.CreateAsync(imobilizado);
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

            // verifica valor da página e tamanho, se um dos dois for informado, aplica paginação (com valores default caso um deles seja nulo)
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

    [HttpPut("{tag}")]
    public async Task<IActionResult> Atualizar([FromRoute] string tag, [FromBody] ImobilizadoComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (string.IsNullOrWhiteSpace(tag))
            {
                return BadRequest(new { message = "TagImobilizado invalida na rota." });
            }

            if (string.IsNullOrWhiteSpace(dto.TagImobilizado))
            {
                dto.TagImobilizado = tag;
            }

            if (!string.Equals(dto.TagImobilizado, tag, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Tag da rota e TagImobilizado do corpo devem ser iguais." });
            }

            var imobilizado = _mapper.Map<ImobilizadoModel>(dto);
            var result = await _service.UpdateNotaAsync(imobilizado);
            return Ok(new { message = $"{_operador} atualizado com sucesso: {result.TagImobilizado}" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao atualizar {_operador}." });
        }
    }

    [HttpDelete("{tag}")]
    public async Task<IActionResult> Deletar([FromRoute] string tag)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return BadRequest(new { message = "TagImobilizado invalida." });
            }

            var deleted = await _service.DeleteByTagAsync(tag);
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

[ApiController]
[Route("api/V1/Comgas/locais-instalacao")]
public class LocaisInstalacaoController : ControllerBase
{
    private readonly LocalInstalacaoService _service;
    private readonly IMapper _mapper;
    private readonly string _operador = "local de instalacao";

    public LocaisInstalacaoController(LocalInstalacaoService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] LocalInstalacaoComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var local = _mapper.Map<LocalInstalacaoModel>(dto);
            if (string.IsNullOrWhiteSpace(local.CodigoLocalInstalacao))
            {
                return BadRequest(new { message = "CodigoLocalInstalacao invalido." });
            }

            var criado = await _service.CreateAsync(local);
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

    [HttpPut("{codigo}")]
    public async Task<IActionResult> Atualizar([FromRoute] string codigo, [FromBody] LocalInstalacaoComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (string.IsNullOrWhiteSpace(codigo))
            {
                return BadRequest(new { message = "CodigoLocalInstalacao invalido na rota." });
            }

            if (string.IsNullOrWhiteSpace(dto.CodigoLocalInstalacao))
            {
                dto.CodigoLocalInstalacao = codigo;
            }

            if (!string.Equals(dto.CodigoLocalInstalacao, codigo, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Codigo da rota e CodigoLocalInstalacao do corpo devem ser iguais." });
            }

            var local = _mapper.Map<LocalInstalacaoModel>(dto);
            var result = await _service.UpdateNotaAsync(local);
            return Ok(new { message = $"{_operador} atualizado com sucesso: {result.CodigoLocalInstalacao}" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao atualizar {_operador}." });
        }
    }

    [HttpDelete("{codigo}")]
    public async Task<IActionResult> Deletar([FromRoute] string codigo)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return BadRequest(new { message = "CodigoLocalInstalacao invalido." });
            }

            var deleted = await _service.DeleteByCodigoAsync(codigo);
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

[ApiController]
[Route("api/V1/Comgas/componentes-equipamento")]
public class ComponentesEquipamentoController : ControllerBase
{
    private readonly ComponenteEquipamentoService _service;
    private readonly IMapper _mapper;
    private readonly string _operador = "componente de equipamento";

    public ComponentesEquipamentoController(ComponenteEquipamentoService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ComponenteEquipamentoComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var componente = _mapper.Map<ComponenteEquipamentoModel>(dto);
            if (componente.IdComponente <= 0)
            {
                return BadRequest(new { message = "IdComponente invalido. Envie IdComponente maior que zero." });
            }

            var criado = await _service.CreateAsync(componente);
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
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] ComponenteEquipamentoComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "IdComponente invalido na rota. Envie id maior que zero." });
            }

            if (dto.IdComponente <= 0)
            {
                dto.IdComponente = id;
            }

            if (dto.IdComponente != id)
            {
                return BadRequest(new { message = "Id da rota e IdComponente do corpo devem ser iguais." });
            }

            var componente = _mapper.Map<ComponenteEquipamentoModel>(dto);
            var result = await _service.UpdateNotaAsync(componente);
            return Ok(new { message = $"{_operador} atualizado com sucesso: {result.IdComponente}" });
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

