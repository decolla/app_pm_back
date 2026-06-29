using app_pm_back.Data;
using app_pm_back.Models.Domain;
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
    /// Inicializa uma nova instância da classe <see cref="BaseCrudService{T}"/>.
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
    /// Cria uma nova entidade.
    /// </summary>
    /// <param name="entity">A entidade a ser criada.</param>
    /// <returns>A entidade criada.</returns>
    public async Task<T> CreateAsync(T entity)
    {
        _dbSet.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// Cria várias entidades em massa.
    /// </summary>
    /// <param name="entities">A lista de entidades.</param>
    public async Task CreateRangeAsync(List<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Atualiza uma entidade existente.
    /// </summary>
    /// <param name="entity">A entidade a ser atualizada.</param>
    /// <returns>A entidade atualizada.</returns>
    public async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// Retorna todas as entidades cadastradas.
    /// </summary>
    /// <returns>Lista de entidades.</returns>
    public async Task<List<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <summary>
    /// Retorna as entidades paginadas.
    /// </summary>
    /// <param name="page">Número da página.</param>
    /// <param name="size">Tamanho da página.</param>
    /// <returns>Objeto contendo o total de registros e a lista paginada.</returns>
    public async Task<object> GetPagedAsync(int page, int size)
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
            .Skip((page - 1) * size) 
            .Take(size) 
            .ToListAsync(); 
        
        return new { Total = count, Result = result }; 
    }

    /// <summary>
    /// Busca uma entidade pelo ID.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <returns>A entidade correspondente ou null se não encontrada.</returns>
    public async Task<T?> GetByIdAsync(long id)
    {
        var keyValue = BuildPrimaryKeyValue(id);
        return await _dbSet.FindAsync(keyValue);
    }

    /// <summary>
    /// Deleta uma entidade pelo ID.
    /// </summary>
    /// <param name="id">Identificador da entidade.</param>
    /// <returns>True se deletada com sucesso, False caso contrário.</returns>
    public async Task<bool> DeleteAsync(long id)
    {
        var keyValue = BuildPrimaryKeyValue(id);
        var entity = await _dbSet.FindAsync(keyValue);
        if (entity == null) return false;

        _dbSet.Remove(entity);
        var changes = await _context.SaveChangesAsync();
        return changes > 0;
    }

    /// <summary>
    /// Retorna a contagem total de entidades.
    /// </summary>
    /// <returns>Número total de entidades.</returns>
    public async Task<long> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    /// <summary>
    /// Cria notas em massa, somente após lido verifica tudo de uma vez no banco
    /// </summary>
    /// <param name="notas"></param>
    [Obsolete("Use CreateRangeAsync instead.")]
    public async Task CreateNotasEmMassaAsync(List<T> notas)
    {
        await CreateRangeAsync(notas);
    }

    /// <summary>
    /// Atualiza uma nota
    /// </summary>
    /// <param name="nota"></param>
    /// <returns></returns>
    [Obsolete("Use UpdateAsync instead.")]
    public async Task<T> UpdateNotaAsync(T nota)
    {
        return await UpdateAsync(nota);
    }

    /// <summary>
    /// Retorna todas as notas cadastradas
    /// </summary>
    /// <returns></returns>
    [Obsolete("Use GetAllAsync instead.")]
    public async Task<List<T>> GetAllNotasAsync()
    {
        return await GetAllAsync();
    }

    /// <summary>
    /// Retorna as notas paginadas
    /// </summary>
    /// <param name="pagina"></param>
    /// <param name="tamanho"></param>
    /// <returns></returns>
    [Obsolete("Use GetPagedAsync instead.")]
    public async Task<object> GetNotasPaginadasAsync(int pagina, int tamanho)
    {
        return await GetPagedAsync(pagina, tamanho);
    }
    

    /// <summary>
    /// Deleta uma nota pelo ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Obsolete("Use DeleteAsync instead.")]
    public async Task<bool> DeleteNotaAsync(long id)
    {
        return await DeleteAsync(id);
    }

    /// <summary>
    /// Retorna a contagem total de notas
    /// </summary>
    /// <returns></returns>
    [Obsolete("Use CountAsync instead.")]
    public async Task<long> CountNotasAsync()
    {
        return await CountAsync();
    }
    
    /// <summary>
    /// Verifica se o usuário tem acesso a uma tabela específica
    /// </summary>
    /// <param name="usuario">Usuário a ser verificado</param>
    /// <param name="tabelaRequerida">Tabela a ser verificada</param>
    /// <returns>Boolean de se o usuário tem ou não acesso à tabela</returns>
    public bool UsuarioTemAcesso(UsuarioModel usuario, PermissoesTabela tabelaRequerida)
    {
        // se for ADM, tem passe livre em tudo
        if (usuario.NivelPermissao == NivelPermissao.Adm)
            return true;
        
        return usuario.Permissoes.HasValue && (usuario.Permissoes.Value & tabelaRequerida) == tabelaRequerida;
    }
}