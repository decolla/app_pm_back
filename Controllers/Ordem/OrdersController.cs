using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using app_pm_back.Services.Manutencao;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

/// <summary>
/// Endpoints de ordens .
/// </summary>
[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrdemService _ordemService;
    private readonly ClassificationService<OrdemModel> _classificationService;
    private readonly IMapper _mapper;

    public OrdersController(
        OrdemService ordemService,
        ClassificationService<OrdemModel> classificationService,
        IMapper mapper
    )
    {
        _ordemService = ordemService;
        _classificationService = classificationService;
        _mapper = mapper;
    }

    /// <summary>
    /// Cadastra uma ordem a partir de DTO.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CriarOrdem([FromBody] OrdemManutencaoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var ordem = _mapper.Map<OrdemModel>(dto);
            if (ordem.IdOrdem <= 0)
            {
                return BadRequest(new { message = "Id da ordem invalido. Envie IdOrdem maior que zero." });
            }

            var notaCriada = await _ordemService.CreateAsync(ordem);
            return Created($"{app_pm_back.Config.RouteConfig.BaseRoute}/orders/{notaCriada.IdOrdem}", notaCriada);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar a ordem : {e.Message}." });
        }
    }

    /// <summary>
    /// Retorna todas as ordens , com paginacao opcional.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListarOrdens([FromQuery] int? pagina, [FromQuery] int? tamanho)
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
                var notasPaginadas = await _ordemService.GetNotasPaginadasAsync(p, t);
                return Ok(notasPaginadas);
            }

            var todasNotas = await _ordemService.GetAllNotasAsync();
            return Ok(todasNotas);
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao processar ordens ." });
        }
    }

    /// <summary>
    /// Conta o número total de notas salvas no banco
    /// </summary>
    [HttpGet("count")]
    public async Task<IActionResult> ContarNotas()
    {
        try
        {
            var notas = await _ordemService.CountNotasAsync();
            return Ok(new { count = notas });
        }
        catch (Exception)
        {
            return NotFound(new { message = "Erro ao procurar ordens ." });
        }
    }

    /// <summary>
    /// Retorna as Fichas de uma nota e suas ocorrências de mesmo tipo
    /// </summary>
    [HttpGet("ativo/{dnsIoT}")]
    public async Task<IActionResult> ListarFichasPorAtivo([FromRoute] int dnsIoT, [FromQuery] string? type)
    {
        try
        {
            if (dnsIoT <= 0)
            {
                return BadRequest(new { message = "Parametro dnsIoT invalido." });
            }

            var resultado = await _ordemService.GetFichasPorAtivoAsync(dnsIoT, type);
            return Ok(resultado);
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao buscar ordens do ativo ." });
        }
    }

    /// <summary>
    /// Atualiza uma ordem pelo DTO.
    /// </summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> AtualizarOrdem([FromRoute] long id, [FromBody] OrdemManutencaoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "Id da ordem invalido na rota. Envie id maior que zero." });
            }

            if (dto.IdOrdem <= 0)
            {
                dto.IdOrdem = id;
            }

            if (dto.IdOrdem != id)
            {
                return BadRequest(new { message = "Id da rota e IdOrdem do corpo da requisição devem ser iguais." });
            }

            var ordem = _mapper.Map<OrdemModel>(dto);
            if (ordem.IdOrdem <= 0)
            {
                return BadRequest(new { message = "Id da ordem invalido. Envie IdOrdem maior que zero." });
            }

            var result = await _ordemService.UpdateNotaAsync(ordem);
            return Ok(new { message = "Ordem atualizada com sucesso: " + result.IdOrdem });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao atualizar ordem ." });
        }
    }

    /// <summary>
    /// Delete uma nota de mesmo Id
    /// </summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeletarOrdem(long id)
    {
        try
        {
            var deleted = await _ordemService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = "Ordem não encontrada." });

            return Ok(new { message = "Ordem deletada com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao deletar ordem ." });
        }
    }

    /// <summary>
    /// Classifica uma ordem with manual label
    /// </summary>
    [HttpPut("classificacao/{id:long}")]
    public async Task<IActionResult> ClassificacaoHumanaOrdem(
        [FromRoute] long id,
        [FromQuery] string classificacao,
        [FromQuery] string idUsuario)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(classificacao) || string.IsNullOrWhiteSpace(idUsuario))
            {
                return BadRequest(new { message = "Parametros classificacao e idUsuario sao obrigatorios." });
            }

            var result = await _classificationService.HumanClassification(id, classificacao, idUsuario);
            if (result == null)
            {
                return NotFound(new { message = "Ordem não encontrada para classificação." });
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao classificar esta ordem: {e.Message}" });
        }
    }

    /// <summary>
    /// Classifica uma ordem com base uma lista de palavras armazenadas no banco
    /// </summary>
    [HttpPost("classificacao/filtro")]
    public async Task<IActionResult> ClassificacaoPalavrasOrdem()
    {
        try
        {
            var result = await _classificationService.WordsClassification([]);
            if (result.Count > 0)
            {
                return Ok(result);
            }

            return BadRequest("Não foram achadas ordem para classificar");
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao classificar ordem: {e.Message}" });
        }
    }

    /// <summary>
    /// Classifica as ordens usando a IA catboost
    /// </summary>
    [HttpPost("classificacao/catboost")]
    public async Task<IActionResult> ClassificacaoCatboostOrdem()
    {
        try
        {
            var result = await _classificationService.CatboostClassification();
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao classificar ordens: {e.Message}" });
        }
    }

    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }
}