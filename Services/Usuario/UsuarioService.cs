using app_pm_back.Data;
using app_pm_back.DTO;
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
    private readonly string _pmApplicationId;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="UsuarioService"/>.
    /// </summary>
    public UsuarioService(AppDbContext context, ILogger<UsuarioService> logger , HttpClient httpClient, IConfiguration configuration) : base(context)
    {
        _logger = logger ;
        _httpClient = httpClient;
        _pmApplicationId = configuration["Portal:PmApplicationId"] ?? string.Empty; 
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

    public async Task<List<UsuarioModel>> AtualizarUsuariosList (List<UsuarioModel> usuarios)
    {
        foreach (var usuario in usuarios)
        {
            var existingUsuario = await _context.Set<UsuarioModel>().FirstOrDefaultAsync(u => u.IdUsuario == usuario.IdUsuario);
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
    
    public async Task<List<UsuarioModel>> SincronizarUsuariosPortalAsync()
    {
        int offset = 0;
        int limit = 100;
        bool hasMoreData = true;

        var schema = Config.RouteConfig.GetDatabaseSchema();
        
        while (hasMoreData)
        {
            // endpoint do thiagão
            string url = $"api/{schema}/43000/0/All?offset={offset}&limit={limit}";
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var portalData = await response.Content.ReadFromJsonAsync<PortalPaginatedResponseDto>();
            
            if (portalData == null || !portalData.Items.Any())
            {
                break; // sai do loop se não vier nada
            }

            foreach (var usuarioPortal in portalData.Items)
            {
                // verifica se o usuário já existe no banco local
                var usuario = await _context.PM_Usuarios.FirstOrDefaultAsync(u => u.PortalId == usuarioPortal.Id);

                if (usuario != null)
                {
                    usuario = new UsuarioModel()
                    {
                        PortalId = usuarioPortal.Id,
                    };
                    _context.PM_Usuarios.Add(usuario);
                }

                usuario.Nome = usuarioPortal.Name;
                usuario.CPF = usuarioPortal.Cpf;
                usuario.Telefone = usuarioPortal.PhoneNumber;
                usuario.Escopos = usuarioPortal.Scopes;
                usuario.NivelPermissao = DeterminarNivelPermissao(usuarioPortal.ApplicationPermissions
                    .FirstOrDefault(ap => ap.ApplicationId == _pmApplicationId)?.ApplicationAccessLevel ?? "VIEW");
            }
        }
        
        return await _context.SaveChangesAsync() > 0 
            ? await _context.PM_Usuarios.ToListAsync() 
            : new List<UsuarioModel>();
    }
    
    /// <summary>
    /// Mapeia o Access Level que vem do portal para o Enum interno do PM
    /// </summary>
    private NivelPermissao DeterminarNivelPermissao(string applicationAccessLevel)
    {
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
            .Where(u => u.NivelPermissao == NivelPermissao.Pendente 
                        && !u.Ctms.Any()
                        && !u.Gpms.Any()
                        )
            .ToListAsync();

        return usuarios;
    }
}