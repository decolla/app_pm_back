using System.Net.Http.Headers;
using app_pm_back.Data;
using app_pm_back.DTO;
using app_pm_back.Interfaces;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe UsuarioService.
/// </summary>
public class UsuarioService : BaseCrudService<UsuarioModel>
{
    private readonly ILogger<UsuarioService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _clientId;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenService _tokenService;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="UsuarioService"/>.
    /// </summary>
    public UsuarioService(AppDbContext context, ILogger<UsuarioService> logger, HttpClient httpClient,
        IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ITokenService tokenService) : base(context)
    {
        _logger = logger;
        _httpClient = httpClient;
        _clientId = configuration["Keycloak:ClientId"] ?? "comgas!pm";
        _httpContextAccessor = httpContextAccessor;
        _tokenService = tokenService;
    }


    public new async Task<List<UsuarioModel>> GetAllNotasAsync()
    {
        var query = AplicarIncludes(_context.Set<UsuarioModel>().AsQueryable());
        return await query.ToListAsync();
    }

    /// <summary>
    /// Retorna os usuários paginados
    /// </summary>
    public new async Task<object> GetPagedAsync(int pagina, int tamanho)
    {
        var query = AplicarIncludes(_context.Set<UsuarioModel>().AsQueryable());

        int count = await query.CountAsync();

        var result = await query
            .OrderBy(u => u.IdUsuario)
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToListAsync();

        return new { Total = count, Result = result };
    }

    /// <summary>
    /// Atualiza uma lista de usuários.
    /// </summary>
    /// <param name="usuarios">Lista de usuários a serem atualizados.</param>
    /// <returns>Lista de usuários atualizados.</returns>
    public async Task<List<UsuarioModel>> AtualizarUsuariosList(List<UsuarioModel> usuarios)
    {
        foreach (var usuario in usuarios)
        {
            var existingUsuario = await _context.Set<UsuarioModel>()
                .FirstOrDefaultAsync(u => u.IdUsuario == usuario.IdUsuario);
            if (existingUsuario != null)
            {
                existingUsuario.Nome = usuario.Nome;
                existingUsuario.CPF = usuario.CPF;
                existingUsuario.Telefone = usuario.Telefone;
                existingUsuario.Escopos = usuario.Escopos;
                existingUsuario.NivelPermissao = usuario.NivelPermissao;
            }
            else
            {
                _context.Set<UsuarioModel>().Add(usuario);
            }
        }

        await _context.SaveChangesAsync();
        return usuarios;
    }

    /// <summary>
    /// Aplica includes na consulta
    /// </summary>
    protected override IQueryable<UsuarioModel> AplicarIncludes(IQueryable<UsuarioModel> query)
    {
        return query
            .Include(u => u.Ctms)
            .Include(u => u.Notas)
            .Include(u => u.Ordens);
    }

    /// <summary>
    /// Atribui a reponsabilidade de uma ordem para um usuário
    /// </summary>
    public async Task<bool> AtribuirPermissao(string? nomeUser, long? ordem)
    {
        try
        {
            var usuario = await _context.Set<UsuarioModel>().FirstOrDefaultAsync(u => u.Nome == nomeUser);
            if (usuario != null)
            {
                var ordemModel = await _context.Set<OrdemModel>().FirstOrDefaultAsync(o => o.IdOrdem == ordem);
                if (ordemModel != null)
                {
                    usuario.Ordens.Add(ordemModel);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar responsabilidade da ordem {ordem} ao usuário {nomeUser}", ordem,
                nomeUser);
            throw;
        }
    }

    public async Task<List<UsuarioModel>> SearchUsuario(string? cpf, string? nome, string? funcao, string? schema)
    {
        var usuarios = await _context.Set<UsuarioModel>()
            .Where(u => (string.IsNullOrEmpty(cpf) || u.CPF!.Contains(cpf)) &&
                        (string.IsNullOrEmpty(nome) || u.Nome!.Contains(nome)) &&
                        //(string.IsNullOrEmpty(funcao) || u.Funcao.Contains(funcao)) &&
                        // TODO: Adicionar filtro por FUNÇÃO se necessário
                        (string.IsNullOrEmpty(schema) || u.Escopos.Contains(schema)))
            .ToListAsync();

        return usuarios;
    }

    /// <summary>
    /// Chamado pelo Frontend. Usa o token do utilizador logado.
    /// </summary>
    public async Task<List<UsuarioModel>> SincronizarUsuariosPortalAsync()
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
        var token = authHeader?.StartsWith("Bearer ") == true ? authHeader.Substring(7).Trim() : authHeader;

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Sincronização manual falhou: Token ausente.");
            return new List<UsuarioModel>();
        }

        return await ExecutarSincronizacaoCoreAsync(token);
    }

    /// <summary>
    /// Chamado pelo Hangfire
    /// </summary>
    public async Task<List<UsuarioModel>> SincronizarUsuariosPortalBackgroundAsync()
    {
        _logger.LogInformation("Gerando token M2M para sincronização em background...");
        var token = await _tokenService.GetAdminAccessTokenAsync();
        
        return await ExecutarSincronizacaoCoreAsync(token);
    }

    /// <summary>
    /// Lógica central partilhada.
    /// </summary>
    private async Task<List<UsuarioModel>> ExecutarSincronizacaoCoreAsync(string token)
    {
        var schema = Config.RouteConfig.GetDatabaseSchema();
        var encodedClientId = Uri.EscapeDataString(_clientId);
        string url = $"https://10.3.192.200/portal/api/{schema}/43000/0/All/Application/ByClientId/{encodedClientId}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.TryAddWithoutValidation("Accept", "*/*");
        request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {token}");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var usuariosPortal = await response.Content.ReadFromJsonAsync<List<PortalAppUserResponseDto>>();
        if (usuariosPortal == null || !usuariosPortal.Any()) return new List<UsuarioModel>();

        List<UsuarioModel> usuariosAtualizados = new List<UsuarioModel>();

        foreach (var item in usuariosPortal)
        {
            var pUser = item.User;
            var usuario = await _context.Set<UsuarioModel>().FirstOrDefaultAsync(u => u.PortalId == pUser.Id);

            if (usuario == null)
            {
                usuario = new UsuarioModel { PortalId = pUser.Id };
                _context.Set<UsuarioModel>().Add(usuario);
            }
            
            usuario.Nome = pUser.Name;
            usuario.CPF = pUser.Cpf;
            usuario.Telefone = pUser.PhoneNumber;
            usuario.Escopos = pUser.Scopes ?? new List<string>();
            usuario.NivelPermissao = DeterminarNivelPermissao(item.AccessLevel);
            
            usuariosAtualizados.Add(usuario);
        }
        
        await _context.SaveChangesAsync();
        return usuariosAtualizados;
    }

    /// <summary>
    /// Mapeia o Access Level que vem do portal para o Enum interno do PM
    /// </summary>
    private NivelPermissao DeterminarNivelPermissao(string applicationAccessLevel)
    {
        if (string.IsNullOrEmpty(applicationAccessLevel)) return NivelPermissao.Usuario;

        return applicationAccessLevel.ToUpper() switch
        {
            "MANAGE" => NivelPermissao.Admin,
            "EDIT" => NivelPermissao.Pendente,
            "VIEW" => NivelPermissao.Usuario,
            _ => NivelPermissao.Usuario
        };
    }

    public async Task<List<UsuarioModel>> ObterIntermediariosSemVinculoAsync()
    {
        var usuarios = await _context.PM_Usuarios
            .Where(u => u.NivelPermissao == NivelPermissao.Pendente)
            .ToListAsync();

        return usuarios;
    }
}