using app_pm_back.DTO.ComgasDTO;
using app_pm_back.Data;
using app_pm_back.Models.Domain;
using app_pm_back.Services.CongasServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Controllers.ControllerPlanoManutencao;

/// <summary>
/// Endpoints de tipo de manutencao da Comgas.
/// </summary>
[ApiController]
[Route("api/V1/Comgas/tipo-manutencao")]
public class TipoManutencaoController : ControllerBase
{
    private readonly string _operador = "tipo de manutenção";
    private readonly TipoManutencaoService _tipoManutencaoService;
    private readonly IMapper _mapper;

    public TipoManutencaoController(TipoManutencaoService tipoManutencaoService, IMapper mapper)
    {
        _tipoManutencaoService = tipoManutencaoService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] TipoManutencaoComgasDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var obj = _mapper.Map<TipoManutencaoModel>(dto);
            var novoObj = await _tipoManutencaoService.CreateAsync(obj);
            return new ObjectResult(novoObj);
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
                var itensPaginados = await _tipoManutencaoService.GetNotasPaginadasAsync(p, t);
                return Ok(itensPaginados);
            }

            var itens = await _tipoManutencaoService.GetAllNotasAsync();
            return Ok(itens);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar {_operador}: {e.Message}" });
        }
    }

    [HttpGet("count")]
    public async Task<IActionResult> Contar()
    {
        try
        {
            var total = await _tipoManutencaoService.CountNotasAsync();
            return Ok(new { count = total });
        }
        catch (Exception)
        {
            return NotFound(new { message = $"Erro ao procurar {_operador}." });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        try
        {
            var obj = await _tipoManutencaoService.GetCompletoByIdAsync(id);
            if (obj == null) return NotFound(new { message = $"{_operador} não encontrado(a)." });

            return Ok(obj);
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao procurar {_operador}." });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] TipoManutencaoComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "IdTipoManutencao invalido na rota. Envie id maior que zero." });
            }

            if (dto.IdTipoManutencao <= 0)
            {
                dto.IdTipoManutencao = id;
            }

            if (id != dto.IdTipoManutencao)
            {
                return BadRequest(new { message = "Id da rota e Id do objeto no corpo da requisição devem ser iguais, ou o do dto = 0." });
            }

            var obj = _mapper.Map<TipoManutencaoModel>(dto);
            var result = await _tipoManutencaoService.UpdateBasicoAsync(obj);
            return Ok(new { message = $"{_operador} atualizado com sucesso: " + result.IdTipoManutencao });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao atualizar {_operador}." });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deletar(int id)
    {
        try
        {
            var deleted = await _tipoManutencaoService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = $"{_operador} não encontrado(a)." });

            return Ok(new { message = $"{_operador} deletado(a) com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao deletar um(a) {_operador}." });
        }
    }

    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }
}

/// <summary>
/// Endpoints de tipo de plano da Comgas.
/// </summary>
[ApiController]
[Route("api/V1/Comgas/tipo-plano")]
public class TipoPlanoController : ControllerBase
{
    private readonly string _operador = "tipo de plano";
    private readonly TipoPlanoService _tipoPlanoService;
    private readonly IMapper _mapper;

    public TipoPlanoController(TipoPlanoService tipoPlanoService, IMapper mapper)
    {
        _tipoPlanoService = tipoPlanoService;
        _mapper = mapper;
    }

    [HttpPost("{tipo:int}")]
    public async Task<IActionResult> Criar(
        [FromRoute] int tipo,
        [FromBody] TipoPlanoCongasDto dto,
        [FromServices] AppDbContext context)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (tipo != 0 && tipo != 1)
        {
            return BadRequest(new
                { message = "Rota inválida. Use /tipo-plano/0 ou /tipo-plano/1" });
        }

        if (!dto.IdTipoEstrategia.HasValue && !dto.IdPlano.HasValue)
        {
            return BadRequest(new
            {
                message = "Regra de Negócio: informe PlanoManutencaoId quando nao houver TipoEstrategiaId."
            });
        }

