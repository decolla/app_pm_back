using app_pm_back.DTO;
using app_pm_back.Models;
using app_pm_back.Models.CongasModel;
using app_pm_back.Services;
using app_pm_back.Services.CongasServices;
using app_pm_back.Services.PetroServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

/// <summary>
/// Controller para ordens
/// </summary>
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderPetroService _petroService;
    private readonly OrderCongasService _congasService;
    private readonly ClassificationService<OrdemPetroModel> _classificationServicePetro;
    private readonly ClassificationService<OrderCongasModel> _classificationServiceCongas;
    private readonly IMapper _mapper;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="petroService"></param>
    /// <param name="congasService"></param>
    /// <param name="mapper"></param>
    public OrdersController(OrderPetroService petroService, OrderCongasService congasService, IMapper mapper)
    {
        _petroService = petroService;
        _congasService = congasService;     
        _mapper = mapper;
    }

   // =========================================================================
    // ENDPOINTS DA PETRO (Rotas: api/orders/petro/...)
    // =========================================================================

    /// <summary>
    /// Cadastra uma ordem Petro pelo Dto
    /// </summary>
    [HttpPost("petro")]
    public async Task<IActionResult> CriarOrdemPetro([FromBody] CreateOrdemPetroDto dto)
    {
        try
        {
            var novaNota = _mapper.Map<OrdemPetroModel>(dto);
            var notaCriada = await _petroService.CreateNotaAsync(novaNota);
            return Created($"api/orders/petro/{notaCriada.IdNota}", notaCriada);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar a ordem Petro: {e.Message}." });
        }
    }

    /// <summary>
    /// Retorna uma lista todas as ordens Petro com sistema de páginas
    /// </summary>
    [HttpGet("petro")]
    public async Task<IActionResult> ListarNotasPetro([FromQuery] int? pagina, [FromQuery] int? tamanho)
    {
        try
        {
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
        catch (Exception )
        {
            return BadRequest(new { message = "Erro ao processar ordens Petro." });
        }
    }

    /// <summary>
    /// Conta o número total de ordens Petro salvas no banco
    /// </summary>
    [HttpGet("petro/count")]
    public async Task<IActionResult> ContarNotasPetro()
    {
        try
        {
            var notas = await _petroService.CountNotasAsync();
            return Ok(new { count = notas });
        }
        catch (Exception )
        {
            return NotFound(new { message = "Erro ao procurar notas Petro." });
        }
    }

    /// <summary>
    /// Retorna as Fichas de uma nota Petro e suas ocorrências de mesmo tipo
    /// </summary>
    [HttpGet("petro/ativo/{dnsIoT}")]
    public async Task<IActionResult> ListarOrdensPorAtivoPetro(string dnsIoT, [FromQuery] string? type)
    {
        try
        {
            // FALTA IMPLEMENTAÇÃO. FAZER: após correção das classes 
            var resultado = await _petroService.GetFichasPorAtivoAsync(dnsIoT, type);
            return Ok(resultado);
        }
        catch (Exception )
        {
            return BadRequest(new { message = "Erro ao buscar fichas do ativo Petro." });
        }
    }

    /// <summary>
    /// Atualiza uma nota Petro de mesmo Id
    /// </summary>
    [HttpPut("petro")]
    public async Task<IActionResult> AtualizarOrdemPetro([FromBody] OrdemPetroModel ordem)
    {
        try
        {
            var result = await _petroService.UpdateNotaAsync(ordem);
            return Ok(new { message = "Nota Petro atualizada com sucesso: " + result.IdNota });
        }
        catch (Exception )
        {
            return BadRequest(new { message = "Erro ao atualizar nota Petro." });
        }
    }

    /// <summary>
    /// Delete uma nota Petro de mesmo Id
    /// </summary>
    [HttpDelete("petro/{id:int}")]
    public async Task<IActionResult> DeletarOrdemPetro(int id)
    {
        try
        {
           await _petroService.DeleteNotaAsync(id);
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
    [HttpPut("petro/classificacao/{id:long}")]
    public async Task<IActionResult> ClassificacaoHumanaOrdemPetro(long id, string classificacao, string idUsuario)
    {
        try
        {
            var result = await _classificationServicePetro.HumanClassification(id, classificacao, idUsuario);
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
    [HttpPost("petro/classificacao/regra")]
    public async Task<IActionResult> CriarRegraPetro(RuleClassificationModel regra)
    {
        try
        {
            var result = await _classificationServicePetro.CreateRule(regra);
            return Ok(new {message = $"Regra criada com sucesso: {result}"});
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
    [HttpPost("petro/classificacao/filtro")]
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
    [HttpPost("petro/classificacao/catboost")]
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


    // =========================================================================
    // ENDPOINTS DA CONGAS (Rotas: api/orders/congas/...)
    // =========================================================================

    /// <summary>
    /// Cadastra uma nota Congas
    /// </summary>
    [HttpPost("congas")]
    public async Task<IActionResult> CriarOrdemCongas([FromBody] OrderCongasModel ordem)
    {
        try
        {
            var notaCriada = await _congasService.CreateNotaAsync(ordem);
            return Created($"api/orders/congas/{notaCriada.NumeroNotaOrigem}", notaCriada);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar a ordem Congas: {e.Message}." });
        }
    }

    /// <summary>
    /// Retorna uma lista todas as notas Congas com sistema de páginas
    /// </summary>
    [HttpGet("congas")]
    public async Task<IActionResult> ListarOrdensCongas([FromQuery] int? pagina, [FromQuery] int? tamanho)
    {
        try
        {
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
            return BadRequest(new { message = "Erro ao processar ordens Congas." });
        }
    }

    /// <summary>
    /// Conta o número total de notas Congas salvas no banco
    /// </summary>
    [HttpGet("congas/count")]
    public async Task<IActionResult> ContarNotasCongas()
    {
        try
        {
            var notas = await _congasService.CountNotasAsync();
            return Ok(new { count = notas });
        }
        catch (Exception)
        {
            return NotFound(new { message = "Erro ao procurar ordens Congas." });
        }
    }

    /// <summary>
    /// Retorna as Fichas de uma nota Congas e suas ocorrências de mesmo tipo
    /// </summary>
    [HttpGet("congas/ativo/{dnsIoT}")]
    public async Task<IActionResult> ListarFichasPorAtivoCongas(string dnsIoT, [FromQuery] string? type)
    {
        try
        {
            var resultado = await _congasService.GetFichasPorAtivoAsync(dnsIoT, type);
            return Ok(resultado);
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao buscar ordens do ativo Congas." });
        }
    }

    /// <summary>
    /// Atualiza uma nota Congas de mesmo Id
    /// </summary>
    [HttpPut("congas")]
    public async Task<IActionResult> AtualizarNotaCongas([FromBody] OrderCongasModel ordem)
    {
        try
        {
            var result = await _congasService.UpdateNotaAsync(ordem);
            return Ok(new { message = "Ordem Congas atualizada com sucesso: " + result.NumeroNotaOrigem });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao atualizar ordem Congas." });
        }
    }

    /// <summary>
    /// Delete uma nota Congas de mesmo Id
    /// </summary>
    [HttpDelete("congas/{id:int}")]
    public async Task<IActionResult> DeletarNotaCongas(int id)
    {
        try
        {
            await _congasService.DeleteNotaAsync(id);
            return Ok(new { message = "Ordem Congas deletada com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Erro ao deletar ordem Congas." });
        }
    }
    
    /// <summary>
    /// Classifica uma ordem Congas com base na classificação humana atribuida a uma nota específica
    /// </summary>
    /// <param name="id">Id da ordem a ser classificada</param>
    /// <param name="classificacao"> Classificação a ser atribuída</param>
    /// <param name="idUsuario">Id do usuário que realizou a classificação</param>
    /// <returns>Nota atualizada</returns>
    [HttpPut("congas/classificacao/{id:long}")]
    public async Task<IActionResult> ClassificacaoHumanaNotaCongas(long id, string classificacao, string idUsuario)
    {
        try
        {
            var result = await _classificationServiceCongas.HumanClassification(id, classificacao, idUsuario);
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
    [HttpPost("congas/classificacao/regra")]
    public async Task<IActionResult> CriarRegraCongas(RuleClassificationModel regra)
    {
        try
        {
            var result = await _classificationServiceCongas.CreateRule(regra);
            return Ok(new {message = $"Regra criada com sucesso: {result}"});
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
    [HttpPost("congas/classificacao/filtro")]
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
    [HttpPost("congas/classificacao/catboost")]
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
}
