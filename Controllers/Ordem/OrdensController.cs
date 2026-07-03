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
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/ordens")]
public class OrdensController : ControllerBase
{
    private readonly OrdemService _ordemService;
    private readonly ClassificationService<OrdemModel> _classificationService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="OrdensController"/>.
    /// </summary>
    public OrdensController(
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
    /// <param name="dto">DTO contendo os dados da requisição.</param>
    /// <returns>O objeto criado com status 201 (Created).</returns>
    /// <response code="201">Registro criado com sucesso.</response>
    /// <response code="400">Erros na validação do modelo ou dados fornecidos inválidos.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CriarOrdem([FromBody] OrdemDto dto)
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
            
            if(ordem.NumeroOrdem <= 0 || ordem.NumeroOrdem == null) ordem.NumeroOrdem = ordem.IdOrdem; // se não for informado, usa o IdOrdem como NumeroOrdem

            var notaCriada = await _ordemService.CreateAsync(ordem);
            return CreatedAtAction(nameof(ObterPorId), new { id = notaCriada.IdOrdem }, notaCriada);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar a ordem : {e.Message}." });
        }
    }

    /// <summary>
    /// Retorna todas as ordens, com paginacao opcional.
    /// </summary>
    /// <param name="pagina">Número da página (1-based) para paginação.</param>
    /// <param name="tamanho">Quantidade de itens por página.</param>
    /// <returns>Uma lista de registros cadastrados ou um objeto paginado contendo total e resultados.</returns>
    /// <response code="200">Sucesso ao listar registros.</response>
    /// <response code="400">Parâmetros de paginação inválidos (menores ou iguais a zero).</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    /// Obtém um registro pelo id da ordem.
    /// </summary>
    /// <param name="id">Identificador do registro.</param>
    /// <returns>Resultado da operação.</returns>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> ObterPorId([FromRoute] long id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Id invalido. Envie id maior que zero." });
            }

            var item = await _ordemService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound(new { message = "Ordem nao encontrada." });
            }

            return Ok(item);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar ordem: {e.Message}." });
        }
    }
        
    /// <summary>
    /// Obtém os grupos responsáveis pela ordem.
    /// </summary>
    /// <returns>Resultado da operação.</returns>
    [HttpGet("grupos/{id:long}")]
    public async Task<IActionResult> ObterGruposResonponsaveis([FromRoute] long id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Id invalido. Envie id maior que zero." });
            }
            
            var grupos = await _ordemService.GetGruposResponsaveis(id);
            if (grupos == null)
            {
                return NotFound(new { message = "Ordem nao encontrada." });
            }

            return Ok(grupos);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar ordem: {e.Message}." });
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
            var notas = await _ordemService.CountAsync();
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
    [HttpGet("ativo/{ativo}")]
    public async Task<IActionResult> ListarFichasPorAtivo([FromRoute] ComponentType ativo, [FromQuery] string? type)
    {
        try
        {
            if (!Enum.IsDefined(typeof(ComponentType), ativo))
            {
                return BadRequest(new { message = "Parametro ativo invalido." });
            }

            var resultado = await _ordemService.GetFichasPorAtivoAsync(ativo, type);
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
    /// <param name="id">Identificador exclusivo na rota (deve ser maior que zero).</param>
    /// <param name="dto">DTO contendo os novos dados para atualização.</param>
    /// <returns>Mensagem de confirmação da atualização.</returns>
    /// <response code="200">Sucesso ao atualizar o registro.</response>
    /// <response code="400">Erros na validação do modelo ou ID divergente entre a rota e o corpo.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AtualizarOrdem([FromRoute] long id, [FromBody] OrdemDto dto)
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
            var OrdemExistente  = await _ordemService.GetByIdAsync(id);
            if (OrdemExistente == null)
            {
                return NotFound(new { message = $"Nota com Id {id} não encontrada." });
            }
            _mapper.Map(dto, OrdemExistente);
            OrdemExistente.DataModificacao = DateTime.UtcNow;

            var result = await _ordemService.UpdateNotaAsync(OrdemExistente);
            return Ok(result);
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao atualizar ordem ." });
        }
    }
    
    /// <summary>
    /// Atualiza uma ordem pelo DTO completo da ordem, atualizando juntamente as operações.
    /// </summary>
    /// <param name="id">Identificador exclusivo na rota (deve ser maior que zero).</param>
    /// <param name="dto">DTO contendo os novos dados para atualização.</param>
    /// <returns>Mensagem de confirmação da atualização.</returns>
    /// <response code="200">Sucesso ao atualizar o registro.</response>
    /// <response code="400">Erros na validação do modelo ou ID divergente entre a rota e o corpo.</response>
    [HttpPut("complete/{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AtualizarNotaCompleta([FromRoute] long id, [FromBody] NotaDto dto)
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

            var ordemExistente = await _ordemService.GetByIdAsync(id);
            if (ordemExistente == null)
            {
                return NotFound(new { message = $"Ordem com Id {id} não encontrada." });
            }

            var dtoParaMapear = dto; 
    
            _mapper.Map(dtoParaMapear, ordemExistente);
            
            ordemExistente.DataModificacao = DateTime.UtcNow;

            var result = await _ordemService.AtualizarOrdemCompletaAsync(ordemExistente);
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
    /// <response code="200">Sucesso ao deletar o registro.</response>
    /// <response code="404">Registro não encontrado no banco de dados.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    public async Task<IActionResult> ClassificacaoHumanaOrdem(
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
                return NotFound(new { message = "Ordem não encontrada para classificação." });
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao classificar esta ordem: {e.Message}" });
        }
    }

    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }
}