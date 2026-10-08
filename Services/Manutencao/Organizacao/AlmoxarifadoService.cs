using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe AlmoxarifadoService.
/// </summary>
public class AlmoxarifadoService : BaseCrudService<AlmoxarifadoModel>
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="AlmoxarifadoService"/>.
    /// </summary>
    public AlmoxarifadoService(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Cria um almoxarifado e associa as ferramentas e usuários informados.
    /// </summary>
    public async Task<AlmoxarifadoModel> CriarComAssociacoesAsync(
        AlmoxarifadoModel almoxarifado,
        IEnumerable<long> idsFerramentas,
        IEnumerable<long> idsUsuarios)
    {
        var ferramentaIds = ValidarIds(idsFerramentas, "ferramentas");
        var usuarioIds = ValidarIds(idsUsuarios, "usuários");

        var ferramentas = await _context.Set<FerramentaModel>()
            .Where(f => ferramentaIds.Contains(f.IdFerramenta))
            .ToListAsync();
        if (ferramentas.Count != ferramentaIds.Count)
        {
            throw new KeyNotFoundException("Uma ou mais ferramentas informadas não foram encontradas.");
        }

        var usuarios = await _context.Set<UsuarioModel>()
            .Where(u => usuarioIds.Contains(u.IdUsuario))
            .ToListAsync();
        if (usuarios.Count != usuarioIds.Count)
        {
            throw new KeyNotFoundException("Um ou mais usuários informados não foram encontrados.");
        }

        almoxarifado.Ferramentas = ferramentas;
        almoxarifado.Usuarios = usuarios;
        await _dbSet.AddAsync(almoxarifado);
        await _context.SaveChangesAsync();

        return (await _context.Set<AlmoxarifadoModel>()
            .Include(a => a.Ferramentas)
            .Include(a => a.Usuarios)
            .FirstAsync(a => a.IdAlmoxarifado == almoxarifado.IdAlmoxarifado));
    }

    /// <inheritdoc />
    protected override IQueryable<AlmoxarifadoModel> AplicarIncludes(
        IQueryable<AlmoxarifadoModel> query)
    {
        return query
            .Include(a => a.Ferramentas)
            .Include(a => a.Usuarios);
    }

    private static List<long> ValidarIds(IEnumerable<long> ids, string entidade)
    {
        var lista = ids.Distinct().ToList();
        if (lista.Any(id => id <= 0))
        {
            throw new ArgumentException($"Os IDs de {entidade} devem ser maiores que zero.");
        }

        return lista;
    }
}