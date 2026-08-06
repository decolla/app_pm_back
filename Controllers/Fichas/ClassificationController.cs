using app_pm_back.DTO;
using app_pm_back.Models;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using app_pm_back.Services.FichasServices;
using app_pm_back.Services.GenericServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers.FichasControllers;

[ApiController]
[Route(Config.RouteConfig.BaseRoute + "/classificacao")]
public class ClassificationController : ControllerBase
{
    private readonly RegraClassificacaoService _regraClassificacaoService;
    private readonly ILogger<FichasController> _logger;
    private readonly IMapper _mapper;
    private readonly ClassificationService<NotaModel> _notaService;
    private readonly ClassificationService<OrdemModel> _ordemService;
    private readonly ConfiguracaoGlobalService _configuracaoGlobalService;
    private readonly GenericSearchService _searchService;

    public ClassificationController(
        RegraClassificacaoService regraClassificacaoService,
        IMapper mapper,
        ILogger<FichasController> logger,
        ClassificationService<NotaModel> notaService,
        ClassificationService<OrdemModel> ordemService,
        GenericSearchService searchService,
        ConfiguracaoGlobalService configuracaoGlobalService
    )
    {
        _regraClassificacaoService = regraClassificacaoService;
        _logger = logger;
        _mapper = mapper;
        _notaService = notaService;
        _ordemService = ordemService;
        _configuracaoGlobalService = configuracaoGlobalService;
        _searchService = searchService;
    }

    /// <summary>
    /// Cria uma regra de classificação por palavras para Ambas, Fichas ou Ordens
    /// </summary>
    /// <param name="regra">Regra a ser aplicada</param>
    /// <returns>Regra criada</returns>
    [HttpPost("regras")]
    public async Task<IActionResult> CriarRegraClassificacao([FromBody] RegraClassificacaoDto dto,
        [FromQuery] Fichas buscaTabela)
    {
        try
        {
            var regra = _mapper.Map<RegraClassificacaoModel>(dto);

            var criado = await _regraClassificacaoService.CreateAsync(regra);
            return CreatedAtAction(regra.IdRegra.ToString(), new { id = criado.IdRegra }, criado);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar regra de classificação: {e.Message}" });
        }
    }

