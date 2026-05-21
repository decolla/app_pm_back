using app_pm_back.Data;
using app_pm_back.Models;
using app_pm_back.Models.CongasModel;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

public class OrderCongasService : BaseCrudService<OrdemManutencaoModel>
{
    public OrderCongasService(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Importa ordens em lote, criando registros fantasma para referencias ausentes.
    /// </summary>
    public async Task ImportarOrdensEmLoteSeguroAsync(List<OrdemManutencaoModel> ordens)
    {
        if (ordens == null || !ordens.Any()) return;

        var notaIds = ordens
            .Where(o => o.NotaRelacionadaId.HasValue)
            .Select(o => o.NotaRelacionadaId!.Value)
            .Distinct()
            .ToList();

        var planosIds = ordens
            .Where(o => o.PlanoManutencaoCodigo.HasValue)
            .Select(o => o.PlanoManutencaoCodigo!.Value)
            .Distinct()
            .ToList();

        var equipamentosTags = ordens
            .Where(o => !string.IsNullOrWhiteSpace(o.EquipamentoTag))
            .Select(o => o.EquipamentoTag!)
            .Distinct()
            .ToList();

        var usuariosIds = ordens
            .SelectMany(o => new[] { o.CriadaPorUsuarioId, o.ModificadaPorUsuarioId })
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        var gpmIds = ordens
            .Where(o => o.GpmResponsavelId.HasValue)
            .Select(o => o.GpmResponsavelId!.Value)
            .Distinct()
            .ToList();

        await ImportGhostHelper.EnsureNotasAsync(_context, notaIds.Select(id => (long?)id));
        await ImportGhostHelper.EnsurePlanosAsync(_context, planosIds.Select(id => (long?)id));
        await ImportGhostHelper.EnsureEquipamentosAsync(_context, equipamentosTags);
        await ImportGhostHelper.EnsureUsuariosSapAsync(_context, usuariosIds.Select(id => (int?)id));
        await ImportGhostHelper.EnsureGpmAsync(_context, gpmIds.Select(id => (int?)id));

        var idsOrdens = ordens.Select(o => o.IdOrdem).Distinct().ToList();
        var existentes = await _context.PM_OrdensManutencao
            .Where(o => idsOrdens.Contains(o.IdOrdem))
            .ToDictionaryAsync(o => o.IdOrdem);

        var novasOrdens = new List<OrdemManutencaoModel>();
        foreach (var ordem in ordens)
        {
            if (existentes.TryGetValue(ordem.IdOrdem, out var existente))
            {
                ImportMergeHelper.MergeNonEmpty(existente, ordem);
            }
            else
            {
                novasOrdens.Add(ordem);
            }
        }

        if (novasOrdens.Any())
        {
            await _context.PM_OrdensManutencao.AddRangeAsync(novasOrdens);
        }

        await _context.SaveChangesAsync();
    }
    
    /// <summary>
    /// Regra específica para a API, busca fichas por ativo e tipo de nota.
    /// </summary>
    /// <param name="ativo"></param>
    /// <param name="tipoOrdem"></param>
    /// <returns></returns>
    public async Task<object> GetFichasPorAtivoAsync(int ativo, string? tipoOrdem)
    {
        var query = _dbSet.AsQueryable(); // usa o _dbSet da Superclasse
        
        query = query.Where(n => 
            n.NotaRelacionada.LocalInstalacaoCodigo == ativo);
        
        if (!string.IsNullOrWhiteSpace(tipoOrdem))
        {
            query = query.Where(n => n.TipoOrdem == tipoOrdem);
        }
        
        var fichas = await query.Select(n => new
        {
            NumerFicha = n.NotaRelacionadaId,
            Data = n.DataEntrada,
            Tipo = n.TipoOrdem
        }).ToListAsync();
        
        return new
        {
            Ativo = ativo,
            Quantidade = fichas.Count,
            Fichas = fichas
        };
    }
}