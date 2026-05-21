using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace app_pm_back.Services;

public class GenericSearchService
{
    private readonly AppDbContext _context;
    private readonly DbSet<NotaManutencaoModel> _notaManutencaoModels;
    private readonly DbSet<OrdemManutencaoModel> _ordemManutencaoModels;
    private readonly DbSet<PlanoManutencaoModel> _planosManutencaoModels;
    private static readonly Dictionary<string, Type> Tabelas = BuildTabelas();

    public GenericSearchService(
        AppDbContext context)
    {
        _context = context;
        _notaManutencaoModels = context.Set<NotaManutencaoModel>();
        _ordemManutencaoModels = context.Set<OrdemManutencaoModel>();
        _planosManutencaoModels = context.Set<PlanoManutencaoModel>();
    }

    public Task<IReadOnlyList<object>> BuscarPorNomeAsync(
        string tabela,
        string campo,
        string nome,
        int? pagina,
        int? tamanho)
    {
        return BuscarPorCampoAsync(tabela, campo, nome, true, pagina, tamanho);
    }

    public IReadOnlyDictionary<string, IReadOnlyList<string>> ListarTabelasCampos()
    {
        return Tabelas.ToDictionary(
            kvp => kvp.Key,
            kvp => (IReadOnlyList<string>)kvp.Value
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => TipoSuportado(ObterTipoBase(p.PropertyType)))
                .Select(p => p.Name)
                .OrderBy(p => p)
                .ToList(),
            StringComparer.OrdinalIgnoreCase);
    }

    public async Task<IReadOnlyList<object>> BuscarPorCampoAsync(
        string tabela,
        string campo,
        string valor,
        bool parcial,
        int? pagina,
        int? tamanho)
    {
        if (!Tabelas.TryGetValue(tabela, out var entityType))
        {
            throw new ArgumentException("Tabela informada nao existe no contexto.", nameof(tabela));
        }

        var metodo = typeof(GenericSearchService).GetMethod(
            nameof(BuscarInternoAsync),
            BindingFlags.Instance | BindingFlags.NonPublic);

        if (metodo == null)
        {
            throw new InvalidOperationException("Metodo de busca interna nao encontrado.");
        }

        var generico = metodo.MakeGenericMethod(entityType);
        var task = (Task<IReadOnlyList<object>>)generico.Invoke(
            this,
            new object[] { campo, valor, parcial, pagina, tamanho })!;

        return await task.ConfigureAwait(false);
    }

    private async Task<IReadOnlyList<object>> BuscarInternoAsync<T>(
        string campo,
        string valor,
        bool parcial,
        int? pagina,
        int? tamanho)
        where T : class
    {
        var property = BuscarPropriedade(typeof(T), campo);
        var propertyType = ObterTipoBase(property.PropertyType);
        if (!TipoSuportado(propertyType))
        {
            throw new ArgumentException("Campo informado nao permite busca.", nameof(campo));
        }

        var parametro = Expression.Parameter(typeof(T), "e");
        var acessoPropriedade = Expression.Property(parametro, property);

        Expression filtro;
        if (propertyType == typeof(string))
        {
            if (parcial)
            {
                var functions = Expression.Property(null, typeof(EF).GetProperty(nameof(EF.Functions))!);
                var pattern = Expression.Constant($"%{valor}%");
                filtro = Expression.Call(
                    typeof(NpgsqlDbFunctionsExtensions),
                    nameof(NpgsqlDbFunctionsExtensions.ILike),
                    Type.EmptyTypes,
                    functions,
                    acessoPropriedade,
                    pattern);
            }
            else
            {
                filtro = Expression.Equal(acessoPropriedade, Expression.Constant(valor));
            }
        }
        else
        {
            var convertido = ConverterValor(valor, propertyType);
            filtro = Expression.Equal(acessoPropriedade, Expression.Constant(convertido, property.PropertyType));
        }

        var lambda = Expression.Lambda<Func<T, bool>>(filtro, parametro);
        var query = _context.Set<T>().AsNoTracking().Where(lambda);
        query = AplicarOrdenacao(query);

        if (pagina.HasValue || tamanho.HasValue)
        {
            int p = pagina ?? 1;
            int t = tamanho ?? 20;
            query = query.Skip((p - 1) * t).Take(t);
        }

        return await query.Cast<object>().ToListAsync().ConfigureAwait(false);
    }

    private static PropertyInfo BuscarPropriedade(Type tipo, string campo)
    {
        var property = tipo
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(p => string.Equals(p.Name, campo, StringComparison.OrdinalIgnoreCase));

        if (property == null)
        {
            throw new ArgumentException("Campo informado nao existe na tabela.", nameof(campo));
        }

        return property;
    }

    private static bool TipoSuportado(Type tipo)
    {
        return tipo.IsPrimitive
               || tipo.IsEnum
               || tipo == typeof(string)
               || tipo == typeof(decimal)
               || tipo == typeof(DateTime)
               || tipo == typeof(DateTimeOffset)
               || tipo == typeof(Guid)
               || tipo == typeof(TimeSpan);
    }

    private static Type ObterTipoBase(Type tipo)
    {
        return Nullable.GetUnderlyingType(tipo) ?? tipo;
    }

    private static object ConverterValor(string valor, Type tipo)
    {
        if (tipo.IsEnum)
        {
            return Enum.Parse(tipo, valor, true);
        }

        if (tipo == typeof(Guid))
        {
            return Guid.Parse(valor);
        }

        if (tipo == typeof(TimeSpan))
        {
            return TimeSpan.Parse(valor, CultureInfo.InvariantCulture);
        }

        return Convert.ChangeType(valor, tipo, CultureInfo.InvariantCulture);
    }

    private IQueryable<T> AplicarOrdenacao<T>(IQueryable<T> query) where T : class
    {
        var entityType = _context.Model.FindEntityType(typeof(T));
        var primaryKey = entityType?.FindPrimaryKey();
        if (primaryKey == null || primaryKey.Properties.Count != 1)
        {
            return query;
        }

        var keyName = primaryKey.Properties[0].Name;
        var parametro = Expression.Parameter(typeof(T), "e");
        var keyExpr = Expression.Call(
            typeof(EF),
            nameof(EF.Property),
            new[] { typeof(object) },
            parametro,
            Expression.Constant(keyName));
        var lambda = Expression.Lambda<Func<T, object>>(keyExpr, parametro);
        return query.OrderBy(lambda);
    }

    private static Dictionary<string, Type> BuildTabelas()
    {
        return typeof(AppDbContext)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsGenericType
                        && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .ToDictionary(
                p => p.Name,
                p => p.PropertyType.GetGenericArguments()[0],
                StringComparer.OrdinalIgnoreCase);
    }
    
    /// <summary>
    /// Busca por uma última nota/ordem associada a um ativo, e o plano de manutenção associado.
    /// </summary>
    /// <param name="ativo">Ativo a ser buscado</param>
    /// <returns></returns>
    public async Task<object?> AtivoGerald(int ativo)
    {
        // seleciona a última ordem associada ao ativo
        OrdemManutencaoModel? ultimaOrdem = await _ordemManutencaoModels
            .Where(o => o.LocalInstalacaoCodigo == ativo)
            .OrderByDescending(o => o.DataCriacao)
            .FirstOrDefaultAsync();
        
        // seleciona a última ordem associada ao ativo
        NotaManutencaoModel? ultimaNota = await _notaManutencaoModels
            .Where(o => o.LocalInstalacaoCodigo == ativo)
            .OrderByDescending(o => o.DataAvaria)
            .FirstOrDefaultAsync();
        
        if (ultimaOrdem == null && ultimaNota == null)
            return null;
        
        DateTime? ultimaData = null;
        long? codigoPlanoParaBuscar = null;
        
        if (ultimaOrdem != null && ultimaNota != null)
        {
            if (ultimaOrdem.DataCriacao > ultimaNota.DataNota)
            {
                ultimaData = ultimaOrdem.DataCriacao;
                codigoPlanoParaBuscar = ultimaOrdem.PlanoManutencaoCodigo;
            }
            else
            {
                ultimaData = ultimaNota.DataNota;
                codigoPlanoParaBuscar = ultimaNota.PlanoManutencaoCodigo;
            }
        }else if (ultimaOrdem != null)
        {
            ultimaData = ultimaOrdem.DataCriacao;
            codigoPlanoParaBuscar = ultimaOrdem.PlanoManutencaoCodigo;
        }
        else if (ultimaNota != null)
        {
            ultimaData = ultimaNota.DataNota;
            codigoPlanoParaBuscar = ultimaNota.PlanoManutencaoCodigo; 
        }
        
        PlanoManutencaoModel? ultimoPlano = null;
        if (codigoPlanoParaBuscar.HasValue)
        {
            ultimoPlano = await _planosManutencaoModels
                .Include(p => p.Estrategia)
                .Include(p => p.Ciclos) // É bom incluir os ciclos para não retornar nulo no JSON
                .Where(p => p.CodigoPlano == codigoPlanoParaBuscar.Value)
                .FirstOrDefaultAsync();
        }

        // pega a confiabilidade associada ao tipo de manutenção do plano, ou da estratégia caso o tipo seja nulo
        int confiabilidadeFinal = ultimoPlano?.TiposPlano?
            .Select(t => t.TipoManutencao ?? t.TipoEstrategia?.TipoManutencao)
            .FirstOrDefault(tm => tm != null)?
            .Confiabilidade ?? 0;
        
        return new
        {
            Ativo = ativo, // Id do ativo/LI que foi passado
            UltimaData = ultimaData, // Ùltima ocorrência em uma nota/ordem
            Estrategia = ultimoPlano?.Estrategia, // Estratégia associada ao plano
            TempoCiclo = ultimoPlano?.Estrategia.Ciclos, // Tempo do ciclo
            UnidadeCiclo = ultimoPlano?.Estrategia.UnidadeCiclo, // Unidade de tempo do ciclo (dias, meses...)
            DataInicio = ultimoPlano?.DataInicio, // Data de início do plano
            DataConclusao = ultimoPlano?.DataConclusao, // Data de conclusão do plano
            Confiabilidade = confiabilidadeFinal // Confiabilidade associada ao tipo de manutenção/estratégia
        };
    }
}
