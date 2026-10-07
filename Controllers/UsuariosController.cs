using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services.Manutencao;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

/// <summary>
/// Classe UsuariosController.
/// </summary>
[ApiController]
[Authorize(Policy = "View")]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/usuarios")]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioService _service;
    private readonly IMapper _mapper;
    private readonly string _operador = "usuario sap";

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="UsuariosController"/>.
    /// </summary>
    public UsuariosController(UsuarioService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }
    
    // /// <summary>
    // /// Sincroniza os usuários do portal com o sistema, apenas usuários com role Admin no Keycloak podem chamar.
    // /// </summary>
    // /// <param name="payload">Lista de usuários do portal a serem sincronizados.</param>
    // /// <returns></returns>
    // [HttpPost("sincronizar-portal")]
    // [Authorize(Policy = "Admin")] 
    // public async Task<IActionResult> SincronizarPortal([FromBody] List<SyncUsuarioDto> payload)
    // {
    //     if (payload == null || !payload.Any())
    //         return BadRequest("O payload não pode estar vazio.");
    //
    //     var result = await _service.SincronizarUsuariosPortalAsync(payload);
    //     return Ok(result);
    // }
    
    /// <summary>
    /// Aciona a sincronização buscando os usuários ativamente na API do Portal.
    /// Apenas usuários com role Admin no Keycloak podem chamar.
    /// </summary>
    [HttpPost("sincronizar-portal")]
    [Authorize(Policy = "Admin")] 
    public async Task<IActionResult> SincronizarPortal()
    {
        var result = await _service.SincronizarUsuariosPortalAsync();
        return Ok(result);
    }

    /// <summary>
    /// Retorna os usuários de nível intermediário que não estão vinculados a nenhum GPM ou CTM.
    /// </summary>
    [HttpGet("intermediarios-sem-vinculo")]
    [Authorize(Policy = "Manage")] 
    public async Task<IActionResult> ObterIntermediariosSemVinculo()
    {
        try
        {
            var usuarios = await _service.ObterIntermediariosSemVinculoAsync();

            var result = _mapper.Map<List<UsuarioDto>>(usuarios);
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar usuários intermediários: {e.Message}" });
        }
    }

    /// <summary>
    /// Cria um novo registro.
    /// </summary>
    /// <param name="dto">DTO contendo os dados da requisição.</param>
    /// <returns>O objeto criado com status 201 (Created).</returns>
    /// <response code="201">Registro criado com sucesso.</response>
    /// <response code="400">Erros na validação do modelo ou dados fornecidos inválidos.</response>
    [HttpPost]
    [Authorize(Policy = "Manage")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] UsuarioDto dto)
    {
        try
        {
            var usuario = _mapper.Map<UsuarioModel>(dto);

            var criado = await _service.CreateAsync(usuario);
            return CreatedAtAction(nameof(ObterPorId), new { id = criado.IdUsuario }, criado);
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
                var paginado = await _service.GetNotasPaginadasAsync(p, t);
                return Ok(paginado);
            }

            var todos = await _service.GetAllNotasAsync();
            return Ok(todos);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao processar {_operador}: {e.Message}." });
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

            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound(new { message = $"{_operador} nao encontrado." });
            }

            return Ok(item);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao buscar {_operador}: {e.Message}." });
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
            var count = await _service.CountAsync();
            return Ok(new { count });
        }
        catch (Exception)
        {
            return NotFound(new { message = $"Erro ao procurar {_operador}." });
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
    [Authorize(Policy = "Manage")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Atualizar([FromRoute] long id, [FromBody] UsuarioDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (id <= 0)
            {
                return BadRequest(new { message = "IdUsuario invalido na rota. Envie id maior que zero." });
            }

            if (dto.IdUsuario <= 0)
            {
                dto.IdUsuario = id;
            }

            if (dto.IdUsuario != id)
            {
                return BadRequest(new { message = "Id da rota e IdUsuario do corpo devem ser iguais." });
            }

            var usuario = _mapper.Map<UsuarioModel>(dto);
            var result = await _service.UpdateAsync(usuario);
            return Ok(new { message = $"{_operador} atualizado com sucesso: {result.IdUsuario}" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao atualizar {_operador}." });
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
    [HttpPut("list")]
    [Authorize(Policy = "Manage")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Atualizar([FromBody] List<UsuarioDto> listUsr)
    {
        try
        {
            List<UsuarioModel> usuarios = _mapper.Map<List<UsuarioModel>>(listUsr);
            
            var result = await _service.AtualizarUsuariosList(usuarios);
            return Ok(usuarios);
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
    [Authorize(Policy = "Manage")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar([FromRoute] long id)
    {
        try
        {
            var deleted = await _service.DeleteNotaAsync(id);
            if (!deleted) return NotFound(new { message = $"{_operador} nao encontrado." });

            return Ok(new { message = $"{_operador} deletado com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = $"Erro ao deletar {_operador}." });
        }
    }
        
    /// <summary>
    /// Atribui a responsabilidade de uma ordem para um usuário
    /// </summary>
    /// <response code="400">Erros na validação do modelo ou dados fornecidos inválidos.</response>
    [HttpPost("atribuir")]
    [Authorize(Policy = "Manage")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AtribuirOrdem([FromQuery] string nome, [FromQuery] long ordem)
    {
        try
        {
            if (string.IsNullOrEmpty(nome) || ordem <= 0)
            {
                return ValidationProblem(ModelState);
            }
            

            var resposta = await _service.AtribuirPermissao(nome,ordem);
            return Ok(resposta);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar {_operador}: {e.Message}." });
        }
    }
    
    /// <summary>
    /// Busca uma lista de funcionários com base nos filtros passados 
    /// </summary>
    /// <param name="cpf">CPF do usuário</param>
    /// <param name="nome">Nome do usuário</param>
    /// <param name="funcao">Função do usuário</param>
    /// <param name="schema">Esquema do usuário</param>
    /// <returns></returns>
    [HttpGet("pesquisar")]
    public async Task<IActionResult> Pesquisar([FromQuery] string? cpf, [FromQuery] string? nome, [FromQuery] string? funcao, [FromQuery] string? schema)
    {
        try
        {
            var usuarios = await _service.SearchUsuario(cpf, nome, funcao, schema);
            return Ok(usuarios);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao pesquisar usuários: {e.Message}" });
        }
    }

    private static bool TemPaginacaoInvalida(int? pagina, int? tamanho)
    {
        return (pagina.HasValue && pagina <= 0) || (tamanho.HasValue && tamanho <= 0);
    }
}