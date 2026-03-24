using app_pm_back.Data;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services;

// O <T> significa que essa classe aceita qualquer tipo (NotaPetroModel, NoteCongasModel, etc)
public abstract class BaseCrudService<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public BaseCrudService(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>(); // descobre automaticamente qual é a tabela
    }

    /// <summary>
    /// Cria uma nota 
    /// </summary>
    /// <param name="nota"></param>
    /// <returns></returns>
    public async Task<T> CreateNotaAsync(T nota)
    {
        _dbSet.Add(nota);
        await _context.SaveChangesAsync();
        return nota;
    }

    /// <summary>
    /// Cria notas em massa, somente após lido verifica tudo de uma vez no banco
    /// </summary>
    /// <param name="notas"></param>
    public async Task CreateNotasEmMassaAsync(List<T> notas)
    {
        await _dbSet.AddRangeAsync(notas);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Atualiza uma nota
    /// </summary>
    /// <param name="nota"></param>
    /// <returns></returns>
    public async Task<T> UpdateNotaAsync(T nota)
    {
        _dbSet.Update(nota);
        await _context.SaveChangesAsync();
        return nota;
    }

    /// <summary>
    /// Retorna todas as notas cadastradas
    /// </summary>
    /// <returns></returns>
    public async Task<List<T>> GetAllNotasAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <summary>
    /// Retorna as notas paginadas
    /// </summary>
    /// <param name="pagina"></param>
    /// <param name="tamanho"></param>
    /// <returns></returns>
    public async Task<List<T>> GetNotasPaginadasAsync(int pagina, int tamanho)
    {
        // O EF.Property permite ordernar dinamicamente sabendo que ambas as tabelas têm a coluna "IdNota"
        return await _dbSet
            .OrderBy(e => EF.Property<int>(e, "IdNota"))
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToListAsync();
    }

    /// <summary>
    /// Busca uma nota pelo ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<T?> GetNotaByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <summary>
    /// Deleta uma nota pelo ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> DeleteNotaAsync(int id)
    {
        var nota = await _dbSet.FindAsync(id);
        if (nota == null) return false;

        _dbSet.Remove(nota);
        var alteracoes = await _context.SaveChangesAsync();
        return alteracoes > 0;
    }

    /// <summary>
    /// Retorna a contagem total de notas
    /// </summary>
    /// <returns></returns>
    public async Task<long> CountNotasAsync()
    {
        return await _dbSet.CountAsync();
    }
}