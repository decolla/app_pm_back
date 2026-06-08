using app_pm_back.Data;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services;

// O <T> significa que essa classe aceita qualquer tipo de entidade (NotaModel, OrdemModel, etc)
/// <summary>
/// Classe BaseCrudService.
/// </summary>
public abstract class BaseCrudService<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="BaseCrudService"/>.
    /// </summary>

    public BaseCrudService(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>(); // descobre automaticamente qual é a tabela
    }

    private object BuildPrimaryKeyValue(long id)
    {
        var entityType = _context.Model.FindEntityType(typeof(T));
        var primaryKey = entityType?.FindPrimaryKey();

        if (primaryKey == null || primaryKey.Properties.Count != 1)
            throw new InvalidOperationException($"A entidade {typeof(T).Name} deve possuir chave primária simples para operações por id.");

        var keyType = primaryKey.Properties[0].ClrType;
        return Convert.ChangeType(id, keyType);
    }

    /// <summary>
    /// Cria uma nota 
    /// </summary>
    /// <param name="nota"></param>
    /// <returns></returns>
    public async Task<T> CreateAsync(T nota)
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
    public async Task<Object> GetNotasPaginadasAsync(int pagina, int tamanho)
    {
        var entityType = _context.Model.FindEntityType(typeof(T));
        var primaryKey = entityType?.FindPrimaryKey();
        if (primaryKey == null || primaryKey.Properties.Count != 1)
            throw new InvalidOperationException(
                $"A entidade {typeof(T).Name} deve possuir chave primária simples para paginação.");

        var keyName = primaryKey.Properties[0].Name;

        int count = await _dbSet.CountAsync(); 
        
        var result = await _dbSet 
            .OrderBy(e => EF.Property<object>(e, keyName)) 
            .Skip((pagina - 1) * tamanho) 
            .Take(tamanho) 
            .ToListAsync(); 
        
        return new { Total = count, Result = result }; 
    }

    /// <summary>
    /// Busca uma nota pelo ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<T?> GetNotaByIdAsync(long id)
    {
        var keyValue = BuildPrimaryKeyValue(id);
        return await _dbSet.FindAsync(keyValue);
    }

    /// <summary>
    /// Deleta uma nota pelo ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> DeleteNotaAsync(long id)
    {
        var keyValue = BuildPrimaryKeyValue(id);
        var nota = await _dbSet.FindAsync(keyValue);
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