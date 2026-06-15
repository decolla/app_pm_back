using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
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
    private readonly NotaManutencaoService _notaService;
    private readonly ItemNotaService _itemNotaService;
    private readonly ClassificationService<NotaModel> _classificationService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="NotasController"/>.
    /// </summary>
    public NotasController(
        NotaManutencaoService notaService,
        ItemNotaService itemNotaService,
        ClassificationService<NotaModel> classificationService,
        IMapper mapper)
    {
        _notaService = notaService;
        _itemNotaService = itemNotaService;
        _classificationService = classificationService;
        _mapper = mapper;
    }

    /// <summary>
    /// Cadastra uma nota a partir de DTO.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CriarNota([FromBody] NotaManutencaoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var novaNota = _mapper.Map<NotaModel>(dto);
            if (novaNota.IdNota <= 0)
            {
                return BadRequest(new { message = "Id da nota invalido. Envie IdNota ou NumeroNota maior que zero." });
            }

            // Normalização de chaves estrangeiras
            if (novaNota.IdPlano <= 0) novaNota.IdPlano = null;
            if (novaNota.IdUsuario <= 0) novaNota.IdUsuario = null;

            var notaCriada = await _notaService.CreateAsync(novaNota);
            return Created($"{app_pm_back.Config.RouteConfig.BaseRoute}/notes/{notaCriada.IdNota}", notaCriada);
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
    [HttpGet]
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
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao processar notas ." });
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
            var notas = await _notaService.CountNotasAsync();
            return Ok(new { count = notas });
        }
        catch (Exception)
        {
            return NotFound(new { message = "Erro ao procurar notas ." });
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

            var resultado = await _notaService.GetFichasPorAtivoAsync(ativo, type);
            return Ok(resultado);
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao buscar fichas do ativo ." });
        }
    }

    /// <summary>
    /// Atualiza uma nota pelo DTO.
    /// </summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> AtualizarNota([FromRoute] long id, [FromBody] NotaManutencaoDto dto)
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

            if (dto.IdNota <= 0)
            {
                dto.IdNota = id;
            }

            if (dto.IdNota != id)
            {
                return BadRequest(new { message = "Id da rota e IdNota do corpo da requisição devem ser iguais." });
            }

            var nota = _mapper.Map<NotaModel>(dto);
            nota.DataModificacao = DateTime.UtcNow;
            // adicionar dps quem modificou a nota aqui
            var result = await _notaService.UpdateNotaAsync(nota);
            return Ok(new { message = "Nota atualizada com sucesso: " + result.IdNota });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao atualizar nota ." });
        }
    }

    /// <summary>
    /// Delete uma nota de mesmo Id
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletarNota(int id)
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
    [HttpPut("classificacao/{id:long}")]
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

    /// <summary>
    /// Cria uma regra de classificação por palavras
    /// </summary>
    /// <param name="regra">Regra a ser aplicada</param>
    /// <returns>Regra criada</returns>
    [HttpPost("classificacao/regra")]
    public async Task<IActionResult> CriarRegraCongas([FromBody] RuleClassificationDto regra)
    {
        try
        {
            var result = await _classificationService.CreateRule(regra);
            return Ok(new { message = $"Regra criada com sucesso com Id: {result.IdRegra}" });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar regra {e.Message}" });
        }
    }

    /// <summary>
    /// Classifica uma nota com base uma lista de palavras armazenadas no banco
    /// </summary>
    /// <returns>Lista de notas atualizadas</returns>
    [HttpPost("classificacao/filtro")]
    public async Task<IActionResult> ClassificacaoPalavrasNotaCongas()
    {
        try
        {
            // passar uma lista vazia por enquanto 
            var result = await _classificationService.WordsClassification([]);
            if (result.Count > 0)
            {
                return Ok(result);
            }

            return BadRequest("Não foram achadas notas para classificar");
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao classificar notas: {e.Message}" });
        }
    }

    /// <summary>
    /// Classifica as notas usando a IA catboost
    /// </summary>
    [HttpPost("classificacao/catboost")]
    public async Task<IActionResult> ClassificacaoCatboostNota()
    {
        try
        {
            var result = await _classificationService.CatboostClassification();
            return Ok("Notas classificadas com sucesso");
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao classificar notas: {e.Message}" });
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