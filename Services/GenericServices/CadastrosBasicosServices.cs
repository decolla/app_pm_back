using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe CpmService.
/// </summary>
public class CpmService : BaseCrudService<CpmModel>
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="CpmService"/>.
    /// </summary>
    public CpmService(AppDbContext context) : base(context)
    {
    }
}

/// <summary>
/// Classe CtmService.
/// </summary>
public class CtmService : BaseCrudService<CtmModel>
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="CtmService"/>.
    /// </summary>
    public CtmService(AppDbContext context) : base(context)
    {
    }
    
    /// <summary>
    /// Obtém os usuários associados a um centro de trabalho.
    /// </summary>
    /// <param name="ctmId">O ID do centro de trabalho.</param>
    /// <returns>A lista de usuários associados ao centro de trabalho.</returns>
    public async Task<List<UsuarioModel>> GetUsuariosCtmAsync(long ctmId)
    {
        var ctm = await _context.Set<CtmModel>().FindAsync(ctmId);
        if (ctm == null)
        {
            return new List<UsuarioModel>();
        }
        return await _context.Set<UsuarioModel>().Where(u => u.Ctms.Contains(ctm)).ToListAsync();
    }
    
    public async Task<bool> AtribuirFichas(NotaOrdem tipoFicha, long idFicha, long idGrupo, List<long> idsUsuariosSelecionados)
    {
        // busca usuários deste grupo
        var usuariosValidosDoGrupo = await _context.Set<UsuarioModel>()
            .Where(u => u.Ctms.Any(c => c.IdCTM == idGrupo))
            .ToListAsync();

        var idsValidosDoGrupo = usuariosValidosDoGrupo.Select(u => u.IdUsuario).ToList();

        if (tipoFicha == NotaOrdem.Ordem)
        {
            var ficha = await _context.Set<OrdemModel>()
                .Include(o => o.Usuarios)
                .FirstOrDefaultAsync(o => o.IdOrdem == idFicha);

            if (ficha == null) throw new KeyNotFoundException($"Ordem com ID {idFicha} não encontrada.");

            var usuariosAtuaisDesteGrupo = ficha.Usuarios.Where(u => idsValidosDoGrupo.Contains(u.IdUsuario)).ToList();

            var remover = usuariosAtuaisDesteGrupo.Where(u => !idsUsuariosSelecionados.Contains(u.IdUsuario)).ToList();
            foreach (var u in remover) ficha.Usuarios.Remove(u);

            var idsAtuaisNaFicha = ficha.Usuarios.Select(x => x.IdUsuario).ToList();
            var adicionar = usuariosValidosDoGrupo.Where(u => idsUsuariosSelecionados.Contains(u.IdUsuario) && !idsAtuaisNaFicha.Contains(u.IdUsuario)).ToList();
            
            foreach (var u in adicionar) ficha.Usuarios.Add(u);
        }
        else 
        {
            var ficha = await _context.Set<NotaModel>()
                .Include(n => n.Usuarios)
                .FirstOrDefaultAsync(n => n.IdNota == idFicha);

            if (ficha == null) throw new KeyNotFoundException($"Nota com ID {idFicha} não encontrada.");

            var usuariosAtuaisDesteGrupo = ficha.Usuarios.Where(u => idsValidosDoGrupo.Contains(u.IdUsuario)).ToList();

            var remover = usuariosAtuaisDesteGrupo.Where(u => !idsUsuariosSelecionados.Contains(u.IdUsuario)).ToList();
            foreach (var u in remover) ficha.Usuarios.Remove(u);

            var idsAtuaisNaFicha = ficha.Usuarios.Select(x => x.IdUsuario).ToList();
            var adicionar = usuariosValidosDoGrupo.Where(u => idsUsuariosSelecionados.Contains(u.IdUsuario) && !idsAtuaisNaFicha.Contains(u.IdUsuario)).ToList();
            
            foreach (var u in adicionar) ficha.Usuarios.Add(u);
        }

        await _context.SaveChangesAsync();
        return true; 
    }
}
    
