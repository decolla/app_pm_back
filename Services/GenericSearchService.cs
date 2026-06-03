using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services;

/// <summary>
/// Classe para busca genérica entre as tabelas do banco de dados 
/// </summary>
public class GenericSearchService
{
    private readonly AppDbContext _context;
    private readonly DbSet<NotaModel> _notaModels;
    private readonly DbSet<OrdemModel> _ordemModels;
    private readonly DbSet<PlanoModel> _planoModels;
    private static readonly Dictionary<string, Type> Tabelas = BuildTabelas();
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="Notas">Parâmetro do método.</param>
    /// <param name="Ordens">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>

    public record NotaOrdemSearchResult(
        IReadOnlyList<object> Notas,
        IReadOnlyList<object> Ordens);

    public record TimelineItem(
        long Id,
        string? Classificacao,
        string? Descricao,
        DateTime? DataCriacao);

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="GenericSearchService"/>.
    /// </summary>

    public GenericSearchService(AppDbContext context)
    {
        _context = context;
        _notaModels = context.PM_Notas;
        _ordemModels = context.PM_Ordens;
        _planoModels = context.PM_Planos;
    }
    /// <summary>
    /// Obtém um registro específico.
    /// </summary>
    /// <param name="tabela">Parâmetro do método.</param>
    /// <param name="campo">Parâmetro do método.</param>
    /// <param name="nome">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    
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
    /// <summary>
    /// Obtém um registro específico.
    /// </summary>
    /// <param name="tabela">Parâmetro do método.</param>
    /// <param name="campo">Parâmetro do método.</param>
    /// <param name="valor">Parâmetro do método.</param>
    /// <param name="parcial">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>

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
    /// <summary>
    /// Obtém um registro específico.
    /// </summary>
    /// <param name="busca">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>

