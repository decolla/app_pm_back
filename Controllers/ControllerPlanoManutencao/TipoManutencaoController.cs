using app_pm_back.DTO.ComgasDTO;
using app_pm_back.Data;
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
    /// <param name="tipo">Tipo do tipo a ser criado, 0 = manutenção, 1 = renovação </param>
    [HttpPost("{tipo:int}")]
    public async Task<IActionResult> Criar(
        [FromRoute] int tipo,
        [FromBody] TipoManutencaoComgasDto dto,
        [FromServices] AppDbContext context)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        
        dto.TipoTipo = tipo;

        if (dto.TipoTipo == 0 && (dto.ItensRenovacao?.Any() ?? false))
        {
            return BadRequest(new { message = "Erro: Um cadastro de MANUTENÇÃO não pode conter Itens de Renovação." });
        }
        
        if (dto.TipoTipo == 1 && (dto.ListaTarefas?.Any() ?? false))
        {
            return BadRequest(new { message = "Erro: Um cadastro de RENOVAÇÃO não pode conter uma lista de tarefas." });
        }

        if (dto.TipoTipo == 1 && (dto.Ferramentas?.Any() ?? false))
        {
            return BadRequest(new { message = "Erro: Um cadastro de RENOVAÇÃO não pode conter Ferramentas." });
        }
        
        if (tipo != 0 && tipo != 1)
        {
            return BadRequest(new
                { message = "Rota inválida. Use /tipo-manutencao/0 ou /tipo-manutencao/1" });
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
    /// Realiza uma busca por Id e retorna o objeto completo, com as listas filhas carregadas.
    /// </summary>
    /// <param name="id">Id a ser buscado</param>
    /// <returns>Objeto completo</returns>
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
    
    /// <summary>
    /// Lista todos os tipos com base em um tipo específico
    /// </summary>
    /// <param name="tipo">Tipo do tipo a ser criado, 0 = manutenção, 1 = renovação</param>
    /// <returns>Lista de tipos</returns>
    [HttpGet("tipado/{tipo:int}")]
    public async Task<IActionResult> ListarPorTipo([FromRoute] int tipo)
    {
        try
        {
            if (tipo != 0 && tipo != 1)
            {
                return BadRequest(new
                    { message = "Rota inválida. Use /tipo-manutencao/tipado/0 ou /tipo-manutencao/tipado/1" });
            }
            TipoTipo tipagem = (TipoTipo)tipo;
            var acoes = await _tipoManutencaoService.GetTodosByTipo(tipagem);
            return Ok(acoes);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar {_operador}: {e.Message}" });
        }
    }

    /// <summary>
    /// Atualiza um tipo de manutenção.
    /// </summary>
    /// <param name="id">Id do DTO a ser atualizado</param>
    /// <param name="dto">DTO do tipo a ser atualizado</param>
    /// <returns></returns>
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
            
            if (id != dto.IdTipoManutencao)            {
                return BadRequest(new { message = "Id da rota e Id do objeto no corpo da requisição devem ser iguais, ou o do dto = 0." });
            }

            var obj = _mapper.Map<TipoManutencaoModel>(dto);
            if (obj.IdTipoManutencao <= 0)
            {
                return BadRequest(new
                    { message = "IdTipoManutencao invalido. Envie IdTipoManutencao maior que zero." });
            }

            var result = await _tipoManutencaoService.UpdateComListasAsync(obj);
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