/// <summary>
/// Classe GpmService.
/// </summary>
public class GpmService : BaseCrudService<GpmModel>
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="GpmService"/>.
    /// </summary>
    public GpmService(AppDbContext context) : base(context)
    {
    }
    
    /// <summary>
    /// Obtém os usuários associados a um grupo de planejamento.
    /// </summary>
    /// <param name="gpmId">O ID do grupo de planejamento.</param>
    /// <returns>A lista de usuários associados ao grupo de planejamento.</returns>
    public async Task<List<UsuarioModel>> GetUsuariosGpmAsync(long gpmId)
    {
        var gpm = await _context.Set<GpmModel>().FindAsync(gpmId);
        if (gpm == null)
        {
            return new List<UsuarioModel>();
        }
        return await _context.Set<UsuarioModel>().Where(u => u.Gpms.Contains(gpm)).ToListAsync();
    }
    
   public async Task<bool> AtribuirFichas(NotaOrdem tipoFicha, long idFicha, long idGrupo, List<long> idsUsuariosSelecionados)
    {
        // busca usuários deste grupo
        var usuariosValidosDoGrupo = await _context.Set<UsuarioModel>()
            .Where(u => u.Gpms.Any(g => g.IdGPM == idGrupo))
            .ToListAsync();

        var idsValidosDoGrupo = usuariosValidosDoGrupo.Select(u => u.IdUsuario).ToList();

        if (tipoFicha == NotaOrdem.Ordem)
        {
            var ficha = await _context.Set<OrdemModel>()
                .Include(o => o.Usuarios)
                .FirstOrDefaultAsync(o => o.IdOrdem == idFicha);

            if (ficha == null) throw new KeyNotFoundException($"Ordem com ID {idFicha} não encontrada.");

            var usuariosAtuaisDesteGrupo = ficha.Usuarios.Where(u => idsValidosDoGrupo.Contains(u.IdUsuario)).ToList();

            var remover = usuariosAtuaisDesteGrupo.Where(u => !idsUsuariosSelecionados.Contains(u.IdUsuario)).ToList();
            foreach (var u in remover)
            {
                ficha.Usuarios.Remove(u);
            }

            var idsAtuaisNaFicha = ficha.Usuarios.Select(x => x.IdUsuario).ToList();
            var adicionar = usuariosValidosDoGrupo.Where(u => idsUsuariosSelecionados.Contains(u.IdUsuario) && !idsAtuaisNaFicha.Contains(u.IdUsuario)).ToList();
            
            foreach (var u in adicionar)
            {
                ficha.Usuarios.Add(u);
            }
        }
        else 
        {
            var ficha = await _context.Set<NotaModel>()
                .Include(n => n.Usuarios)
                .FirstOrDefaultAsync(n => n.IdNota == idFicha);

            if (ficha == null) throw new KeyNotFoundException($"Nota com ID {idFicha} não encontrada.");

            var usuariosAtuaisDesteGrupo = ficha.Usuarios.Where(u => idsValidosDoGrupo.Contains(u.IdUsuario)).ToList();

            var remover = usuariosAtuaisDesteGrupo.Where(u => !idsUsuariosSelecionados.Contains(u.IdUsuario)).ToList();
            foreach (var u in remover) ficha.Usuarios.Remove(u);

            var idsAtuaisNaFicha = ficha.Usuarios.Select(x => x.IdUsuario).ToList();
            var adicionar = usuariosValidosDoGrupo.Where(u => idsUsuariosSelecionados.Contains(u.IdUsuario) && !idsAtuaisNaFicha.Contains(u.IdUsuario)).ToList();
            
            foreach (var u in adicionar) ficha.Usuarios.Add(u);
        }

        await _context.SaveChangesAsync();
        return true; 
    }
}