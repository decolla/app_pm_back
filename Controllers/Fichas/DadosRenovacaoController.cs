using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services.FichasServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using app_pm_back.Data;
using Microsoft.EntityFrameworkCore;


namespace app_pm_back.Controllers.FichasControllers;

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
            var count = await _dadosRenovacaoService.CountAsync();
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
    /// <param name="id">Identificador exclusivo do registro (deve ser maior que zero).</param>
    /// <returns>O objeto correspondente cadastrado.</returns>
    /// <response code="200">Sucesso ao obter o registro.</response>
    /// <response code="400">ID fornecido inválido.</response>
    /// <response code="404">Registro não encontrado no banco de dados.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterDadosRenovacaoPorId(int id)
    {
        try
        {
            var dadosRenovacao = await _dadosRenovacaoService.GetByIdAsync(id);
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
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeletarDadosRenovacao([FromRoute] long id)
    {
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
    /// Busca todas as Fichas, Notas ou Ordens para preencher a tabela de Dados Renovacao
    /// </summary>
    [HttpPost("renovacao-fichas")]
    public async Task<IActionResult> GerarRenovacaoFichas(Fichas buscaTabela, CancellationToken cancellationToken)
    {
        try
        {
            var message = "";
            if (buscaTabela == Fichas.Ambas || buscaTabela == Fichas.Notas)
            {
                var idNotas = await _context.PM_Notas
                    .Select(n => n.IdNota)
                    .ToListAsync();
                var idItens = await _context.PM_ItensNota
                    .Select(n => n.IdItemNota)
                    .ToListAsync();
                var dadosRenovacao = await _dadosRenovacaoService.ImportarDadosNotaAsync(idNotas, idItens, cancellationToken);
                message = "Notas classificadas com sucesso";
            }

            if (buscaTabela == Fichas.Ambas || buscaTabela == Fichas.Ordens)
            {
                var idsOrdens = await _context.PM_Ordens
                    .Select(n => n.IdOrdem)
                    .ToListAsync();
                var dadosRenovacao = await _dadosRenovacaoService.ImportarDadosOrdemAsync(idsOrdens, cancellationToken);
                message += "| Ordens classificadas com sucesso";
            }

            return Ok(message);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao listar dados de renovação: {e.Message}." });
        }
    }
    
    /// <summary>
    /// Busca as datas de renovação de um LI e as próximas datas em que ocorrerá uma renovação
    /// </summary>
    /// <param name="idLi">LI (Local de Instalação) a ser procurada</param>
    /// <param name="tipoDados">Tipo de dados da renovação</param>
    /// <returns>Objeto com informações relevantes</returns>
    [HttpGet("datas")]
    public async Task<IActionResult> BuscaAtivoDatas([FromQuery] string idLi, [FromQuery] string? SapId)
    {
        try
        {

            if (string.IsNullOrWhiteSpace(idLi))
            {
                return BadRequest(new { message = "Parametro idLi obrigatorio." });
            }

            var obj = await _dadosRenovacaoService.AtivoDatas(idLi, SapId);
            if (obj == null) return NotFound(new { message = $"Nenhuma busca realizada para o LI informado." });

            return Ok(obj);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar datas para este ativo: {e.Message}." });
        }
    }
    
    /// <summary>
    /// Busca a última data de renovação do LI
    /// </summary>
    [HttpGet("li")]
    public async Task<IActionResult> BuscaLiRenovacao([FromQuery] string idLi, [FromQuery] string? SapId)
    {
        try
        {

            if (string.IsNullOrWhiteSpace(idLi))
            {
                return BadRequest(new { message = "Parametro idLi obrigatorio." });
            }

            var obj = await _dadosRenovacaoService.LiRenovacao(idLi, SapId);
            if (obj == null) return NotFound(new { message = $"Nenhuma busca realizada para o LI informado." });

            return Ok(obj);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar datas para este ativo: {e.Message}." });
        }
    }
    
    /// <summary>
    /// Busca a última data de renovação do LI
    /// </summary>
    [HttpPut("lista-li")]
    public async Task<IActionResult> BuscaListaLi([FromBody] List<NomesLiDto> nomesLi)
    {
        try
        {

            if (nomesLi == null || nomesLi.Count == 0)
                return BadRequest("A lista não pode ser vazia.");

            var obj = await _dadosRenovacaoService.ListaLiRenovacao(nomesLi);
            if (obj == null) return NotFound(new { message = $"Nenhuma busca realizada para o LI informado." });

            return Ok(obj);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar datas para este ativo: {e.Message}." });
        }
    }
}