using app_pm_back.DTO.ComgasDTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using app_pm_back.Services.CongasServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

/// <summary>
/// Endpoints de ordens da Comgas.
/// </summary>
[ApiController]
[Route("api/V1/Comgas/orders")]
public class ComgasOrdersController : ControllerBase
{
    private readonly OrdemService _congasService;
    private readonly ClassificationService<OrdemModel> _classificationServiceCongas;
    private readonly IMapper _mapper;

    public ComgasOrdersController(
        OrdemService congasService,
        ClassificationService<OrdemModel> classificationServiceCongas,
        IMapper mapper
    )
    {
        _congasService = congasService;
        _classificationServiceCongas = classificationServiceCongas;
        _mapper = mapper;
    }

    /// <summary>
    /// Cadastra uma ordem Comgas a partir de DTO.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CriarOrdemCongas([FromBody] OrdemManutencaoComgasDto dto)
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

            var notaCriada = await _congasService.CreateAsync(ordem);
            return Created($"api/V1/Comgas/orders/{notaCriada.IdOrdem}", notaCriada);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar a ordem Comgas: {e.Message}." });
        }
    }

    /// <summary>
    /// Retorna todas as ordens Comgas, com paginacao opcional.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListarOrdensCongas([FromQuery] int? pagina, [FromQuery] int? tamanho)
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
                var notasPaginadas = await _congasService.GetNotasPaginadasAsync(p, t);
                return Ok(notasPaginadas);
            }

            var todasNotas = await _congasService.GetAllNotasAsync();
            return Ok(todasNotas);
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao processar ordens Comgas." });
        }
    }

    /// <summary>
    /// Conta o número total de notas Congas salvas no banco
    /// </summary>
    [HttpGet("count")]
    public async Task<IActionResult> ContarNotasCongas()
    {
        try
        {
            var notas = await _congasService.CountNotasAsync();
            return Ok(new { count = notas });
        }
        catch (Exception)
        {
            return NotFound(new { message = "Erro ao procurar ordens Comgas." });
        }
    }

    /// <summary>
    /// Retorna as Fichas de uma nota Congas e suas ocorrências de mesmo tipo
    /// </summary>
    [HttpGet("ativo/{dnsIoT}")]
    public async Task<IActionResult> ListarFichasPorAtivoCongas([FromRoute] int dnsIoT, [FromQuery] string? type)
    {
        try
        {
            if (dnsIoT <= 0)
            {
                return BadRequest(new { message = "Parametro dnsIoT invalido." });
            }

            var resultado = await _congasService.GetFichasPorAtivoAsync(dnsIoT, type);
            return Ok(resultado);
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao buscar ordens do ativo Comgas." });
        }
    }

    /// <summary>
    /// Atualiza uma ordem Comgas pelo DTO.
    /// </summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> AtualizarNotaCongas([FromRoute] long id, [FromBody] OrdemManutencaoComgasDto dto)
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

            var result = await _congasService.UpdateNotaAsync(ordem);
            return Ok(new { message = "Ordem Comgas atualizada com sucesso: " + result.IdOrdem });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao atualizar ordem Comgas." });
        }
    }

    /// <summary>
    /// Delete uma nota Congas de mesmo Id
    /// </summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeletarNotaCongas(long id)
    {
        try
        {
            var deleted = await _congasService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = "Ordem Comgas não encontrada." });

            return Ok(new { message = "Ordem Comgas deletada com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao deletar ordem Comgas." });
        }
    }

    /// <summary>
    /// Classifica uma ordem Congas with manual label
    /// </summary>
    [HttpPut("classificacao/{id:long}")]
    public async Task<IActionResult> ClassificacaoHumanaNotaCongas(
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

            var result = await _classificationServiceCongas.HumanClassification(id, classificacao, idUsuario);
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
    public async Task<IActionResult> ClassificacaoPalavrasNotaCongas()
    {
        try
        {
            var result = await _classificationServiceCongas.WordsClassification([]);
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
    public async Task<IActionResult> ClassificacaoCatboostCongas()
    {
        try
        {
            var result = await _classificationServiceCongas.CatboostClassification();
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