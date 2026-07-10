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
                    //Iguala o ID da planilha ao ID do banco.
                    nota.IdNota = existente.IdNota; 
                
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
    
    /// <summary>
    /// Atualiza uma nota por completo com base no DTO da tela do frontend.
    /// </summary>
    /// <param name="notaTela">Nota completa</param>
    /// <returns>Nota atualizada</returns>
    public async Task<NotaModel> AtualizarNotaCompletaAsync(NotaModel notaTela)
    {
        // limpa o change tracker
        _context.ChangeTracker.Clear();

        // busca a nota no banco de dados
        var notaDoBanco = await _dbSet
            .Include(n => n.Itens)
            .Include(n => n.Variaveis)
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.IdNota == notaTela.IdNota);

        if (notaDoBanco == null)
            throw new Exception("Nota não encontrada.");

        // atualiza o cabeçalho da nota
        _context.Entry(notaDoBanco).CurrentValues.SetValues(notaTela);
        _context.Entry(notaDoBanco).State = EntityState.Modified;
        
        // sincroniza itens 
        var idsItens = notaTela.Itens.Select(i => i.IdItemNota).ToList();
        
        // deleta
        foreach (var itemBanco in notaDoBanco.Itens.Where(i => !idsItens.Contains(i.IdItemNota)))
        {
            _context.Entry(itemBanco).State = EntityState.Deleted;
        }
        
        // add/ update processando o que vier do frontend
        foreach (var itemTela in notaTela.Itens)
        {
            if (itemTela.IdItemNota == 0)
            {
                // caso de novo item
                itemTela.IdNota = notaDoBanco.IdNota;
                _context.Entry(itemTela).State = EntityState.Added;
            }
            else
            {
                // caso de item update
                _context.Entry(itemTela).State = EntityState.Modified;
            }
        }

        // sincroniza variáveis
        var idsVariaveis = notaTela.Variaveis.Select(v => v.IdVariavel).ToList();
    
        // deleta
        foreach (var varBanco in notaDoBanco.Variaveis.Where(v => !idsVariaveis.Contains(v.IdVariavel)))
        {
            _context.Entry(varBanco).State = EntityState.Deleted;
        }

        foreach (var varTela in notaTela.Variaveis)
        {
            if (varTela.IdVariavel == 0)
            {
                // caso de nova variável
                varTela.IdNota = notaDoBanco.IdNota;
                _context.Entry(varTela).State = EntityState.Added;
            }
            else
            {
                // caso de update
                _context.Entry(varTela).State = EntityState.Modified;
            }
        }

        // salva bb
        await _context.SaveChangesAsync();

        return notaDoBanco;
    }
}