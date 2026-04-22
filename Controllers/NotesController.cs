using app_pm_back.DTO.ComgasDTO;
using app_pm_back.Models;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using app_pm_back.Services.CongasServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

// =========================================================================
// ENDPOINTS DA PETRO (Rotas: api/notes/petro/...)
// =========================================================================

/// <summary>
/// Endpoints de notas da Petrobras.
/// </summary>
[ApiController]
[Route("api/V1/Petrobras/notes")]
public class PetroNotesController : ControllerBase
{
    private readonly NotesPetroService _petroService;
    private readonly ClassificationService<NotaPetroModel> _classificationServicePetro;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="petroService"></param>
    /// <param name="classificationServicePetro"></param>
    public PetroNotesController(
        NotesPetroService petroService,
        ClassificationService<NotaPetroModel> classificationServicePetro)
    {
        _petroService = petroService;
        _classificationServicePetro = classificationServicePetro;
    }

    /// <summary>
    /// Cadastra uma nota Petro.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CriarNotaPetro([FromBody] NotaPetroModel dto)
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
            return BadRequest(new { message = $"Erro ao criar a nota Petro: {e.Message}." });
        }
    }

    /// <summary>
    /// Retorna uma lista todas as notas Petro com sistema de páginas
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
            return BadRequest(new { message = "Erro ao processar notas Petro." });
        }
    }

    /// <summary>
    /// Conta o número total de notas Petro salvas no banco
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
    public async Task<IActionResult> ListarFichasPorAtivoPetro([FromRoute] string dnsIoT, [FromQuery] string? type)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dnsIoT))
            {
                return BadRequest(new { message = "Parametro dnsIoT invalido." });
            }

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
    /// <param name="id">Id da nota na rota</param>
    /// <param name="nota">Dados da nota a ser atualizada</param>
    // [HttpPut]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> AtualizarNotaPetro([FromRoute] int id, [FromBody] NotaPetroModel nota)
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

            if (nota.IdNota <= 0)
            {
                nota.IdNota = id;
            }

            if (nota.IdNota != id)
            {
                return BadRequest(new { message = "Id da rota e IdNota do corpo da requisição devem ser iguais." });
            }

            var result = await _petroService.UpdateNotaAsync(nota);
            return Ok(new { message = "Nota Petro atualizada com sucesso: " + result.IdNota });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao atualizar nota Petro." });
        }
    }

    /// <summary>
    /// Deleta uma nota Petro de mesmo Id
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletarNotaPetro(int id)
    {
        try
        {
            var deleted = await _petroService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = "Nota Petro não encontrada." });

            return Ok(new { message = "Nota Petro deletada com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao deletar nota Petro." });
        }
    }

    /// <summary>
    /// Classifica uma nota Petro com base na classificação humana atribuida a uma nota específica
    /// </summary>
    /// <param name="id">Id da nota a ser classificada</param>
    /// <param name="classificacao"> Classificação a ser atribuída</param>
    /// <param name="idUsuario">Id do usuário que realizou a classificação</param>
    /// <returns>Nota atualizada</returns>
    // [HttpPut("{id:long}")]
    [HttpPut("classificacao/{id:long}")]
    public async Task<IActionResult> ClassificacaoHumanaNotaPetro(
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
                return NotFound(new { message = "Nota não encontrada para classificação." });
            }

            if (result.MetodoClassificacao == "MANUAL")
            {
                return Ok(result);
            }

            return BadRequest(new { message = "Não foi possível classificar esta nota" });
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
    /// <returns>Lista de notas atualizadas</returns>
    [HttpPost("classificacao/filtro")]
    public async Task<IActionResult> ClassificacaoPalavrasNotaPetro()
    {
        try
        {
            // passar uma lista vazia por enquanto 
            var result = await _classificationServicePetro.WordsClassification([]);
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
    /// Classifica as notas usando a IA catboost,
    /// onde ela analisa o texto das notas e classifica-as com base no treinamento
    /// para notas/ordems corretivas e preventivas 
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
            return BadRequest(new { message = $"Erro ao classificar notas: {e.Message}" });
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
// ENDPOINTS DA CONGAS (Rotas: api/notes/congas/...)
// =========================================================================

/// <summary>
/// Endpoints de notas da Comgas.
/// </summary>
[ApiController]
[Route("api/V1/Comgas/notes")]
public class ComgasNotesController : ControllerBase
{
    private readonly NotaManutencaoService _congasService;
    private readonly ItemNoteCongasService _itemNoteCongasService;
    private readonly ClassificationService<NotaManutencaoModel> _classificationServiceCongas;
    private readonly IMapper _mapper;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="congasService"></param>
    /// <param name="itemNoteCongasService"></param>
    /// <param name="classificationServiceCongas"></param>
    /// <param name="mapper"></param>
    public ComgasNotesController(
        NotaManutencaoService congasService,
        ItemNoteCongasService itemNoteCongasService,
        ClassificationService<NotaManutencaoModel> classificationServiceCongas,
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

            var novaNota = _mapper.Map<NotaManutencaoModel>(dto);
            if (novaNota.IdNota <= 0)
            {
                return BadRequest(new { message = "Id da nota invalido. Envie IdNota ou NumeroNota maior que zero." });
            }

            var notaCriada = await _congasService.CreateAsync(novaNota);
            return Created($"api/V1/Comgas/notes/{notaCriada.IdNota}", notaCriada);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar a nota Comgas: {e.Message}." });
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
    public async Task<IActionResult> ListarFichasPorAtivoCongas([FromRoute] string dnsIoT, [FromQuery] string? type)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dnsIoT))
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
    /// <param name="id">Id da nota na rota</param>
    /// <param name="dto">Dados da nota a ser atualizada</param>
    // [HttpPut]
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

            var nota = _mapper.Map<NotaManutencaoModel>(dto);
            if (nota.IdNota <= 0)
            {
                return BadRequest(new { message = "Id da nota invalido. Envie IdNota ou NumeroNota maior que zero." });
            }

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
    /// <param name="notaId">Id da nota a ser realizado a busca</param>
    [HttpGet("{notaId:long}/items")]
    public async Task<IActionResult> BuscarItensNota(long notaId)
    {
        try
        {
            List<ItemNotaManutencaoModel> lista = await _itemNoteCongasService.GetItemsByNotaIdAsync(notaId);
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
    /// <param name="id">Id da nota a ser classificada</param>
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
                return NotFound(new { message = "Nota não encontrada para classificação." });
            }

            if (result.MetodoClassificacao == "MANUAL")
            {
                return Ok(result);
            }

            return BadRequest(new { message = "Não foi possível classificar esta nota" });
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
    /// Classifica uma nota com base uma lista de palavras armazenadas no banco
    /// </summary>
    /// <returns>Lista de notas atualizadas</returns>
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

            return BadRequest("Não foram achadas notas para classificar");
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao classificar notas: {e.Message}" });
        }
    }

    /// <summary>
    /// Classifica as notas usando a IA catboost,
    /// onde ela analisa o texto das notas e classifica-as com base no treinamento
    /// para notas/ordems corretivas e preventivas 
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
            return BadRequest(new { message = $"Erro ao classificar notas: {e.Message}" });
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