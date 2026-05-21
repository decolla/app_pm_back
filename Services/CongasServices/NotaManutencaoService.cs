using app_pm_back.Data;
using app_pm_back.Models.CongasModel;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

public class NotaManutencaoService : BaseCrudService<NotaManutencaoModel>
{
    public NotaManutencaoService(AppDbContext context) : base(context)
    {
    }
    
    /// <summary>
    /// Importa notas em lote, criando registros fantasma para referencias ausentes.
    /// </summary>
    /// <param name="notas">Notas a serem cadastradas</param>
    public async Task ImportarNotasEmLoteSeguroAsync(List<NotaManutencaoModel> notas)
    {
        if (notas == null || !notas.Any()) return;

        var tagsEquipamentosExcel = notas
            .Where(n => !string.IsNullOrWhiteSpace(n.EquipamentoTag))
            .Select(n => n.EquipamentoTag!)
            .Distinct()
            .ToList();

        var usuariosCriadoresExcel = notas
            .Where(n => n.CriadoPorUsuarioId.HasValue)
            .Select(n => n.CriadoPorUsuarioId!.Value)
            .Distinct()
            .ToList();

        var planosExcel = notas
            .Where(n => n.PlanoManutencaoCodigo.HasValue)
            .Select(n => n.PlanoManutencaoCodigo!.Value)
            .Distinct()
            .ToList();

        await ImportGhostHelper.EnsureEquipamentosAsync(_context, tagsEquipamentosExcel);
        await ImportGhostHelper.EnsureUsuariosSapAsync(_context, usuariosCriadoresExcel.Select(id => (int?)id));
        await ImportGhostHelper.EnsurePlanosAsync(_context, planosExcel.Select(id => (long?)id));

        var idsNotas = notas.Select(n => n.IdNota).Distinct().ToList();
        var numerosNotas = notas.Select(n => n.NumeroNota).Distinct().ToList();

        var existentesPorId = await _context.PM_NotasManutencao
            .Where(n => idsNotas.Contains(n.IdNota))
            .ToDictionaryAsync(n => n.IdNota);

        var existentesPorNumero = await _context.PM_NotasManutencao
            .Where(n => numerosNotas.Contains(n.NumeroNota))
            .ToDictionaryAsync(n => n.NumeroNota);

        var novasNotas = new List<NotaManutencaoModel>();
        foreach (var nota in notas)
        {
            if (existentesPorId.TryGetValue(nota.IdNota, out var existente) ||
                existentesPorNumero.TryGetValue(nota.NumeroNota, out existente))
            {
                ImportMergeHelper.MergeNonEmpty(existente, nota);
            }
            else
            {
                novasNotas.Add(nota);
            }
        }

        if (novasNotas.Any())
        {
            await _dbSet.AddRangeAsync(novasNotas);
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Regra específica para a API, busca fichas por ativo e tipo de nota
    /// </summary>
    /// <param name="ativo">Nome do ativo a ser buscado como referência</param>
    /// <param name="tipoNota">Tipo da nota (corretiva ou preventiva)</param>
    /// <returns></returns>
    public async Task<object> GetFichasPorAtivoAsync(int ativo, string? tipoNota)
    {
        var query = _dbSet.AsQueryable(); 
        
        query = query.Where(n => n.LocalInstalacaoCodigo == ativo);
        
        if (!string.IsNullOrWhiteSpace(tipoNota))
        {
            query = query.Where(n => n.TipoNota == tipoNota);
        }
        
        var fichas = await query.Select(n => new
        {
            NumerFicha = n.IdNota,
            Data = n.DataNota,
            Tipo = n.TipoNota
        }).ToListAsync();
        
        return new
        {
            Ativo = ativo,
            Quantidade = fichas.Count,
            Fichas = fichas
        };
    }
}