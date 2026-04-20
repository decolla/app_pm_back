using app_pm_back.DTO.ComgasDTO;
using app_pm_back.Models.CongasModel;
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
    private readonly string _operador = "tipo de mantuenção";
    private readonly TipoManutencaoService _tipoManutencaoService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Construtor.
    /// </summary>
    /// <param name="tipoManutencaoService"></param>
    /// <param name="mapper"></param>
    public TipoManutencaoController(TipoManutencaoService tipoManutencaoService, IMapper mapper)
    {
        _tipoManutencaoService = tipoManutencaoService;
        _mapper = mapper;
    }

    /// <summary>
    /// Cria um tipo de manutenção respeitando a hierarquia da Estratégia
    /// </summary>
    /// <param name="dto">Objeto a ser adicionado</param>
    /// <param name="context">Contexto injetado para validação</param>
    [HttpPost]
    public async Task<IActionResult> Criar(
        [FromBody] TipoManutencaoComgasDto dto,
        [FromServices] app_pm_back.Data.AppDbContext context)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        // validação da arquitetura da hierarquia
        if (dto.PlanoManutencaoId.HasValue && dto.TipoEstrategiaId.HasValue)
        {
            // busca o plano com a estratégia associada
            var plano = await context.PlanosManutencaoNormalizados
                .Include(p => p.Estrategia)
                .ThenInclude(e => e!.Itens) // Itens é a coleção de TipoEstrategiaModel no seu modelo
                .FirstOrDefaultAsync(p => p.CodigoPlano == dto.PlanoManutencaoId);

            if (plano == null)
                return NotFound(new { message = "O Plano de Manutenção informado não existe." });

            if (plano.Estrategia == null)
                return BadRequest(new { message = "Este Plano de Manutenção não possui uma Estratégia vinculada." });

            // valida se o TipoEstrategiaId enviado pertence à Estratégia do Plano
            var tipoEstrategiaValido = plano.Estrategia.Itens
                .Any(te => te.IdTipoEstrategia == dto.TipoEstrategiaId);

            if (!tipoEstrategiaValido)
            {
                return BadRequest(new
                {
                    message =
                        "Regra de Negócio: O Tipo de Estratégia selecionado não pertence à Estratégia vinculada a este Plano de Manutenção."
                });
            }
        }

        // CRIAÇÃO DE FATO
        try
        {
            var obj = _mapper.Map<TipoManutencaoModel>(dto);
            var novoObj = await _tipoManutencaoService.CreateAsync(obj);
            // URL corrigida
            return Created($"api/V1/Comgas/tipo-manutencao/{novoObj.IdTipoManutencao}", novoObj);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar {_operador}: {e.Message}." });
        }
    }

    /// <summary>
    /// Lista as estratégias em formato de página 
    /// </summary>
    /// <param name="pagina">Página a ser exibida</param>
    /// <param name="tamanho">Quantidade de objetos na página</param>
    /// <returns>Lista de objetos presentes na página</returns>
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
                var acoesPaginadas = await _tipoManutencaoService.GetNotasPaginadasAsync(p, t);
                return Ok(acoesPaginadas);
            }

            var acoes = await _tipoManutencaoService.GetAllNotasAsync();
            return Ok(acoes);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar {_operador}: {e.Message}" });
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
            var notas = await _tipoManutencaoService.CountNotasAsync();
            return Ok(new { count = notas });
        }
        catch (Exception)
        {
            return NotFound(new { message = $"Erro ao procurar {_operador}." });
        }
    }

    /// <summary>
    /// Atualiza um objeto do modelo
    /// </summary>
    /// <param name="dto">Objeto a ser atualizado</param>
    /// <returns>Novo objeto</returns>
    [HttpPut]
    public async Task<IActionResult> Atualizar([FromBody] TipoManutencaoComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var obj = _mapper.Map<TipoManutencaoModel>(dto);
            if (obj.IdTipoManutencao <= 0)
            {
                return BadRequest(new { message = "IdTipoManutencao invalido. Envie IdTipoManutencao maior que zero." });
            }

            var result = await _tipoManutencaoService.UpdateNotaAsync(obj);
            return Ok(new { message = $"{_operador} atualizada com sucesso: " + result.IdTipoManutencao });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao atualizar {_operador}." });
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
            var deleted = await _tipoManutencaoService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = $"{_operador} não encontrado(a)." });

            return Ok(new { message = $"{_operador} deletado(a) com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao deletar um(a) {_operador}." });
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