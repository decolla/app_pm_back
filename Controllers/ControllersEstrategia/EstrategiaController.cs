using app_pm_back.DTO.ComgasDTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services.CongasServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers.ControllersEstrategia;

/// <summary>
/// Endpoints de estrategia de manutencao da Comgas.
/// </summary>
[ApiController]
[Route("api/V1/Comgas/estrategia")]
public class EstrategiaController : ControllerBase
{
    private readonly string _operador = "estratégia de mantuenção";
    private readonly EstrategiaComgasService _estrategiaComgasService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Construtor.
    /// </summary>
    /// <param name="estrategiaComgasService"></param>
    /// <param name="mapper"></param>
    public EstrategiaController(EstrategiaComgasService estrategiaComgasService, IMapper mapper)
    {
        _estrategiaComgasService = estrategiaComgasService;
        _mapper = mapper;
    }

    /// <summary>
    /// Cria uma estratégia 
    /// </summary>
    /// <param name="dto">Objeto a ser adicionado</param>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] EstrategiaManutencaoComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var obj = _mapper.Map<EstrategiaManutencaoModel>(dto);
            if (string.IsNullOrWhiteSpace(obj.Nome))
            {
                return BadRequest(new { message = "Nome da estrategia obrigatorio." });
            }

            var novoObj = await _estrategiaComgasService.CreateAsync(obj);
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
                var acoesPaginadas = await _estrategiaComgasService.GetNotasPaginadasAsync(p, t);
                return Ok(acoesPaginadas);
            }

            var acoes = await _estrategiaComgasService.GetAllNotasAsync();
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
            var notas = await _estrategiaComgasService.CountNotasAsync();
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
    /// <param name="id">Id da estrategia na rota</param>
    /// <param name="dto">Objeto a ser atualizado</param>
    /// <returns>Novo objeto</returns>
    // [HttpPut]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] EstrategiaManutencaoComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "IdEstrategia invalido na rota. Envie id maior que zero." });
            }

            if (dto.IdEstrategia <= 0)
            {
                dto.IdEstrategia = id;
            }

            if (dto.IdEstrategia != id)
            {
                return BadRequest(new { message = "Id da rota e IdEstrategia do corpo da requisição devem ser iguais." });
            }

            var obj = _mapper.Map<EstrategiaManutencaoModel>(dto);
            if (obj.IdEstrategia <= 0)
            {
                return BadRequest(new { message = "IdEstrategia invalido. Envie IdEstrategia maior que zero." });
            }

            var result = await _estrategiaComgasService.UpdateNotaAsync(obj);
            return Ok(new { message = $"{_operador} atualizada com sucesso: " + result.IdEstrategia });
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
            var deleted = await _estrategiaComgasService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = $"{_operador} não encontrado(a)." });

            return Ok(new { message = $"{_operador} deletado(a) com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao deletar um(a) {_operador}." });
        }
    }
    
    /// <summary>
    /// Pesquisa estrategias com filtros da tela (nome, id, tipo de ativo e visao).
    /// </summary>
    /// <param name="nome">Nome da estrategia (parcial).</param>
    /// <param name="id">Id exato da estrategia.</param>
    /// <param name="tipoAtivo">Tipo de ativo aplicavel (parcial).</param>
    /// <param name="visao">Visao/denominacao (parcial).</param>
    /// <param name="pagina">Pagina para retorno paginado (opcional).</param>
    /// <param name="tamanho">Quantidade por pagina (opcional).</param>
    [HttpGet("pesquisa")]
    public async Task<IActionResult> Pesquisar(
        [FromQuery] string? nome,
        [FromQuery] int? id,
        [FromQuery] string? tipoAtivo,
        [FromQuery] string? visao,
        [FromQuery] int? pagina,
        [FromQuery] int? tamanho)
    {
        try
        {
            if (id.HasValue && id <= 0)
            {
                return BadRequest(new { message = "Parametro id deve ser maior que zero." });
            }

            if (TemPaginacaoInvalida(pagina, tamanho))
            {
                return BadRequest(new { message = "Os parametros pagina e tamanho devem ser maiores que zero." });
            }

            var resultados = await _estrategiaComgasService.BuscarAsync(id, nome, tipoAtivo, visao, pagina, tamanho);
            return Ok(resultados);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao pesquisar {_operador}: {e.Message}." });
        }
    }

    /// <summary>
    /// Busca por id a estratégia 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "IdEstrategia invalido na rota. Envie id maior que zero." });
            }

            var obj = await _estrategiaComgasService.GetNotaByIdAsync(id);
            if (obj == null)
            {
                return NotFound(new { message = $"{_operador} não encontrado(a)." });
            }

            return Ok(obj);
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao procurar {_operador}." });
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