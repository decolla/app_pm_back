using app_pm_back.DTO.ComgasDTO;
using app_pm_back.Models;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using app_pm_back.Services.CongasServices;
using app_pm_back.Services.PetroServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

// =========================================================================
// ENDPOINTS DA PETRO (Rotas: api/orders/...)
// =========================================================================

/// <summary>
/// Endpoints de ordens da Petrobras.
/// </summary>
[ApiController]
[Route("api/V1/Petrobras/orders")]
public class PetroOrdersController : ControllerBase
{
    private readonly OrderPetroService _petroService;
    private readonly ClassificationService<OrdemPetroModel> _classificationServicePetro;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="petroService"></param>
    /// <param name="classificationServicePetro"></param>
    public PetroOrdersController(
        OrderPetroService petroService,
        ClassificationService<OrdemPetroModel> classificationServicePetro
    )
    {
        _petroService = petroService;
        _classificationServicePetro = classificationServicePetro;
    }


    /// <summary>
    /// Cadastra uma ordem Petro.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CriarOrdemPetro([FromBody] OrdemPetroModel dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var notaCriada = await _petroService.CreateAsync(dto);
            return new ObjectResult(notaCriada);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar a ordem Petro: {e.Message}." });
        }
    }

    /// <summary>
    /// Retorna uma lista todas as ordens Petro com sistema de páginas
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListarNotasPetro([FromQuery] int? pagina, [FromQuery] int? tamanho)
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
                var notasPaginadas = await _petroService.GetNotasPaginadasAsync(p, t);
                return Ok(notasPaginadas);
            }

            var todasNotas = await _petroService.GetAllNotasAsync();
            return Ok(todasNotas);
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao processar ordens Petro." });
        }
    }

    /// <summary>
    /// Conta o número total de ordens Petro salvas no banco
    /// </summary>
    [HttpGet("count")]
    public async Task<IActionResult> ContarNotasPetro()
    {
        try
        {
            var notas = await _petroService.CountNotasAsync();
            return Ok(new { count = notas });
        }
        catch (Exception)
        {
            return NotFound(new { message = "Erro ao procurar notas Petro." });
        }
    }

    /// <summary>
    /// Retorna as Fichas de uma nota Petro e suas ocorrências de mesmo tipo
    /// </summary>
    [HttpGet("ativo/{dnsIoT}")]
    public async Task<IActionResult> ListarOrdensPorAtivoPetro([FromRoute] string dnsIoT, [FromQuery] string? type)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dnsIoT))
            {
                return BadRequest(new { message = "Parametro dnsIoT invalido." });
            }

            // FALTA IMPLEMENTAÇÃO. FAZER: após correção das classes 
            var resultado = await _petroService.GetFichasPorAtivoAsync(dnsIoT, type);
            return Ok(resultado);
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao buscar fichas do ativo Petro." });
        }
    }

    /// <summary>
    /// Atualiza uma nota Petro de mesmo Id
    /// </summary>
    /// <param name="id">Id da ordem na rota</param>
    /// <param name="ordem">Dados da ordem a ser atualizada</param>
    // [HttpPut]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> AtualizarOrdemPetro([FromRoute] int id, [FromBody] OrdemPetroModel ordem)
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

            if (ordem.IdNota <= 0)
            {
                ordem.IdNota = id;
            }

            if (ordem.IdNota != id)
            {
                return BadRequest(new { message = "Id da rota e IdNota do corpo da requisição devem ser iguais." });
            }

            var result = await _petroService.UpdateNotaAsync(ordem);
            return Ok(new { message = "Nota Petro atualizada com sucesso: " + result.IdNota });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao atualizar nota Petro." });
        }
    }

    /// <summary>
    /// Delete uma nota Petro de mesmo Id
    /// </summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeletarOrdemPetro(long id)
    {
        try
        {
            var deleted = await _petroService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = "Ordem Petro não encontrada." });

            return Ok(new { message = "Nota Petro deletada com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao deletar nota Petro." });
        }
    }

    /// <summary>
    /// Classifica uma ordem Petro com base na classificação humana atribuida a uma nota específica
    /// </summary>
    /// <param name="id">Id da ordem a ser classificada</param>
    /// <param name="classificacao"> Classificação a ser atribuída</param>
    /// <param name="idUsuario">Id do usuário que realizou a classificação</param>
    /// <returns>Nota atualizada</returns>
    [HttpPut("classificacao/{id:long}")]
    public async Task<IActionResult> ClassificacaoHumanaOrdemPetro(
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

            var result = await _classificationServicePetro.HumanClassification(id, classificacao, idUsuario);
            if (result == null)
            {
                return NotFound(new { message = "Ordem não encontrada para classificação." });
            }

            if (result.MetodoClassificacao == "MANUAL")
            {
                return Ok(result);
            }

            return BadRequest(new { message = "Não foi possível classificar esta ordem" });
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
    public async Task<IActionResult> CriarRegraPetro([FromBody] RuleClassificationModel regra)
    {
        try
        {
            var result = await _classificationServicePetro.CreateRule(regra);
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
    /// <returns>Lista de ordem atualizadas</returns>
    [HttpPost("classificacao/filtro")]
    public async Task<IActionResult> ClassificacaoPalavrasOrdemPetro()
    {
        try
        {
            // passar uma lista vazia por enquanto 
            var result = await _classificationServicePetro.WordsClassification([]);
            if (result.Count > 0)
            {
                return Ok(result);
            }

            return BadRequest("Não foram achadas ordem para classificar");
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao classificar ordem: {e.Message}" });
        }
    }

    /// <summary>
    /// Classifica as ordens usando a IA catboost,
    /// onde ela analisa o texto das ordem e classifica-as com base no treinamento
    /// para notas/ordens corretivas e preventivas 
    /// </summary>
    /// <returns></returns>
    [HttpPost("classificacao/catboost")]
    public async Task<IActionResult> ClassificacaoCatboostPetro()
    {
        try
        {
            var result = await _classificationServicePetro.CatboostClassification();
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao classificar ordens: {e.Message}" });
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

// =========================================================================
// ENDPOINTS DA CONGAS (Rotas: api/orders/...)
// =========================================================================

/// <summary>
/// Endpoints de ordens da Comgas.
/// </summary>
[ApiController]
[Route("api/V1/Comgas/orders")]
public class ComgasOrdersController : ControllerBase
{
    private readonly OrderCongasService _congasService;
    private readonly ClassificationService<OrdemManutencaoModel> _classificationServiceCongas;
    private readonly IMapper _mapper;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="congasService"></param>
    /// <param name="classificationServiceCongas"></param>
    /// <param name="mapper"></param>
    public ComgasOrdersController(
        OrderCongasService congasService,
        ClassificationService<OrdemManutencaoModel> classificationServiceCongas,
        IMapper mapper
    )
    {
        _congasService = congasService;
        _classificationServiceCongas = classificationServiceCongas;
        _mapper = mapper;
    }

    /// <summary>
    /// Cadastra uma ordem Comgas a partir de DTO.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CriarOrdemCongas([FromBody] OrdemManutencaoComgasDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var ordem = _mapper.Map<OrdemManutencaoModel>(dto);
            if (ordem.IdOrdem <= 0)
            {
                return BadRequest(new { message = "Id da ordem invalido. Envie IdOrdem maior que zero." });
            }

            var notaCriada = await _congasService.CreateAsync(ordem);
            return Created($"api/V1/Comgas/orders/{notaCriada.IdOrdem}", notaCriada);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar a ordem Comgas: {e.Message}." });
        }
    }

    /// <summary>
    /// Retorna todas as ordens Comgas, com paginacao opcional.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListarOrdensCongas([FromQuery] int? pagina, [FromQuery] int? tamanho)
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
            return BadRequest(new { message = "Erro ao processar ordens Comgas." });
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
            return NotFound(new { message = "Erro ao procurar ordens Comgas." });
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
            return BadRequest(new { message = "Erro ao buscar ordens do ativo Comgas." });
        }
    }

    /// <summary>
    /// Atualiza uma ordem Comgas pelo DTO.
    /// </summary>
    /// <param name="id">Id da ordem na rota</param>
    /// <param name="dto">Dados da ordem a ser atualizada</param>
    // [HttpPut]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> AtualizarNotaCongas([FromRoute] long id, [FromBody] OrdemManutencaoComgasDto dto)
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

            if (dto.IdOrdem <= 0)
            {
                dto.IdOrdem = id;
            }

            if (dto.IdOrdem != id)
            {
                return BadRequest(new { message = "Id da rota e IdOrdem do corpo da requisição devem ser iguais." });
            }

            var ordem = _mapper.Map<OrdemManutencaoModel>(dto);
            if (ordem.IdOrdem <= 0)
            {
                return BadRequest(new { message = "Id da ordem invalido. Envie IdOrdem maior que zero." });
            }

            var result = await _congasService.UpdateNotaAsync(ordem);
            return Ok(new { message = "Ordem Comgas atualizada com sucesso: " + result.IdOrdem });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao atualizar ordem Comgas." });
        }
    }

    /// <summary>
    /// Delete uma nota Congas de mesmo Id
    /// </summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeletarNotaCongas(long id)
    {
        try
        {
            var deleted = await _congasService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = "Ordem Comgas não encontrada." });

            return Ok(new { message = "Ordem Comgas deletada com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao deletar ordem Comgas." });
        }
    }

    /// <summary>
    /// Classifica uma ordem Congas com base na classificação humana atribuida a uma nota específica
    /// </summary>
    /// <param name="id">Id da ordem a ser classificada</param>
    /// <param name="classificacao"> Classificação a ser atribuída</param>
    /// <param name="idUsuario">Id do usuário que realizou a classificação</param>
    /// <returns>Nota atualizada</returns>
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
                return NotFound(new { message = "Ordem não encontrada para classificação." });
            }

            if (result.MetodoClassificacao == "MANUAL")
            {
                return Ok(result);
            }

            return BadRequest(new { message = "Não foi possível classificar esta ordem" });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao classificar esta ordem: {e.Message}" });
        }
    }

    /// <summary>
    /// Cria uma regra de classificação por palavras
    /// </summary>
    /// <param name="regra">Regra a ser aplicada</param>
    /// <returns>Regra criada</returns>
    [HttpPost("classificacao/regra")]
    public async Task<IActionResult> CriarRegraCongas([FromBody] RuleClassificationModel regra)
    {
        try
        {
            var result = await _classificationServiceCongas.CreateRule(regra);
            return Ok(new { message = $"Regra criada com sucesso com Id: {result.IdRegra}" });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar regra {e.Message}" });
        }
    }

    /// <summary>
    /// Classifica uma ordem com base uma lista de palavras armazenadas no banco
    /// </summary>
    /// <returns>Lista de ordem atualizadas</returns>
    [HttpPost("classificacao/filtro")]
    public async Task<IActionResult> ClassificacaoPalavrasNotaCongas()
    {
        try
        {
            // passar uma lista vazia por enquanto 
            var result = await _classificationServiceCongas.WordsClassification([]);
            if (result.Count > 0)
            {
                return Ok(result);
            }

            return BadRequest("Não foram achadas ordem para classificar");
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao classificar ordem: {e.Message}" });
        }
    }

    /// <summary>
    /// Classifica as ordens usando a IA catboost,
    /// onde ela analisa o texto das ordem e classifica-as com base no treinamento
    /// para ordem/ordems corretivas e preventivas 
    /// </summary>
    /// <returns></returns>
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
            return BadRequest(new { message = $"Erro ao classificar ordens: {e.Message}" });
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