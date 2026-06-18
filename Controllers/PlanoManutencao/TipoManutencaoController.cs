using app_pm_back.DTO;
using app_pm_back.Data;
using app_pm_back.Models.Domain;
using app_pm_back.Services.Manutencao;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Controllers.PlanoManutencao;

/// <summary>
/// Endpoints de tipo de manutencao .
/// </summary>
[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/tipos-manutencao")]
public class TipoManutencaoController : ControllerBase
{
    private readonly string _operador = "tipo de manutenção";
    private readonly TipoManutencaoService _tipoManutencaoService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="TipoManutencaoController"/>.
    /// </summary>
    public TipoManutencaoController(TipoManutencaoService tipoManutencaoService, IMapper mapper)
    {
        _tipoManutencaoService = tipoManutencaoService;
        _mapper = mapper;
    }

        /// <summary>
    /// Cria um novo registro.
    /// </summary>
    /// <param name="dto">DTO contendo os dados da requisição.</param>
    /// <returns>O objeto criado com status 201 (Created).</returns>
    /// <response code="201">Registro criado com sucesso.</response>
    /// <response code="400">Erros na validação do modelo ou dados fornecidos inválidos.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] TipoManutencaoDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var obj = _mapper.Map<TipoManutencaoModel>(dto);
            var novoObj = await _tipoManutencaoService.CreateAsync(obj);
            return CreatedAtAction(nameof(BuscarPorId), new { id = novoObj.IdTipoManutencao }, novoObj);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar {_operador}: {e.Message}." });
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
                var itensPaginados = await _tipoManutencaoService.GetNotasPaginadasAsync(p, t);
                return Ok(itensPaginados);
            }

            var itens = await _tipoManutencaoService.GetAllNotasAsync();
            return Ok(itens);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar {_operador}: {e.Message}" });
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
            var total = await _tipoManutencaoService.CountNotasAsync();
            return Ok(new { count = total });
        }
        catch (Exception)
        {
            return NotFound(new { message = $"Erro ao procurar {_operador}." });
        }
    }

        /// <summary>
    /// Obtém um registro específico.
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
    public async Task<IActionResult> BuscarPorId(long id)
    {
        try
        {
            var obj = await _tipoManutencaoService.GetCompletoByIdAsync(id);
            if (obj == null) return NotFound(new { message = $"{_operador} não encontrado(a)." });

            return Ok(obj);
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao procurar {_operador}." });
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
    public async Task<IActionResult> Atualizar([FromRoute] long id, [FromBody] TipoManutencaoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "IdTipoManutencao invalido na rota. Envie id maior que zero." });
            }

            if (dto.IdTipoManutencao <= 0)
            {
                dto.IdTipoManutencao = id;
            }

            if (id != dto.IdTipoManutencao)
            {
                return BadRequest(new
                {
                    message = "Id da rota e Id do objeto no corpo da requisição devem ser iguais, ou o do dto = 0."
                });
            }

            var obj = _mapper.Map<TipoManutencaoModel>(dto);
            var result = await _tipoManutencaoService.UpdateBasicoAsync(obj);
            return Ok(new { message = $"{_operador} atualizado com sucesso: " + result.IdTipoManutencao });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao atualizar {_operador}." });
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
    public async Task<IActionResult> Deletar(long id)
    {
        try
        {
            var deleted = await _tipoManutencaoService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = $"{_operador} não encontrado(a)." });

            return Ok(new { message = $"{_operador} deletado(a) com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao deletar um(a) {_operador}." });
        }
    }

    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }
}

