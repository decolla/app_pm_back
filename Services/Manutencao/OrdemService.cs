using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe OrdemService.
/// </summary>
public class OrdemService : BaseCrudService<OrdemModel>
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="OrdemService"/>.
    /// </summary>
    public OrdemService(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="ordens">Parâmetro do método.</param>
    public async Task ImportarOrdensEmLoteSeguroAsync(List<OrdemModel> ordens, bool sobrescrever, bool adicionarNovas)
    {
        if (ordens == null || !ordens.Any()) return;

        var notaIds = ordens
            .Where(o => o.IdNota.HasValue)
            .Select(o => o.IdNota!.Value)
            .Distinct()
            .ToList();

        await ImportGhostHelper.EnsureNotasAsync(_context, notaIds.Select(id => (long?)id));

        var numerosOrdens = ordens
            .Where(o => o.NumeroOrdem.HasValue)
            .Select(o => o.NumeroOrdem!.Value)
            .Distinct()
            .ToList();

        var idsOrdens = ordens
            .Where(o => !o.NumeroOrdem.HasValue)
            .Select(o => o.IdOrdem)
            .Distinct()
            .ToList();

        var existentesPorNumero = numerosOrdens.Any()
            ? await _context.PM_Ordens
                .Where(o => o.NumeroOrdem.HasValue && numerosOrdens.Contains(o.NumeroOrdem.Value))
                .ToDictionaryAsync(o => o.NumeroOrdem!.Value)
            : new Dictionary<long, OrdemModel>();

        var existentesPorId = idsOrdens.Any()
            ? await _context.PM_Ordens
                .Where(o => idsOrdens.Contains(o.IdOrdem))
                .ToDictionaryAsync(o => o.IdOrdem)
            : new Dictionary<long, OrdemModel>();

        var novasOrdens = new List<OrdemModel>();
        foreach (var ordem in ordens)
        {
            OrdemModel? existente = null;

            if (ordem.NumeroOrdem.HasValue)
            {
                existentesPorNumero.TryGetValue(ordem.NumeroOrdem.Value, out existente);
            }
            else
            {
                existentesPorId.TryGetValue(ordem.IdOrdem, out existente);
            }

            if (existente != null)
            {
                if (sobrescrever)
                {
                    _context.Entry(existente).CurrentValues.SetValues(ordem);
                }
            }
            else if (adicionarNovas)
            {
                novasOrdens.Add(ordem);
            }
        }

        if (novasOrdens.Any())
        {
            await _context.PM_Ordens.AddRangeAsync(novasOrdens);
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Obtém um registro específico.
    /// </summary>
    /// <param name="ativo">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    public async Task<object> GetFichasPorAtivoAsync(ComponentType ativo, string? tipoOrdem)
    {
        var query = _dbSet.AsQueryable();

        query = query.Where(n => n.Nota != null && n.Nota.IdAtivo == ativo);

        var fichas = await query.Select(n => new
        {
            NumerFicha = n.IdNota,
            Data = n.FimReal ?? n.FimPlanejado ?? DateTime.UtcNow,
            Tipo = n.Classificacao
        }).ToListAsync();

        return new
        {
            Ativo = ativo,
            Quantidade = fichas.Count,
            Fichas = fichas
        };
    }
}