    public async Task<NotaOrdemSearchResult> BuscarNotaOrdemAsync(
        dynamic busca,
        int? pagina,
        int? tamanho)
    {
        if (busca == null)
        {
            throw new ArgumentNullException(nameof(busca));
        }

        var notasQuery = _notaModels.AsNoTracking().AsQueryable();
        var ordensQuery = _ordemModels.AsNoTracking().AsQueryable();

        // Safe check for properties of dynamic/anonymous object
        var buscaType = (Type)busca.GetType();

        string? statusNota = buscaType.GetProperty("StatusNota")?.GetValue(busca) as string;
        if (!string.IsNullOrWhiteSpace(statusNota))
        {
            notasQuery = notasQuery.Where(n => n.StatusNota == statusNota);
        }

        long? notaIdInicio = buscaType.GetProperty("NotaIdInicio")?.GetValue(busca) as long?;
        if (notaIdInicio.HasValue)
        {
            notasQuery = notasQuery.Where(n => n.NumeroNota >= notaIdInicio.Value);
        }

        long? notaIdFim = buscaType.GetProperty("NotaIdFim")?.GetValue(busca) as long?;
        if (notaIdFim.HasValue)
        {
            notasQuery = notasQuery.Where(n => n.NumeroNota <= notaIdFim.Value);
        }

        long? ordemIdInicio = buscaType.GetProperty("OrdemIdInicio")?.GetValue(busca) as long?;
        if (ordemIdInicio.HasValue)
        {
            ordensQuery = ordensQuery.Where(o => o.IdOrdem >= ordemIdInicio.Value);
        }

        long? ordemIdFim = buscaType.GetProperty("OrdemIdFim")?.GetValue(busca) as long?;
        if (ordemIdFim.HasValue)
        {
            ordensQuery = ordensQuery.Where(o => o.IdOrdem <= ordemIdFim.Value);
        }

        ComponentType? localInstalacaoInicio = buscaType.GetProperty("LocalInstalacaoInicio")?.GetValue(busca) as ComponentType?;
        if (localInstalacaoInicio.HasValue)
        {
            notasQuery = notasQuery.Where(n => n.IdAtivo >= localInstalacaoInicio.Value);
            ordensQuery = ordensQuery.Where(o => o.IdAtivo >= localInstalacaoInicio.Value);
        }

        ComponentType? localInstalacaoFim = buscaType.GetProperty("LocalInstalacaoFim")?.GetValue(busca) as ComponentType?;
        if (localInstalacaoFim.HasValue)
        {
            notasQuery = notasQuery.Where(n => n.IdAtivo <= localInstalacaoFim.Value);
            ordensQuery = ordensQuery.Where(o => o.IdAtivo <= localInstalacaoFim.Value);
        }

        DateTime? notaDataInicio = buscaType.GetProperty("NotaDataInicio")?.GetValue(busca) as DateTime?;
        if (notaDataInicio.HasValue)
        {
            notasQuery = notasQuery.Where(n => n.DataNota >= notaDataInicio.Value);
        }

        DateTime? notaDataFim = buscaType.GetProperty("NotaDataFim")?.GetValue(busca) as DateTime?;
        if (notaDataFim.HasValue)
        {
            notasQuery = notasQuery.Where(n => n.DataNota <= notaDataFim.Value);
        }

        DateTime? ordemDataInicio = buscaType.GetProperty("OrdemDataInicio")?.GetValue(busca) as DateTime?;
        if (ordemDataInicio.HasValue)
        {
            ordensQuery = ordensQuery.Where(o => o.DataCriacao >= ordemDataInicio.Value);
        }

        DateTime? ordemDataFim = buscaType.GetProperty("OrdemDataFim")?.GetValue(busca) as DateTime?;
        if (ordemDataFim.HasValue)
        {
            ordensQuery = ordensQuery.Where(o => o.DataCriacao <= ordemDataFim.Value);
        }

        string? prioridadeInicio = buscaType.GetProperty("PrioridadeInicio")?.GetValue(busca) as string;
        if (!string.IsNullOrWhiteSpace(prioridadeInicio) && int.TryParse(prioridadeInicio, out var priInicio))
        {
            var prioridadeEnum = (Prioridade)priInicio;
            notasQuery = notasQuery.Where(n => n.Prioridade >= prioridadeEnum);
        }

        string? prioridadeFim = buscaType.GetProperty("PrioridadeFim")?.GetValue(busca) as string;
        if (!string.IsNullOrWhiteSpace(prioridadeFim) && int.TryParse(prioridadeFim, out var priFim))
        {
            var prioridadeEnum = (Prioridade)priFim;
            notasQuery = notasQuery.Where(n => n.Prioridade <= prioridadeEnum);
        }

        string? statusOrdem = buscaType.GetProperty("StatusOrdem")?.GetValue(busca) as string;
        if (!string.IsNullOrWhiteSpace(statusOrdem))
        {
            ordensQuery = ordensQuery.Where(o => o.StatusOrdem == statusOrdem);
        }

        notasQuery = AplicarOrdenacao(notasQuery);
        ordensQuery = AplicarOrdenacao(ordensQuery);

        if (pagina.HasValue || tamanho.HasValue)
        {
            int p = pagina ?? 1;
            int t = tamanho ?? 20;
            notasQuery = notasQuery.Skip((p - 1) * t).Take(t);
            ordensQuery = ordensQuery.Skip((p - 1) * t).Take(t);
        }

        var notas = await notasQuery.Cast<object>().ToListAsync().ConfigureAwait(false);
        var ordens = await ordensQuery.Cast<object>().ToListAsync().ConfigureAwait(false);

        return new NotaOrdemSearchResult(notas, ordens);
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
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="ativo">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    
    public async Task<object?> AtivoGerald(ComponentType ativo)
    {
        OrdemModel? ultimaOrdem = await _ordemModels
            .Where(o => o.IdAtivo == ativo)
            .OrderByDescending(o => o.DataCriacao)
            .FirstOrDefaultAsync();
        
        NotaModel? ultimaNota = await _notaModels
            .Where(o => o.IdAtivo == ativo)
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
                codigoPlanoParaBuscar = ultimaOrdem.IdNota.HasValue ? (long?)ultimaOrdem.IdNota.Value : null; // search using NotaId
            }
            else
            {
                ultimaData = ultimaNota.DataNota;
                codigoPlanoParaBuscar = ultimaNota.IdPlano;
            }
        }
        else if (ultimaOrdem != null)
        {
            ultimaData = ultimaOrdem.DataCriacao;
            codigoPlanoParaBuscar = ultimaOrdem.IdNota.HasValue ? (long?)ultimaOrdem.IdNota.Value : null;
        }
        else if (ultimaNota != null)
        {
            ultimaData = ultimaNota.DataNota;
            codigoPlanoParaBuscar = ultimaNota.IdPlano; 
        }
        
        PlanoModel? ultimoPlano = null;
        if (codigoPlanoParaBuscar.HasValue)
        {
            ultimoPlano = await _planoModels
                .Include(p => p.Estrategia)
                .Where(p => p.IdPlano == codigoPlanoParaBuscar.Value)
                .FirstOrDefaultAsync();
        }
        // retornar tipos c do plano, invés das estatégias
        // ultima data puxar da tabela do lucas
        
