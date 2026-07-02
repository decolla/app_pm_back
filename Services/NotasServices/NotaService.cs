using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe NotaService.
/// </summary>
public class NotaService : BaseCrudService<NotaModel>
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="NotaService"/>.
    /// </summary>
    public NotaService(AppDbContext context) : base(context)
    {
    }
    
    /// <summary>
    /// Override necessário para incluir os relacionamentos que devem ser retornados ao front end.
    /// </summary>
    protected override IQueryable<NotaModel> AplicarIncludes(IQueryable<NotaModel> query)
    {
        return query
            .Include(n => n.Itens)
            .Include(n => n.Variaveis)
            .Include(n => n.Anexos)
            .Include(n => n.Ordens);
    }

    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="notas">Parâmetro do método.</param>
    public virtual async Task ImportarNotasEmLoteSeguroAsync(List<NotaModel> notas, bool sobrescrever,
        bool adicionarNovas, CancellationToken cancellationToken)
    {
        if (notas == null || !notas.Any()) return;

        var usuariosCriadoresExcel = notas
            .Where(n => n.IdUsuario.HasValue)
            .Select(n => n.IdUsuario!.Value)
            .Distinct()
            .ToList();

        var planosExcel = notas
            .Where(n => n.IdPlano.HasValue)
            .Select(n => n.IdPlano!.Value)
            .Distinct()
            .ToList();

        await ImportGhostHelper.EnsureUsuariosSapAsync(_context, usuariosCriadoresExcel.Select(id => (long?)id));
        await ImportGhostHelper.EnsurePlanosAsync(_context, planosExcel.Select(id => (long?)id));

        var numerosNotas = notas
            .Select(n => n.NumeroNota)
            .Distinct()
            .ToList();

        var existentesPorNumero = await _context.PM_Notas
            .Where(n => numerosNotas.Contains(n.NumeroNota))
            .ToDictionaryAsync(n => n.NumeroNota);

        var novasNotas = new List<NotaModel>();
        foreach (var nota in notas)
        {
            // lança uma excessão caso o cancelation token seja acionado
            cancellationToken.ThrowIfCancellationRequested();
            
            if (existentesPorNumero.TryGetValue(nota.NumeroNota, out var existente))
            {
                if (sobrescrever)
                {
                    _context.Entry(existente).CurrentValues.SetValues(nota);
                }
            }
            else if (adicionarNovas)
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
    /// Obtém um registro específico.
    /// </summary>
    /// <param name="ativo">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    public async Task<object> GetFichasPorAtivoAsync(ComponentType ativo, string? tipoNota)
    {
        var query = _dbSet.AsQueryable();

        query = query.Where(n => n.IdAtivo == ativo);

        var fichas = await query.Select(n => new
        {
            NumerFicha = n.IdNota,
            Data = n.DataNota,
            Tipo = "MG" // default or mock value
        }).ToListAsync();

        return new
        {
            Ativo = ativo,
            Quantidade = fichas.Count,
            Fichas = fichas
        };
    }
}