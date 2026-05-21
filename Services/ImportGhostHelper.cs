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
    /// Verifica se os usuarios SAP existem, e caso contrario cria registros "fantasma" com nome e senha padrao.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="ids">Ids a serem criados fantasmas</param>
    public static async Task EnsureUsuariosSapAsync(AppDbContext context, IEnumerable<int?> ids)
    {
        var idsList = ids.Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
        if (!idsList.Any()) return;

        var existentes = await context.PM_UsuariosSap
            .Where(u => idsList.Contains(u.IdUsuario))
            .Select(u => u.IdUsuario)
            .ToListAsync();

        var faltantes = idsList.Except(existentes).ToList();
        if (!faltantes.Any()) return;

        var novos = faltantes.Select(id => new UsuarioSapModel
        {
            IdUsuario = id,
            Nome = $"IMPORTADO_{id}",
            Senha = "IMPORTADO",
            Mandante = "IMPORT"
        });

        await context.PM_UsuariosSap.AddRangeAsync(novos);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Cria equipamentos "fantasmas" caso não existam
    /// </summary>
    /// <param name="context"></param>
    /// <param name="tags"></param>
    public static async Task EnsureEquipamentosAsync(AppDbContext context, IEnumerable<string?> tags)
    {
        var tagsList = tags.Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag!.Trim())
            .Distinct()
            .ToList();

        if (!tagsList.Any()) return;

        var existentes = await context.PM_EquipamentosAtivos
            .Where(e => tagsList.Contains(e.TagEquipamento))
            .Select(e => e.TagEquipamento)
            .ToListAsync();

        var faltantes = tagsList.Except(existentes).ToList();
        if (!faltantes.Any()) return;

        var novos = faltantes.Select(tag => new EquipamentoAtivoModel
        {
            TagEquipamento = tag
        });

        await context.PM_EquipamentosAtivos.AddRangeAsync(novos);
        await context.SaveChangesAsync();
    }

    public static async Task EnsurePlanosAsync(AppDbContext context, IEnumerable<long?> codigos)
    {
        var codigosList = codigos.Where(c => c.HasValue).Select(c => c!.Value).Distinct().ToList();
        if (!codigosList.Any()) return;

        var existentes = await context.PM_PlanosManutencao
            .Where(p => codigosList.Contains(p.CodigoPlano))
            .Select(p => p.CodigoPlano)
            .ToListAsync();

        var faltantes = codigosList.Except(existentes).ToList();
        if (!faltantes.Any()) return;

        var novos = faltantes.Select(codigo => new PlanoManutencaoModel
        {
            CodigoPlano = codigo,
            NomePlano = $"PLANO_{codigo}",
            NomeAtivo = $"ATIVO_{codigo}"
        });

        await context.PM_PlanosManutencao.AddRangeAsync(novos);
        await context.SaveChangesAsync();
    }

    public static async Task EnsureNotasAsync(AppDbContext context, IEnumerable<long?> ids)
    {
        var idsList = ids.Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
        if (!idsList.Any()) return;

        var existentes = await context.PM_NotasManutencao
            .Where(n => idsList.Contains(n.IdNota))
            .Select(n => n.IdNota)
            .ToListAsync();

        var faltantes = idsList.Except(existentes).ToList();
        if (!faltantes.Any()) return;

        var novos = faltantes.Select(id => new NotaManutencaoModel
        {
            IdNota = id,
            NumeroNota = id,
            DataNota = DateTime.UtcNow
        });

        await context.PM_NotasManutencao.AddRangeAsync(novos);
        await context.SaveChangesAsync();
    }

    public static async Task EnsureGpmAsync(AppDbContext context, IEnumerable<int?> ids)
    {
        var idsList = ids.Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
        if (!idsList.Any()) return;

        var existentes = await context.PM_GruposPlanejamentoManutencao
            .Where(g => idsList.Contains(g.IdGpm))
            .Select(g => g.IdGpm)
            .ToListAsync();

        var faltantes = idsList.Except(existentes).ToList();
        if (!faltantes.Any()) return;

        var novos = faltantes.Select(id => new GrupoPlanejamentoManutencaoModel
        {
            IdGpm = id,
            Nome = $"GPM_{id}"
        });

        await context.PM_GruposPlanejamentoManutencao.AddRangeAsync(novos);
        await context.SaveChangesAsync();
    }

    public static async Task EnsureFabricantesAsync(AppDbContext context, IEnumerable<int?> ids)
    {
        var idsList = ids.Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
        if (!idsList.Any()) return;

        var existentes = await context.PM_Fabricantes
            .Where(f => idsList.Contains(f.IdFabricante))
            .Select(f => f.IdFabricante)
            .ToListAsync();

        var faltantes = idsList.Except(existentes).ToList();
        if (!faltantes.Any()) return;

        var novos = faltantes.Select(id => new FabricanteModel
        {
            IdFabricante = id,
            Nome = $"FAB_{id}"
        });

        await context.PM_Fabricantes.AddRangeAsync(novos);
        await context.SaveChangesAsync();
    }

    public static async Task EnsureLocalizacoesAsync(AppDbContext context, IEnumerable<int?> ids)
    {
        var idsList = ids.Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
        if (!idsList.Any()) return;

        var existentes = await context.PM_Localizacoes
            .Where(l => idsList.Contains(l.IdLocalizacao))
            .Select(l => l.IdLocalizacao)
            .ToListAsync();

        var faltantes = idsList.Except(existentes).ToList();
        if (!faltantes.Any()) return;

        var novos = faltantes.Select(id => new LocalizacaoModel
        {
            IdLocalizacao = id,
            Nome = $"LOCAL_{id}"
        });

        await context.PM_Localizacoes.AddRangeAsync(novos);
        await context.SaveChangesAsync();
    }

    public static async Task EnsureEnderecosAsync(AppDbContext context, IEnumerable<int?> ids)
    {
        var idsList = ids.Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
        if (!idsList.Any()) return;

        var existentes = await context.PM_Enderecos
            .Where(e => idsList.Contains(e.IdEndereco))
            .Select(e => e.IdEndereco)
            .ToListAsync();

        var faltantes = idsList.Except(existentes).ToList();
        if (!faltantes.Any()) return;

        var novos = faltantes.Select(id => new EnderecoModel
        {
            IdEndereco = id,
            Rua = $"RUA_{id}"
        });

        await context.PM_Enderecos.AddRangeAsync(novos);
        await context.SaveChangesAsync();
    }

    public static async Task EnsureImobilizadosAsync(AppDbContext context, IEnumerable<string?> tags)
    {
        var tagsList = tags.Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag!.Trim())
            .Distinct()
            .ToList();

        if (!tagsList.Any()) return;

        var existentes = await context.PM_Imobilizados
            .Where(i => tagsList.Contains(i.TagImobilizado))
            .Select(i => i.TagImobilizado)
            .ToListAsync();

        var faltantes = tagsList.Except(existentes).ToList();
        if (!faltantes.Any()) return;

        var novos = faltantes.Select(tag => new ImobilizadoModel
        {
            TagImobilizado = tag
        });

        await context.PM_Imobilizados.AddRangeAsync(novos);
        await context.SaveChangesAsync();
    }

    public static async Task EnsureCentrosTrabalhoAsync(AppDbContext context, IEnumerable<int?> ids)
    {
        var idsList = ids.Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
        if (!idsList.Any()) return;

        var existentes = await context.PM_CentrosTrabalho
            .Where(c => idsList.Contains(c.IdCentroTrabalho))
            .Select(c => c.IdCentroTrabalho)
            .ToListAsync();

        var faltantes = idsList.Except(existentes).ToList();
        if (!faltantes.Any()) return;

        var novos = faltantes.Select(id => new CentroTrabalhoModel
        {
            IdCentroTrabalho = id
        });

        await context.PM_CentrosTrabalho.AddRangeAsync(novos);
        await context.SaveChangesAsync();
    }

    public static async Task EnsureLocaisInstalacaoAsync(AppDbContext context, IEnumerable<int?> codigos)
    {
        var codigosList = codigos.Where(c => c.HasValue).Select(c => c!.Value).Distinct().ToList();
        if (!codigosList.Any()) return;

        var existentes = await context.PM_LocaisInstalacao
            .Where(l => codigosList.Contains(l.CodigoLocalInstalacao))
            .Select(l => l.CodigoLocalInstalacao)
            .ToListAsync();

        var faltantes = codigosList.Except(existentes).ToList();
        if (!faltantes.Any()) return;

        var novos = faltantes.Select(codigo => new LocalInstalacaoModel
        {
            CodigoLocalInstalacao = codigo,
            Denominacao = $"LOCAL_{codigo}"
        });

        await context.PM_LocaisInstalacao.AddRangeAsync(novos);
        await context.SaveChangesAsync();
    }
}

