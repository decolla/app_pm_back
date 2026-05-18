using app_pm_back.DTO.ComgasDTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services.CongasServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

[ApiController]
[Route("api/V1/Comgas/analises-risco")]
public class AnalisesRiscoController : ControllerBase
{
    private readonly AnaliseRiscoService _service;
    private readonly IMapper _mapper;
    private readonly string _operador = "analise de risco";

    public AnalisesRiscoController(AnaliseRiscoService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] AnaliseRiscoComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var analise = _mapper.Map<AnaliseRiscoModel>(dto);
            var criado = await _service.CreateAsync(analise);
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
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] AnaliseRiscoComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "IdAnaliseRisco invalido na rota. Envie id maior que zero." });
            }

            var analise = _mapper.Map<AnaliseRiscoModel>(dto);
            analise.IdAnaliseRisco = id;

            var result = await _service.UpdateNotaAsync(analise);
            return Ok(new { message = $"{_operador} atualizado com sucesso: {result.IdAnaliseRisco}" });
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

[ApiController]
[Route("api/V1/Comgas/analises-risco-operacional")]
public class AnalisesRiscoOperacionalController : ControllerBase
{
    private readonly AnaliseRiscoOperacionalService _service;
    private readonly IMapper _mapper;
    private readonly string _operador = "analise de risco operacional";

    public AnalisesRiscoOperacionalController(AnaliseRiscoOperacionalService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] AnaliseRiscoOperacionalComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var analise = _mapper.Map<AnaliseRiscoOperacionalModel>(dto);
            var criado = await _service.CreateAsync(analise);
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
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] AnaliseRiscoOperacionalComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "IdAnaliseRiscoOperacional invalido na rota. Envie id maior que zero." });
            }

            var analise = _mapper.Map<AnaliseRiscoOperacionalModel>(dto);
            analise.IdAnaliseRiscoOperacional = id;

            var result = await _service.UpdateNotaAsync(analise);
            return Ok(new { message = $"{_operador} atualizado com sucesso: {result.IdAnaliseRiscoOperacional}" });
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

[ApiController]
[Route("api/V1/Comgas/permissoes-trabalho")]
public class PermissoesTrabalhoController : ControllerBase
{
    private readonly PermissaoTrabalhoService _service;
    private readonly IMapper _mapper;
    private readonly string _operador = "permissao de trabalho";

    public PermissoesTrabalhoController(PermissaoTrabalhoService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] PermissaoTrabalhoComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var permissao = _mapper.Map<PermissaoTrabalhoModel>(dto);
            var criado = await _service.CreateAsync(permissao);
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
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] PermissaoTrabalhoComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "IdPermissaoTrabalho invalido na rota. Envie id maior que zero." });
            }

            var permissao = _mapper.Map<PermissaoTrabalhoModel>(dto);
            permissao.IdPermissaoTrabalho = id;

            var result = await _service.UpdateNotaAsync(permissao);
            return Ok(new { message = $"{_operador} atualizado com sucesso: {result.IdPermissaoTrabalho}" });
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

[ApiController]
[Route("api/V1/Comgas/matrizes-libra")]
public class MatrizesLibraController : ControllerBase
{
    private readonly MatrizLibraService _service;
    private readonly IMapper _mapper;
    private readonly string _operador = "matriz libra";

    public MatrizesLibraController(MatrizLibraService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] MatrizLibraComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var matriz = _mapper.Map<MatrizLibraModel>(dto);
            var criado = await _service.CreateAsync(matriz);
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
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] MatrizLibraComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "IdMatrizLibra invalido na rota. Envie id maior que zero." });
            }

            var matriz = _mapper.Map<MatrizLibraModel>(dto);
            matriz.IdMatrizLibra = id;

            var result = await _service.UpdateNotaAsync(matriz);
            return Ok(new { message = $"{_operador} atualizado com sucesso: {result.IdMatrizLibra}" });
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

[ApiController]
[Route("api/V1/Comgas/operacoes-ordem")]
public class OperacoesOrdemController : ControllerBase
{
    private readonly OperacaoOrdemService _service;
    private readonly IMapper _mapper;
    private readonly string _operador = "operacao de ordem";

    public OperacoesOrdemController(OperacaoOrdemService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] OperacaoOrdemComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var operacao = _mapper.Map<OperacaoOrdemModel>(dto);
            if (operacao.IdOperacao <= 0)
            {
                return BadRequest(new { message = "IdOperacao invalido. Envie IdOperacao maior que zero." });
            }

            var criado = await _service.CreateAsync(operacao);
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
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] OperacaoOrdemComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "IdOperacao invalido na rota. Envie id maior que zero." });
            }

            if (dto.IdOperacao <= 0)
            {
                dto.IdOperacao = id;
            }

            if (dto.IdOperacao != id)
            {
                return BadRequest(new { message = "Id da rota e IdOperacao do corpo devem ser iguais." });
            }

            var operacao = _mapper.Map<OperacaoOrdemModel>(dto);
            var result = await _service.UpdateNotaAsync(operacao);
            return Ok(new { message = $"{_operador} atualizado com sucesso: {result.IdOperacao}" });
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

