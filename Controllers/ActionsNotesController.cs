using app_pm_back.DTO.ComgasDTO;
using app_pm_back.Models.CongasModel;
using app_pm_back.Services.CongasServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

/// <summary>
/// Controller para Ações de Notas 
/// </summary>
[ApiController]
[Route("api/V1/Comgas/actions-notes")]
public class ActionsNotesController : ControllerBase
{
    private readonly AcaoNotaComgasService _acaoNotaService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="acaoNotaService"></param>
    /// <param name="mapper"></param>
    public ActionsNotesController(AcaoNotaComgasService acaoNotaService, IMapper mapper)
    {
        _acaoNotaService = acaoNotaService;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Cria uma ação de nota 
    /// </summary>
    /// <param name="dto">Ação a ser criada</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CriarAcaoNota([FromBody] AcaoNotaCongasComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var acao = _mapper.Map<AcaoNotaCongasModel>(dto);
            var novaAcao = await _acaoNotaService.CreateAsync(acao);
            return Created($"api/V1/Comgas/actions-notes/{novaAcao.IdAcao}", novaAcao);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar a ação de notas: {e.Message}." });
        }
    }

    /// <summary>
    /// Exibe uma lista de ação de nota
    /// </summary>
    /// <param name="pagina">Pagina a ser exibida</param>
    /// <param name="tamanho">Tamanho de ações da página</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> ListarAcaoDeNotas([FromQuery] int? pagina, [FromQuery] int? tamanho)
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
                var acoesPaginadas = await _acaoNotaService.GetNotasPaginadasAsync(p, t);
                return Ok(acoesPaginadas);
            }

            var acoes = await _acaoNotaService.GetAllNotasAsync();
            return Ok(acoes);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar ações: {e.Message}" });
        }
    }

    /// <summary>
    /// Endpoint para a quantidade de ações de nota cadastradas
    /// </summary>
    /// <returns>Quantidade de ações cadastradas</returns>
    [HttpGet("count")]
    public async Task<IActionResult> ContarAcoes()
    {
        try
        {
            var notas = await _acaoNotaService.CountNotasAsync();
            return Ok(new { count = notas });
        }
        catch (Exception)
        {
            return NotFound(new { message = "Erro ao procurar ações." });
        }
    }

    /// <summary>
    /// Atualiza uma ação de nota
    /// </summary>
    /// <param name="id">Id da acao a ser atualizada</param>
    /// <param name="dto">Ação a ser atualizada</param>
    /// <returns>Ação atualizada</returns>
   // [HttpPut]
   [HttpPut("{id:int}")]
    public async Task<IActionResult> AtualizarAcao([FromRoute] int id, [FromBody] AcaoNotaCongasComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "Id da acao invalido na rota. Envie id maior que zero." });
            }

            if (dto.IdAcao <= 0)
            {
                dto.IdAcao = id;
            }

            if (dto.IdAcao != id)
            {
                return BadRequest(new { message = "Id da rota e IdAcao do corpo da requisição devem ser iguais." });
            }

            var acao = _mapper.Map<AcaoNotaCongasModel>(dto);
            if (acao.IdAcao <= 0)
            {
                return BadRequest(new { message = "Id da acao invalido. Envie IdAcao maior que zero." });
            }

            var result = await _acaoNotaService.UpdateNotaAsync(acao);
            return Ok(new { message = "Ação atualizada com sucesso: " + result.IdAcao });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao atualizar a ação." });
        }
    }
    
    /// <summary>
    /// Deleta uma ação de nota
    /// </summary>
    /// <param name="id">/Id da ação a ser deletada</param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletarAcao(int id)
    {
        try
        {
            var deleted = await _acaoNotaService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = "Ação não encontrada." });

            return Ok(new { message = "Ação deletada com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao deletar uma ação de nota." });
        }
    }

    /// <summary>
    /// Valida pagina e tamanho para evitar consultas inconsistentes.
    /// </summary>
    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }
}