using app_pm_back.DTO.ComgasDTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using app_pm_back.Services.CongasServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace app_pm_back.Controllers;

/// <summary>
/// Endpoints de notas da Comgas.
/// </summary>
[ApiController]
[Route("api/V1/Comgas/notas")]
public class NotasController : ControllerBase
{
    private readonly NotaManutencaoService _congasService;
    private readonly ItemNotaService _itemNoteCongasService;
    private readonly ClassificationService<NotaModel> _classificationServiceCongas;
    private readonly IMapper _mapper;

    public NotasController(
        NotaManutencaoService congasService,
        ItemNotaService itemNoteCongasService,
        ClassificationService<NotaModel> classificationServiceCongas,
        IMapper mapper)
    {
        _congasService = congasService;
        _itemNoteCongasService = itemNoteCongasService;
        _classificationServiceCongas = classificationServiceCongas;
        _mapper = mapper;
    }

    /// <summary>
    /// Cadastra uma nota Comgas a partir de DTO.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CriarNotaCongas([FromBody] NotaManutencaoComgasDto dto)
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

            var notaCriada = await _congasService.CreateAsync(novaNota);
            return Created($"api/V1/Comgas/notes/{notaCriada.IdNota}", notaCriada);
        }
        catch (Exception e)
        {
            var message = GetDetailedErrorMessage(e);
            return BadRequest(new { message = $"Erro ao criar a nota Comgas: {message}" });
        }
    }

    /// <summary>
    /// Retorna todas as notas Comgas, com paginacao opcional.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListarNotasCongas([FromQuery] int? pagina, [FromQuery] int? tamanho)
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
            return BadRequest(new { message = "Erro ao processar notas Comgas." });
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
            return NotFound(new { message = "Erro ao procurar notas Comgas." });
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
            return BadRequest(new { message = "Erro ao buscar fichas do ativo Comgas." });
        }
    }

    /// <summary>
    /// Atualiza uma nota Comgas pelo DTO.
    /// </summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> AtualizarNotaCongas([FromRoute] long id, [FromBody] NotaManutencaoComgasDto dto)
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
            var result = await _congasService.UpdateNotaAsync(nota);
            return Ok(new { message = "Nota Comgas atualizada com sucesso: " + result.IdNota });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao atualizar nota Comgas." });
        }
    }

    /// <summary>
    /// Delete uma nota Congas de mesmo Id
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletarNotaCongas(int id)
    {
        try
        {
            var deleted = await _congasService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = "Nota Comgas não encontrada." });

            return Ok(new { message = "Nota Comgas deletada com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao deletar nota Comgas." });
        }
    }

    /// <summary>
    /// Retorna os itens de uma nota Congas
    /// </summary>
    [HttpGet("{notaId:long}/items")]
    public async Task<IActionResult> BuscarItensNota(long notaId)
    {
        try
        {
            List<ItemNotaModel> lista = await _itemNoteCongasService.GetItemsByNotaIdAsync(notaId);
            return Ok(lista);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar itens da nota: {e.Message}" });
        }
    }

    /// <summary>
    /// Classifica uma nota Congas com base na classificação humana atribuida a uma nota específica
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
    /// Classifica uma nota com base uma lista de palavras armazenadas no banco
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
    public async Task<IActionResult> ClassificacaoCatboostCongas()
    {
        try
        {
            var result = await _classificationServiceCongas.CatboostClassification();
            return Ok(result);
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
        if (ex.Message == "A chave estrangeira passada não existe" || ex.Message == "Este registro já existe no sistema")
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