        if (dto.IdPlano.HasValue)
        {
            var planoExiste = await context.PM_Planos
                .AnyAsync(p => p.IdPlano == dto.IdPlano.Value);

            if (!planoExiste)
            {
                return NotFound(new { message = "O Plano de Manutenção informado não existe." });
            }
        }

        if (dto.IdPlano.HasValue && dto.IdTipoEstrategia.HasValue)
        {
            var plano = await context.PM_Planos
                .FirstOrDefaultAsync(p => p.IdPlano == dto.IdPlano);

            if (plano == null)
                return NotFound(new { message = "O Plano de Manutenção informado não existe." });

            var tipoEstrategiaValido = await context.PM_TiposEstrategia
                .AnyAsync(te => te.IdTipoEstrategia == dto.IdTipoEstrategia && te.IdEstrategia == plano.IdEstrategia);

            if (!tipoEstrategiaValido)
            {
                return BadRequest(new
                {
                    message =
                        "Regra de Negócio: O Tipo de Estratégia selecionado não pertence à Estratégia vinculada a este Plano de Manutenção."
                });
            }
        }

        try
        {
            var obj = _mapper.Map<TipoPlanoModel>(dto);
            var novoObj = await _tipoPlanoService.CreateComCopiaAsync(obj);
            return new ObjectResult(novoObj);
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
                var acoesPaginadas = await _tipoPlanoService.GetNotasPaginadasAsync(p, t);
                return Ok(acoesPaginadas);
            }

            var acoes = await _tipoPlanoService.GetAllNotasAsync();
            return Ok(acoes);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar {_operador}: {e.Message}" });
        }
    }

    [HttpGet("count")]
    public async Task<IActionResult> Contar()
    {
        try
        {
            var notas = await _tipoPlanoService.CountNotasAsync();
            return Ok(new { count = notas });
        }
        catch (Exception)
        {
            return NotFound(new { message = $"Erro ao procurar {_operador}." });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        try
        {
            var obj = await _tipoPlanoService.GetCompletoByIdAsync(id);
            if (obj == null) return NotFound(new { message = $"{_operador} não encontrado(a)." });

            return Ok(obj);
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao procurar {_operador}." });
        }
    }

    [HttpGet("tipado/{tipo:int}")]
    public async Task<IActionResult> ListarPorTipo([FromRoute] int tipo)
    {
        try
        {
            if (tipo != 0 && tipo != 1)
            {
                return BadRequest(new
                    { message = "Rota inválida. Use /tipo-plano/tipado/0 ou /tipo-plano/tipado/1" });
            }
            CategoriaTipoEstrategia tipagem = (CategoriaTipoEstrategia)tipo;
            var acoes = await _tipoPlanoService.GetTodosByTipo(tipagem.ToString());
            return Ok(acoes);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar {_operador}: {e.Message}" });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] TipoPlanoCongasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "IdTipoManutencao invalido na rota. Envie id maior que zero." });
            }

            if (dto.IdTipoManutencao <= 0)
            {
                dto.IdTipoManutencao = id;
            }

            if (id != dto.IdTipoManutencao)
            {
                return BadRequest(new { message = "Id da rota e Id do objeto no corpo da requisição devem ser iguais, ou o do dto = 0." });
            }

                if (!dto.IdTipoEstrategia.HasValue && !dto.IdPlano.HasValue)
            {
                return BadRequest(new
                {
                    message = "Regra de Negócio: informe PlanoManutencaoId quando nao houver TipoEstrategiaId."
                });
            }

            var obj = _mapper.Map<TipoPlanoModel>(dto);
            if (obj.IdTipoPlano <= 0)
            {
                return BadRequest(new
                    { message = "IdTipoPlano invalido. Envie IdTipoPlano maior que zero." });
            }

            var result = await _tipoPlanoService.UpdateComListasAsync(obj);
            return Ok(new { message = $"{_operador} atualizado com sucesso: " + result.IdTipoPlano });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao atualizar {_operador}." });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deletar(int id)
    {
        try
        {
            var deleted = await _tipoPlanoService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = $"{_operador} não encontrado(a)." });

            return Ok(new { message = $"{_operador} deletado(a) com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao deletar um(a) {_operador}." });
        }
    }

    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }
}