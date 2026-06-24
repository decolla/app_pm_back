using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services;

/// <summary>
/// Cria registros "fantasma" para referencias que ainda nao existem no banco.
/// </summary>
public static class ImportGhostHelper
{
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="context">Parâmetro do método.</param>
    /// <param name="ids">Parâmetro do método.</param>
    public static async Task EnsureUsuariosSapAsync(AppDbContext context, IEnumerable<long?> ids)
    {
        var idsList = ids.Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
        if (!idsList.Any()) return;

        var existentes = await context.PM_Usuarios
            .Where(u => idsList.Contains(u.IdUsuario))
            .Select(u => u.IdUsuario)
            .ToListAsync();

        var faltantes = idsList.Except(existentes).ToList();
        if (!faltantes.Any()) return;

        var novos = faltantes.Select(id => new UsuarioModel
        {
            IdUsuario = id,
            Nome = $"IMPORTADO_{id}",
            CPF = $"000.000.000-{id:D2}",
            Permissoes = PermissoesTabela.Nenhuma
        });

        await context.PM_Usuarios.AddRangeAsync(novos);
        await context.SaveChangesAsync();
    }
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="context">Parâmetro do método.</param>
    /// <param name="codigos">Parâmetro do método.</param>

    public static async Task EnsurePlanosAsync(AppDbContext context, IEnumerable<long?> codigos)
    {
        var codigosList = codigos.Where(c => c.HasValue).Select(c => c!.Value).Distinct().ToList();
        if (!codigosList.Any()) return;

        var existentes = await context.PM_Planos
            .Where(p => codigosList.Contains(p.IdPlano))
            .Select(p => p.IdPlano)
            .ToListAsync();

        var faltantes = codigosList.Except(existentes).ToList();
        if (!faltantes.Any()) return;

        // Ensure a default Estrategia exists
        var defaultEstrategia = await context.PM_Estrategias.FirstOrDefaultAsync();
        if (defaultEstrategia == null)
        {
            defaultEstrategia = new EstrategiaModel
            {
                IdEstrategia = 1,
                Nome = "DEFAULT",
                TipoAtivoAplicavel = "DEFAULT"
            };
            await context.PM_Estrategias.AddAsync(defaultEstrategia);
            await context.SaveChangesAsync();
        }

        var novos = faltantes.Select(codigo => new PlanoModel
        {
            IdPlano = codigo,
            Nome = $"PLANO_{codigo}",
            IdEstrategia = defaultEstrategia.IdEstrategia
        });

        await context.PM_Planos.AddRangeAsync(novos);
        await context.SaveChangesAsync();
    }
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="context">Parâmetro do método.</param>
    /// <param name="ids">Parâmetro do método.</param>

    public static async Task EnsureNotasAsync(AppDbContext context, IEnumerable<long?> ids)
    {
        var idsList = ids.Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
        if (!idsList.Any()) return;

        var existentes = await context.PM_Notas
            .Where(n => idsList.Contains(n.IdNota))
            .Select(n => n.IdNota)
            .ToListAsync();

        var faltantes = idsList.Except(existentes).ToList();
        if (!faltantes.Any()) return;

        var novos = faltantes.Select(id => new NotaModel
        {
            IdNota = id,
            NumeroNota = id,
            DataNota = DateTime.UtcNow,
            CriadaPorUsuario = "IMPORTADO",
            DescricaoBreve = "IMPORTADO AUTOMATICAMENTE PELA IMPORTAÇÃO DE ORDEM",
        });

        await context.PM_Notas.AddRangeAsync(novos);
        await context.SaveChangesAsync();
    }
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="context">Parâmetro do método.</param>
    /// <param name="ids">Parâmetro do método.</param>

    public static async Task EnsureGpmAsync(AppDbContext context, IEnumerable<long?> ids)
    {
        var idsList = ids.Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
        if (!idsList.Any()) return;

        var existentes = await context.PM_Gpms
            .Where(g => idsList.Contains(g.IdGPM))
            .Select(g => g.IdGPM)
            .ToListAsync();

        var faltantes = idsList.Except(existentes).ToList();
        if (!faltantes.Any()) return;

        // Ensure a default CPM exists
        var defaultCpm = await context.PM_Cpms.FirstOrDefaultAsync();
        if (defaultCpm == null)
        {
            defaultCpm = new CpmModel
            {
                IdCPM = 1,
                NomeCentro = "DEFAULT",
                Organizacao = "DEFAULT"
            };
            await context.PM_Cpms.AddAsync(defaultCpm);
            await context.SaveChangesAsync();
        }

        var novos = faltantes.Select(id => new GpmModel
        {
            IdGPM = id,
            Nome = $"GPM_{id}",
            IdCPM = defaultCpm.IdCPM
        });

        await context.PM_Gpms.AddRangeAsync(novos);
        await context.SaveChangesAsync();
    }
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="context">Parâmetro do método.</param>
    /// <param name="ids">Parâmetro do método.</param>

    public static async Task EnsureImobilizadosAsync(AppDbContext context, IEnumerable<long?> ids)
    {
        var idsList = ids.Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
        if (!idsList.Any()) return;

        var existentes = await context.PM_Imobilizados
            .Where(i => idsList.Contains(i.IdImobilizado))
            .Select(i => i.IdImobilizado)
            .ToListAsync();

        var faltantes = idsList.Except(existentes).ToList();
        if (!faltantes.Any()) return;

        var novos = faltantes.Select(id => new ImobilizadoModel
        {
            IdImobilizado = id,
            Nome = $"IMOB_{id}",
            Denominacao = "IMPORTADO",
            Nserie = "IMPORTADO",
            Quantidade = 1,
            DataAquisicao = DateTime.UtcNow,
            DataIncorporacao = DateTime.UtcNow,
            Moeda = "BRL"
        });

        await context.PM_Imobilizados.AddRangeAsync(novos);
        await context.SaveChangesAsync();
    }
}