/// <summary>
/// Endpoints de tipo de plano .
/// </summary>
[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/tipos-plano")]
public class TipoPlanoController : ControllerBase
{
    private readonly string _operador = "tipo de plano";
    private readonly TipoPlanoService _tipoPlanoService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="TipoPlanoController"/>.
    /// </summary>
    public TipoPlanoController(TipoPlanoService tipoPlanoService, IMapper mapper)
    {
        _tipoPlanoService = tipoPlanoService;
        _mapper = mapper;
    }

        /// <summary>
    /// Cria um novo registro.
    /// </summary>
    /// <param name="dto">DTO contendo os dados da requisição.</param>
    /// <returns>O objeto criado com status 201 (Created).</returns>
    /// <response code="201">Registro criado com sucesso.</response>
    /// <response code="400">Erros na validação do modelo ou dados fornecidos inválidos.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar(
        [FromBody] TipoPlanoDto dto,
        [FromServices] AppDbContext context)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (!dto.IdTipoEstrategia.HasValue && !dto.IdPlano.HasValue)
        {
            return BadRequest(new
            {
                message = "Regra de Negócio: informe PlanoManutencaoId quando nao houver TipoEstrategiaId."
            });
        }

        if (dto.IdPlano.HasValue)
        {
            var planoExiste = await context.PM_Planos
                .AnyAsync(p => p.IdPlano == dto.IdPlano.Value);

            if (!planoExiste)
            {
                return NotFound(new { message = "O Plano de Manutenção informado não existe." });
            }
        }

        if (dto.IdPlano.HasValue && dto.IdTipoEstrategia.HasValue)
        {
            var plano = await context.PM_Planos
                .FirstOrDefaultAsync(p => p.IdPlano == dto.IdPlano);

            if (plano == null)
                return NotFound(new { message = "O Plano de Manutenção informado não existe." });

            var tipoEstrategiaValido = await context.PM_TiposEstrategia
                .AnyAsync(te => te.IdTipoEstrategia == dto.IdTipoEstrategia && te.IdEstrategia == plano.IdEstrategia);

            if (!tipoEstrategiaValido)
            {
                return BadRequest(new
                {
                    message =
                        "Regra de Negócio: O Tipo de Estratégia selecionado não pertence à Estratégia vinculada a este Plano de Manutenção."
                });
            }
        }

        try
        {
            var obj = _mapper.Map<TipoPlanoModel>(dto);
            var novoObj = await _tipoPlanoService.CreateComCopiaAsync(obj);
            return CreatedAtAction(nameof(BuscarPorId), new { id = novoObj.IdTipoPlano }, novoObj);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar {_operador}: {e.Message}." });
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
                var acoesPaginadas = await _tipoPlanoService.GetNotasPaginadasAsync(p, t);
                return Ok(acoesPaginadas);
            }

            var acoes = await _tipoPlanoService.GetAllNotasAsync();
            return Ok(acoes);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar {_operador}: {e.Message}" });
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
            var notas = await _tipoPlanoService.CountNotasAsync();
            return Ok(new { count = notas });
        }
        catch (Exception)
        {
            return NotFound(new { message = $"Erro ao procurar {_operador}." });
        }
    }

        /// <summary>
    /// Obtém um registro específico.
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
    public async Task<IActionResult> BuscarPorId(long id)
    {
        try
        {
            var obj = await _tipoPlanoService.GetCompletoByIdAsync(id);
            if (obj == null) return NotFound(new { message = $"{_operador} não encontrado(a)." });

            return Ok(obj);
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao procurar {_operador}." });
        }
    }

    /// <summary>
    /// Lista os registros cadastrados com paginação.
    /// </summary>
    /// <param name="tipo">Tipo do tipo plano</param>
    /// <returns>Resultado da operação.</returns>
    /// <exception cref="System.Exception">Lançada em caso de erro interno ou falha no processamento.</exception>
    [HttpGet("tipado/{tipo:int}")]
    public async Task<IActionResult> ListarPorTipo([FromRoute] int tipo)
    {
        try
        {
            if (tipo != 0 && tipo != 1)
            {
                return BadRequest(new
                    { message = "Rota inválida. Use /tipo-plano/tipado/0 ou /tipo-plano/tipado/1" });
            }

            CategoriaTipoManutencao tipagem = (CategoriaTipoManutencao)tipo;
            var acoes = await _tipoPlanoService.GetTodosByTipo(tipagem);
            return Ok(acoes);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar {_operador}: {e.Message}" });
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
    public async Task<IActionResult> Atualizar([FromRoute] long id, [FromBody] TipoPlanoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "IdTipoManutencao invalido na rota. Envie id maior que zero." });
            }

            if (dto.IdTipoManutencao <= 0)
            {
                dto.IdTipoManutencao = id;
            }

            if (id != dto.IdTipoManutencao)
            {
                return BadRequest(new
                {
                    message = "Id da rota e Id do objeto no corpo da requisição devem ser iguais, ou o do dto = 0."
                });
            }

            if (!dto.IdTipoEstrategia.HasValue && !dto.IdPlano.HasValue)
            {
                return BadRequest(new
                {
                    message = "Regra de Negócio: informe PlanoManutencaoId quando nao houver TipoEstrategiaId."
                });
            }

            var obj = _mapper.Map<TipoPlanoModel>(dto);
            if (obj.IdTipoPlano <= 0)
            {
                return BadRequest(new
                    { message = "IdTipoPlano invalido. Envie IdTipoPlano maior que zero." });
            }

            var result = await _tipoPlanoService.UpdateComListasAsync(obj);
            return Ok(new { message = $"{_operador} atualizado com sucesso: " + result.IdTipoPlano });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao atualizar {_operador}." });
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
    public async Task<IActionResult> Deletar(long id)
    {
        try
        {
            var deleted = await _tipoPlanoService.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = $"{_operador} não encontrado(a)." });

            return Ok(new { message = $"{_operador} deletado(a) com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao deletar um(a) {_operador}." });
        }
    }

    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }
}