        return new
        {
            Ativo = ativo,
            UltimaData = ultimaData,
            Estrategia = ultimoPlano?.Estrategia,
            TempoCiclo = ultimoPlano?.Estrategia?.Ciclos,
            UnidadeCiclo = "Dias",
            DataInicio = ultimoPlano?.DataInicios,
            DataConclusao = ultimoPlano?.DataConclusao,
            Confiabilidade = 100
        };
    }
    
    public async Task<object?> AtivoDatas(ComponentType ativo, string idLi, TipoDados tipoDados)
    {
        //Busca a data da última renovação daquele conjunto LI + ativo
        DadosRenovacaoModel? ultimaRenovacao = await _context.PM_DadosRenovacao
            .Where(d => d.IdAtivo == ativo && d.IdLi == idLi && d.TipoDados == tipoDados)
            .OrderByDescending(d => d.UltimaRenovacao)
            .FirstOrDefaultAsync();

        //Busca o plano relacionado ao conjunto LI + ativo
        PlanoModel? plano = await _planoModels
            .Where(p => p.IdAtivo == ativo && p.IdLi == idLi)
            .FirstOrDefaultAsync();
        
        //Se plano não existe, apenas retorna data
        if (plano == null)
        {
            return new
            {
                UltimaData = ultimaRenovacao?.UltimaRenovacao,
                ListaRenovacao = new List<object>(),
                TempoCiclo = 120
            };
        }

        // Buscar todos os TipoPlano relacionados ao Plano com categoria "renovação"
        var tiposPlanoRenovacao = await _context.PM_TiposPlano
            .Where(tp => tp.IdPlano == plano.IdPlano && tp.Categoria == CategoriaTipoManutencao.Renovacao)
            .Include(tp => tp.TipoManutencao)
            .ToListAsync();

        //Monta lista com meses, se é contínua ou não e o valor da confibailidade
        var listaRenovacao = tiposPlanoRenovacao
            .Select(tp => new
            {
                Meses = (double?)(tp.Dias / 30.0),
                Continua = tp.Continua,
                Confiabilidade = tp.TipoManutencao?.Confiabilidade
            })
            .ToList();

        return new
        {
            UltimaData = ultimaRenovacao?.UltimaRenovacao,
            ListaRenovacao = listaRenovacao,
            TempoCiclo = plano.Estrategia?.Ciclos
        };
    }

    /// <summary>
    /// Obtém uma timeline de fichas de um ativo para um range de data específico
    /// </summary>
    public async Task<IReadOnlyList<TimelineItem>> BuscarTimelineAsync(
        DateTime dataInicio,
        DateTime dataFim,
        string idLi)
    {
        // 1. Projeção usando Tipo Anônimo (Nomes e tipos devem ser IDÊNTICOS nas duas queries)
        var notas = _notaModels.AsNoTracking()
            .Where(n => n.IdLi == idLi
                        && n.DataNota.HasValue
                        && n.DataNota.Value >= dataInicio
                        && n.DataNota.Value <= dataFim)
            .Select(n => new 
            {
                Id = n.IdNota,
                Classificacao = n.Classificacao,
                Descricao = n.DescricaoBreve ?? n.AnomaliaObservada,
                Data = n.DataNota
            });

        var ordens = _ordemModels.AsNoTracking()
            .Where(o => o.IdLi == idLi
                        && o.DataCriacao.HasValue
                        && o.DataCriacao.Value >= dataInicio
                        && o.DataCriacao.Value <= dataFim)
            .Select(o => new 
            {
                Id = o.IdOrdem, // Assumindo que IdNota e IdOrdem são do mesmo tipo (ex: long)
                Classificacao = o.Classificacao,
                Descricao = o.DescricaoBreve,
                Data = o.DataCriacao
            });

        // 2. O Concat agora acontece 100% no banco de dados (SQL UNION ALL)
        var combinadosDb = await notas
            .Concat(ordens)
            .OrderBy(r => r.Data)
            .ToListAsync()
            .ConfigureAwait(false);

        // 3. Conversão em memória para o seu DTO final (TimelineItem)
        var timeline = combinadosDb.Select(x => new TimelineItem(
            x.Id,
            x.Classificacao,
            x.Descricao,
            x.Data
        )).ToList();

        return timeline;
    }
}
