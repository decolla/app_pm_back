using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services.Manutencao;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using app_pm_back.Data;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Controllers.Ordem;

/// <summary>
/// Endpoints de dado renovação
/// </summary>
[ApiController]
[Route(Config.RouteConfig.BaseRoute + "/dados-renovacao")]
public class DadosRenovacaoController : ControllerBase
{
    private readonly DadosRenovacaoService _dadosRenovacaoService;
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="DadosRenovacaoController"/>.
    /// </summary>
    /// <param name="dadosRenovacaoService"></param>
    /// <param name="mapper"></param>
    public DadosRenovacaoController(AppDbContext context, DadosRenovacaoService dadosRenovacaoService, IMapper mapper)
    {
        _context = context;
        _dadosRenovacaoService = dadosRenovacaoService;
        _mapper = mapper;
    }

    /// <summary>
    /// Cria um dado de renovação a partir de DTO.
    /// </summary>
    /// <param name="dto">DTO de dados renovação</param>
    /// <returns>IActionResult</returns>
    [HttpPost]
    public async Task<IActionResult> CriarDadosRenovacao([FromBody] DadosRenovacaoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var obj = _mapper.Map<DadosRenovacaoModel>(dto);
            var novoObj = await _dadosRenovacaoService.CreateAsync(obj);
            return new ObjectResult(novoObj);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar dados de renovação: {e.Message}." });
        }
    }
    
    /// <summary>
    /// Lista os dados de renovação em formato de página
    /// </summary>
    /// <returns>Lista Paginada</returns>
    [HttpPost("busca")]
    public async Task<IActionResult> ListarDadosRenovacao(        
        [FromQuery] int? pagina = null,
        [FromQuery] int? tamanho = null)
    {
        try
        {
            if (pagina != null && tamanho != null && (pagina <= 0 || tamanho <= 0))
            {
                return BadRequest(new { message = "Os parametros pagina e tamanho devem ser maiores que zero." });
            }
            var dadosRenovacao = await _dadosRenovacaoService.BuscarRenovacaoAsync(pagina,
                tamanho);
            return new ObjectResult(dadosRenovacao);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao listar dados de renovação: {e.Message}." });
        }
    }
    
    /// <summary>
    /// Conta a quantidade total de dados de renovação cadastrados.
    /// </summary>
    /// <returns>Número de dados renovação cadastrados</returns>
    [HttpGet("count")]
    public async Task<IActionResult> ContarDadosRenovacao()
    {
        try
        {
            var count = await _dadosRenovacaoService.CountNotasAsync();
            return new ObjectResult(new { count });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao contar dados de renovação: {e.Message}." });
        }
    }

    /// <summary>
    /// Obtém um dado de renovação específico a partir do id na rota.
    /// </summary>
    /// <param name="id">Id do Dado</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> ObterDadosRenovacaoPorId(int id)
    {
        try
        {
            var dadosRenovacao = await _dadosRenovacaoService.GetNotaByIdAsync(id);
            if (dadosRenovacao == null)
            {
                return NotFound(new { message = $"Dados de renovação com id {id} não encontrado." });
            }
            return new ObjectResult(dadosRenovacao);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao obter dados de renovação por id: {e.Message}." });
        }
    }
    
    /// <summary>
    /// Atualiza um dado de renovação existente a partir de DTO e id na rota.
    /// </summary>
    /// <param name="id">Id do dado a ser atulaizado</param>
    /// <param name="dto">Dto com os dados atualizados</param>
    /// <returns>IActionResult</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarDadosRenovacao(int id, [FromBody] DadosRenovacaoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "Id invalido na rota. Envie id maior que zero." });
            }

            if (dto.IdDados <= 0)
            {
                dto.IdDados = id;
            }

            if (dto.IdDados != id)
            {
                return BadRequest(new { message = "Id da rota e do corpo da requisição devem ser iguais." });
            }

            var ordem = _mapper.Map<DadosRenovacaoModel>(dto);
            if (ordem.IdDados <= 0)
            {
                return BadRequest(new { message = "Id invalido. Envie um Id maior que zero." });
            }

            var result = await _dadosRenovacaoService.UpdateNotaAsync(ordem);
            return Ok(new { message = "Dados Renovação atualizado com sucesso: " + result.IdDados });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao atualizar Dados Renovação ." });
        }
    }
    
    /// <summary>
    /// Deleta um dado de renovação existente a partir do id na rota.
    /// </summary>
    /// <param name="id">Id do dado a ser deletado</param>
    /// <returns>IActionResult</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarDadosRenovacao(int id){
        try
        {
            var deleted = await _dadosRenovacaoService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = "Dados de renovação não encontrado." });

            return Ok(new { message = "Dados de renovação deletado com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao deletar dados de renovação ." });
        }
    }
    
    /// <summary>
    /// Busca todas as notas para preencher a tabela de Dados Renovacao
    /// </summary>
    [HttpPost("RenovacaoNotas")]
    public async Task<IActionResult> GerarRenovacaoNota()
    {
        try
        {
            var idNotas = await _context.PM_Notas
                .Select(n => n.IdNota)
                .ToListAsync();
            var idItens = await _context.PM_ItensNota
                .Select(n => n.IdItemNota)
                .ToListAsync();
            var dadosRenovacao = await _dadosRenovacaoService.ImportarDadosNotaAsync(idNotas, idItens);
            return Ok("Notas classificadas com sucesso");
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao listar dados de renovação: {e.Message}." });
        }
    }
        
    /// <summary>
    /// Busca todas as ordens para preencher a tabela de Dados Renovacao
    /// </summary>
    [HttpPost("RenovacaoOrdens")]
    public async Task<IActionResult> GerarRenovacaoOrdem()
    {
        try
        {
            var idsOrdens = await _context.PM_Ordens
                .Select(n => n.IdOrdem)
                .ToListAsync();
            var dadosRenovacao = await _dadosRenovacaoService.ImportarDadosOrdemAsync(idsOrdens);
            return Ok("Notas classificadas com sucesso");
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao listar dados de renovação: {e.Message}." });
        }
    }
}