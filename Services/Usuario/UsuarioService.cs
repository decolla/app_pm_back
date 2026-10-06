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

    public async Task<List<UsuarioModel>> SearchUsuario(string? cpf, string? nome, string? funcao, int? schema)
    {
        var usuarios = await _context.Set<UsuarioModel>()
            .Where(u => (string.IsNullOrEmpty(cpf) || u.CPF!.Contains(cpf)) &&
                        (string.IsNullOrEmpty(nome) || u.Nome!.Contains(nome)) &&
                        //(string.IsNullOrEmpty(funcao) || u.Funcao.Contains(funcao)) &&
                        // TODO: Adicionar filtro por FUNÇÃO se necessário
                        (!schema.HasValue || u.Escopos.Contains(schema.Value)))
            .ToListAsync();

        return usuarios;
    }

    public async Task<List<UsuarioModel>> SincronizarUsuariosPortalAsync()
    {
        throw new NotImplementedException();
        return new List<UsuarioModel>();
        
        var schema = Config.RouteConfig.GetDatabaseSchema();
        
        var response = await _httpClient.GetAsync($"https://IP/portal/{schema}/api/usuarios");
        
        var usuariosSync = await response.Content.ReadFromJsonAsync<List<PortalPaginatedResponseDto>>();
        var result = new List<UsuarioModel>();
        
        //
        // foreach (var dto in usuariosSync)
        // {
        //     // processa cada usuário do portal, buscando pelo PortalId ou Nome
        //     var usuarioDb = await _context.Set<UsuarioModel>()
        //         .Include(u => u.Gpms)
        //         .Include(u => u.Ctms)
        //         .FirstOrDefaultAsync(u => u.PortalId == dto.Id || u.Nome == dto.Name);
        //
        //     if (usuarioDb == null)
        //     {
        //         // cria um novo usuário caso não exista
        //         usuarioDb = new UsuarioModel()
        //         {
        //             PortalId = dto.Id,
        //             Nome = dto.Name,
        //             CPF = dto.Cpf,
        //             Telefone = dto.Telefone,
        //             NivelPermissao = dto.NivelPermissao ?? NivelPermissao.Usuario,
        //             Permissoes = dto.Permissoes ?? PermissoesTabela.Nenhuma,
        //             Escopos = dto.Escopos
        //         };
        //         _context.Set<UsuarioModel>().Add(usuarioDb);
        //     }
        //     else
        //     {
        //         // atualiza os campos do usuário existente
        //         usuarioDb.PortalId = dto.Id; // salva UUID
        //         usuarioDb.Nome = dto.Name;
        //         if (!string.IsNullOrEmpty(dto.Cpf)) usuarioDb.CPF = dto.Cpf;
        //         if (!string.IsNullOrEmpty(dto.Telefone)) usuarioDb.Telefone = dto.Telefone;
        //         if (dto.NivelPermissao.HasValue) usuarioDb.NivelPermissao = dto.NivelPermissao;
        //         if (dto.Permissoes.HasValue) usuarioDb.Permissoes = dto.Permissoes;
        //         if (dto.Escopos.Any()) usuarioDb.Escopos = dto.Escopos;
        //     }
        //
        //     // atualiza as relações com GPMs e CTMs
        //     if (dto.GpmIds.Any())
        //     {
        //         usuarioDb.Gpms.Clear();
        //         var gpms  = await _context.Set<GpmModel>().Where(g => dto.GpmIds.Contains(g.IdGPM)).ToListAsync();
        //         foreach (var gpm in gpms)
        //         {
        //             usuarioDb.Gpms.Add(gpm);
        //         }
        //     }
        //     
        //     if (dto.CtmIds.Any())
        //     {
        //         usuarioDb.Ctms.Clear();
        //         var ctms = await _context.Set<CtmModel>().Where(c => dto.CtmIds.Contains(c.IdCTM)).ToListAsync();
        //         foreach (var ctm in ctms)
        //         {
        //             usuarioDb.Ctms.Add(ctm);
        //         }
        //     }
            
    }
    
    public async Task<List<UsuarioModel>> ObterIntermediariosSemVinculoAsync()
    {
        var usuarios = await _context.PM_Usuarios
            .Where(u => u.NivelPermissao == NivelPermissao.Ctm 
                        && !u.Ctms.Any()
                        && !u.Gpms.Any()
                        )
            .ToListAsync();

        return usuarios;
    }
}