    /// <summary>
    /// Lista os registros cadastrados com paginação.
    /// </summary>
    /// <param name="pagina">Número da página (1-based) para paginação.</param>
    /// <param name="tamanho">Quantidade de itens por página.</param>
    /// <returns>Uma lista de registros cadastrados ou um objeto paginado contendo total e resultados.</returns>
    /// <response code="200">Sucesso ao listar registros.</response>
    /// <response code="400">Parâmetros de paginação inválidos (menores ou iguais a zero).</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
                var paginado = await _regraClassificacaoService.GetPagedAsync(p, t);
                return Ok(paginado);
            }

            var todos = await _regraClassificacaoService.GetAllAsync();
            return Ok(todos);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar regra: {e.Message}." });
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

            var item = await _regraClassificacaoService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound(new { message = $"Regra não encontrada." });
            }

            return Ok(item);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar regra: {e.Message}." });
        }
    }

    /// <summary>
    /// Obtém o total de registros cadastrados.
    /// </summary>
    /// <returns>Resultado da operação.</returns>
    /// <exception cref="System.Exception">Lançada em caso de erro interno ou falha no processamento.</exception>
    [HttpGet("count")]
    public async Task<IActionResult> Contar()
    {
        try
        {
            var count = await _regraClassificacaoService.CountAsync();
            return Ok(new { count });
        }
        catch (Exception)
        {
            return NotFound(new { message = $"Erro ao procurar regra." });
        }
    }

    /// <summary>
    /// Atualiza um registro existente.
    /// </summary>
    /// <param name="id">Identificador exclusivo na rota (deve ser maior que zero).</param>
    /// <param name="dto">DTO contendo os novos dados para atualização.</param>
    /// <returns>Mensagem de confirmação da atualização.</returns>
    /// <response code="200">Sucesso ao atualizar o registro.</response>
    /// <response code="400">Erros na validação do modelo ou ID divergente entre a rota e o corpo.</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Atualizar([FromRoute] long id, [FromBody] RegraClassificacaoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new
                    { message = "Regra invalido na rota. Envie id maior que zero." });
            }

            if (dto.IdRegra <= 0)
            {
                dto.IdRegra = id;
            }

            if (dto.IdRegra != id)
            {
                return BadRequest(new { message = "Id da rota e regra do corpo devem ser iguais." });
            }

            var regra = _mapper.Map<RegraClassificacaoModel>(dto);
            var result = await _regraClassificacaoService.UpdateAsync(regra);
            return Ok(new { message = $"Regra atualizada com sucesso: {result.IdRegra}" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao atualizar regra." });
        }
    }

    /// <summary>
    /// Exclui um registro pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador exclusivo do registro a ser excluído.</param>
    /// <returns>Mensagem de confirmação da exclusão.</returns>
    /// <response code="200">Sucesso ao deletar o registro.</response>
    /// <response code="404">Registro não encontrado no banco de dados.</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar([FromRoute] long id)
    {
        try
        {
            var deleted = await _regraClassificacaoService.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = $"Regra não encontrada." });

            return Ok(new { message = $"Regra deletada com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao deletar regra." });
        }
    }

    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }

    /// <summary>
    /// Classifica para Ambas, Fichas ou Ordens com base uma lista de palavras armazenadas no banco
    /// </summary>
    /// <returns>Lista de notas atualizadas</returns>
    [HttpGet("palavras")]
    public async Task<IActionResult> ClassificacaoPalavrasNotaCongas(Fichas buscaTabela)
    {
        var config = await _configuracaoGlobalService.ObterConfiguracaoAsync();
        if (config.ClassificacaoAutomatica != ModoClassificacaoAutomatica.ClassificarPalavrasChave)
        {
            return BadRequest(new { message = "Classificação por palavras desativada nas configurações globais." });
        }

        try
        {
            // passar uma lista vazia por enquanto 
            var message = "";
            if (buscaTabela == Fichas.Ambas || buscaTabela == Fichas.Notas)
            {
                await _notaService.AutoClassifyWords();
                message = "Notas classificadas com sucesso";
            }

            if (buscaTabela == Fichas.Ambas || buscaTabela == Fichas.Ordens)
            {
                await _ordemService.AutoClassifyWords();
                message += "| Ordens classificadas com sucesso";
            }

            if (!message.Any())
            {
                return BadRequest(new { message = "Ficha inválida" });
            }

            return Ok(new { message = $"{message} !" });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao classificar notas: {e.Message}" });
        }
    }

    /// <summary>
    /// Classifica as Ambas, Fichas ou Ordens usando a IA catboost
    /// </summary>
    [HttpGet("catboost")]
    public async Task<IActionResult> ClassificacaoCatboostNota(Fichas buscaTabela)
    {
        var config = await _configuracaoGlobalService.ObterConfiguracaoAsync();
        if (config.ClassificacaoAutomatica != ModoClassificacaoAutomatica.ClassificarCatBoost)
        {
            return BadRequest(new { message = "Classificação por catboost desativada nas configurações globais." });
        }

        try
        {
            var message = "";
            if (buscaTabela == Fichas.Ambas || buscaTabela == Fichas.Notas)
            {
                await _notaService.AutoClassifyCatboost();
                message = "Notas classificadas com sucesso";
            }

            if (buscaTabela == Fichas.Ambas || buscaTabela == Fichas.Ordens)
            {
                await _ordemService.AutoClassifyCatboost();
                message += "| Ordens classificadas com sucesso";
            }

            if (!message.Any())
            {
                return BadRequest(new { message = "Ficha inválida" });
            }

            return Ok(new { message = $"{message} !" });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao classificar notas: {e.Message}" });
        }
    }

    /// <summary>
    /// Busca notas ou ordens de manutenção com filtros no body, retorna uma lista de objetos com os dados relevantes
    /// </summary>
    /// <param name="busca">Modelo de busca com campos comparáveis</param>
    /// <param name="pagina">Página a ser exibida</param>
    /// <param name="tamanho">Número de elementos presente na página</param>
    /// <returns>Lista de Nota e Ordens com base nos filtros</returns>
    [HttpPost("filtro")]
    public async Task<IActionResult> BuscarNotaOrdem(
        [FromBody] BuscaOrdemNotaDto busca,
        [FromQuery] Fichas? tFichas = Fichas.Ambas)
    {
        try
        {


            if (!tFichas.HasValue)
            {
                tFichas = Fichas.Ambas;
            }

            var resultadoBusca = await _searchService.FiltraNotaOrdemAsync(busca, null, null);

            List<object> resultados = new List<object>();

            if (tFichas == Fichas.Ambas || tFichas == Fichas.Notas)
            {
                var notas = resultadoBusca.Item1;
                resultados.Add(notas);
            }

            if (tFichas == Fichas.Ambas || tFichas == Fichas.Ordens)
            {
                var ordens = resultadoBusca.Item2;
                resultados.Add(ordens);
            }
            
            int totalGeral = resultados.Count;
            
            object resultado = new
            {
                Total = totalGeral,
                Resultado = resultados
            };
            
            return Ok(resultado);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar nota/ordem: {e.Message}." });
        }
    }
}