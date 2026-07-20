using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using app_pm_back.Services.FichasServices;
using app_pm_back.Services.Manutencao;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace app_pm_back.Controllers;

/// <summary>
/// Endpoints de notas .
/// </summary>
[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/notas")]
public class NotasController : ControllerBase
{
    private readonly DadosRenovacaoService _dadosRenovacaoService;
    private readonly NotaService _notaService;
    private readonly ItemNotaService _itemNotaService;
    private readonly ClassificationService<NotaModel> _classificationService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="NotasController"/>.
    /// </summary>
    public NotasController(
        NotaService notaService,
        ItemNotaService itemNotaService,
        DadosRenovacaoService dadosRenovacaoService,
        ClassificationService<NotaModel> classificationService,
        IMapper mapper)
    {
        _notaService = notaService;
        _itemNotaService = itemNotaService;
        _classificationService = classificationService;
        _dadosRenovacaoService = dadosRenovacaoService;
        _mapper = mapper;
    }

    /// <summary>
    /// Cadastra uma nota a partir de DTO.
    /// </summary>
    /// <param name="dto">DTO contendo os dados da requisição.</param>
    /// <returns>O objeto criado com status 201 (Created).</returns>
    /// <response code="201">Registro criado com sucesso.</response>
    /// <response code="400">Erros na validação do modelo ou dados fornecidos inválidos.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CriarNota([FromBody] NotaDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var novaNota = _mapper.Map<NotaModel>(dto);

            // Normalização de chaves estrangeiras
            if (novaNota.IdPlano <= 0) novaNota.IdPlano = null;
            if (novaNota.IdUsuario <= 0) novaNota.IdUsuario = null;
            if (novaNota.IdGpm <= 0) novaNota.IdGpm = null;

            var notaCriada = await _notaService.CreateAsync(novaNota);

            if (notaCriada.NumeroNota <= 0)
            {
                notaCriada.NumeroNota = notaCriada.IdNota;
                await _notaService.UpdateAsync(notaCriada);
            }

            if (notaCriada.Classificacao != Classificacao.NAOCLASSIFICADA)
            {
                await _classificationService.HumanClassification(notaCriada.IdNota, notaCriada.Classificacao,
                    notaCriada.CriadaPorUsuario);
            }

            return CreatedAtAction(nameof(ObterPorId), new { id = notaCriada.IdNota }, notaCriada);
        }
        catch (Exception e)
        {
            var message = GetDetailedErrorMessage(e);
            return BadRequest(new { message = $"Erro ao criar a nota : {message}" });
        }
    }

    /// <summary>
    /// Retorna todas as notas , com paginacao opcional.
    /// </summary>
    /// <param name="pagina">Número da página (1-based) para paginação.</param>
    /// <param name="tamanho">Quantidade de itens por página.</param>
    /// <returns>Uma lista de registros cadastrados ou um objeto paginado contendo total e resultados.</returns>
    /// <response code="200">Sucesso ao listar registros.</response>
    /// <response code="400">Parâmetros de paginação inválidos (menores ou iguais a zero).</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListarNotas([FromQuery] int? pagina, [FromQuery] int? tamanho)
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
                var notasPaginadas = await _notaService.GetNotasPaginadasAsync(p, t);
                return Ok(notasPaginadas);
            }

            var todasNotas = await _notaService.GetAllNotasAsync();
            return Ok(todasNotas);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar notas {e.Message}" });
        }
    }

    /// <summary>
    /// Obtém um registro pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador exclusivo do registro (deve ser maior que zero).</param>
    /// <returns>O objeto correspondente cadastrado.</returns>
    /// <response code="200">Sucesso ao obter o registro.</response>
    /// <response code="400">ID fornecido inválido.</response>
    /// <response code="404">Registro não encontrado no banco de dados.</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId([FromRoute] long id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Id invalido. Envie id maior que zero." });
            }

            var item = await _notaService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound(new { message = "Nota nao encontrada." });
            }

            return Ok(item);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar nota: {e.Message}." });
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
            var notas = await _notaService.CountAsync();
            return Ok(new { count = notas });
        }
        catch (Exception e)
        {
            return NotFound(new { message = $"Erro ao procurar notas {e.Message}." });
        }
    }

    /// <summary>
    /// Retorna as notas para um tipo de ativo
    /// </summary>
    [HttpGet("ativo/{ativo}")]
    public async Task<IActionResult> ListarFichasPorAtivo([FromRoute] ComponentType ativo, [FromQuery] string? type)
    {
        try
        {
            if (!Enum.IsDefined(typeof(ComponentType), ativo))
            {
                return BadRequest(new { message = "Parametro ativo invalido." });
            }

            var resultado = await _notaService.GetFichasPorAtivoAsync(ativo, type);
            return Ok(resultado);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar fichas do ativo {e.Message}." });
        }
    }

    /// <summary>
    /// Atualiza uma nota pelo DTO.
    /// </summary>
    /// <param name="id">Identificador exclusivo na rota (deve ser maior que zero).</param>
    /// <param name="dto">DTO contendo os novos dados para atualização.</param>
    /// <returns>Mensagem de confirmação da atualização.</returns>
    /// <response code="200">Sucesso ao atualizar o registro.</response>
    /// <response code="400">Erros na validação do modelo ou ID divergente entre a rota e o corpo.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AtualizarNota([FromRoute] long id, [FromBody] NotaDto dto,CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "Id da nota invalido na rota. Envie id maior que zero." });
            }

            var notaModel = _mapper.Map<NotaModel>(dto);

            var result = await _notaService.AtualizarNotaCompletaAsync(notaModel, id);

            if (result.ConfiabilidadeRestaurada != ConfiabilidadeRestaurada.NaoRestaura)
            {
                var listaNotaId = new List<long> { result.IdNota };
                await _dadosRenovacaoService.ImportarDadosNotaAsync(listaNotaId, null, cancellationToken);
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao atualizar nota: {e.Message}." });
        }
    }

    /// <summary>
    /// Delete uma nota de mesmo Id
    /// </summary>
    /// <param name="id">Identificador exclusivo do registro a ser excluído.</param>
    /// <returns>Mensagem de confirmação da exclusão.</returns>
    /// <returns>Mensagem de confirmação da exclusão.</returns>
    /// <response code="200">Sucesso ao deletar o registro.</response>
    /// <response code="404">Registro não encontrado no banco de dados.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletarNota(long id)
    {
        try
        {
            var deleted = await _notaService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = "Nota não encontrada." });

            return Ok(new { message = "Nota deletada com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao deletar nota ." });
        }
    }

    /// <summary>
    /// Retorna os itens de uma nota 
    /// </summary>
    [HttpGet("{notaId:long}/items")]
    public async Task<IActionResult> BuscarItensNota(long notaId)
    {
        try
        {
            List<ItemNotaModel> lista = await _itemNotaService.GetItemsByNotaIdAsync(notaId);
            return Ok(lista);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar itens da nota: {e.Message}" });
        }
    }

    /// <summary>
    /// Classifica uma nota com base na classificação humana atribuida a uma nota específica
    /// </summary>
    /// <param name="id">Identificador exclusivo do registro a ser classificado.</param>
    /// <param name="classificacao">Valor da classificação humana.</param>
    /// <param name="idUsuario">Identificador do usuário que realizou a classificação.</param>
    /// <returns>O objeto classificado atualizado.</returns>
    /// <response code="200">Sucesso ao classificar.</response>
    /// <response code="400">Erros na validação do usuário ou ID informado inválido.</response>
    /// <response code="404">Registro não encontrado.</response>
    [HttpPatch("classificacao/{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClassificacaoHumanaNota(
        [FromRoute] long id,
        [FromQuery] Classificacao classificacao,
        [FromQuery] string idUsuario)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(idUsuario))
            {
                return BadRequest(new { message = "Parametros classificacao e idUsuario sao obrigatorios." });
            }

            var result = await _classificationService.HumanClassification(id, classificacao, idUsuario);
            if (result == null)
            {
                return NotFound(new { message = "Nota não encontrada para classificação." });
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao classificar esta nota: {e.Message}" });
        }
    }


    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }

    private static string GetDetailedErrorMessage(Exception ex)
    {
        if (ex.Message == "A chave estrangeira passada não existe" ||
            ex.Message == "Este registro já existe no sistema")
        {
            return ex.Message;
        }

        if (ex is DbUpdateException dbEx && dbEx.InnerException is PostgresException pgEx)
        {
            if (pgEx.SqlState == "23503")
            {
                return "A chave estrangeira passada não existe";
            }

            if (pgEx.SqlState == "23505")
            {
                return "Este registro já existe no sistema";
            }

            return $"Erro de integridade no banco de dados (Detalhe: {pgEx.MessageText})";
        }

        return ex.InnerException?.Message ?? ex.Message;
    }
}