using System.Collections;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;
using app_pm_back.Services.Extensions;

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
        Classificacao Classificacao,
        string? Descricao,
        DateTime? DataCriacao);

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="GenericSearchService"/>.
    /// </summary>
    public GenericSearchService(AppDbContext context, ConfiguracaoGlobalService configuracaoGlobalService)
    {
        _context = context;
        _notaModels = context.PM_Notas;
        _ordemModels = context.PM_Ordens;
        _planoModels = context.PM_Planos;
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
    /// Obtém um registro específico em uma tabela.
    /// </summary>
    /// <param name="tabela">Nome da tabela.</param>
    /// <param name="campo">Campo de busca.</param>
    /// <param name="valor">Valor atribuído ao campo.</param>
    /// <param name="parcial"></param>
    /// <returns>Resultado da operação.</returns>
    public async Task<object> BuscarPorCampoAsync(
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
        var task = (Task<object>)generico.Invoke(
            this,
            new object?[] { campo, valor, parcial, pagina, tamanho })!;

        return await task.ConfigureAwait(false);
    }

    /// <summary>
    /// Obtém um registro específico.
    /// </summary>
    /// <param name="busca">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    public async Task<object> BuscarNotaOrdemAsync(
        dynamic busca,
        int? pagina,
        int? tamanho,
        Fichas? Tfichas)
    {
        if (busca == null)
        {
            throw new ArgumentNullException(nameof(busca));
        }

        // Includes para buscar o Plano e, em seguida, a Estratégia
        var notasQuery = _notaModels
            .Include(n => n.Plano)
            .ThenInclude(p => p.Estrategia)
            .AsNoTracking()
            .AsQueryable();

        var ordensQuery = _ordemModels
            .Include(o => o.Plano)
            .ThenInclude(p => p.Estrategia)
            .AsNoTracking()
            .AsQueryable();

        // Safe check for properties of dynamic/anonymous object
        var buscaType = (Type)busca.GetType();

        StatusFichas? statusNota = buscaType.GetProperty("StatusNota")?.GetValue(busca);
        if (statusNota.HasValue)
        {
            notasQuery = notasQuery.Where(n => n.StatusNota == statusNota.Value);
        }

        var notaIdInicioVal = buscaType.GetProperty("NotaIdInicio")?.GetValue(busca);
        long? notaIdInicio = notaIdInicioVal != null ? Convert.ToInt64(notaIdInicioVal) : null;
        if (notaIdInicio.HasValue)
        {
            notasQuery = notasQuery.Where(n => n.NumeroNota >= notaIdInicio.Value);
        }

        var notaIdFimVal = buscaType.GetProperty("NotaIdFim")?.GetValue(busca);
        long? notaIdFim = notaIdFimVal != null ? Convert.ToInt64(notaIdFimVal) : null;
        if (notaIdFim.HasValue)
        {
            notasQuery = notasQuery.Where(n => n.NumeroNota <= notaIdFim.Value);
        }

        var ordemIdInicioVal = buscaType.GetProperty("OrdemIdInicio")?.GetValue(busca);
        long? ordemIdInicio = ordemIdInicioVal != null ? Convert.ToInt64(ordemIdInicioVal) : null;
        if (ordemIdInicio.HasValue)
        {
            ordensQuery = ordensQuery.Where(o => o.NumeroOrdem >= ordemIdInicio.Value);
        }

        var ordemIdFimVal = buscaType.GetProperty("OrdemIdFim")?.GetValue(busca);
        long? ordemIdFim = ordemIdFimVal != null ? Convert.ToInt64(ordemIdFimVal) : null;
        if (ordemIdFim.HasValue)
        {
            ordensQuery = ordensQuery.Where(o => o.NumeroOrdem <= ordemIdFim.Value);
        }

        string? localInstalacaoInicio = buscaType.GetProperty("LocalInstalacaoInicio")?.GetValue(busca);
        if (!string.IsNullOrEmpty(localInstalacaoInicio))
        {
            notasQuery = notasQuery.Where(n => string.Compare(n.IdLi, localInstalacaoInicio) <= 0);
            ordensQuery = ordensQuery.Where(o => string.Compare(o.IdLi, localInstalacaoInicio) <= 0);
        }

        string? localInstalacaoFim = buscaType.GetProperty("LocalInstalacaoFim")?.GetValue(busca);
        if (!string.IsNullOrEmpty(localInstalacaoInicio))
        {
            notasQuery = notasQuery.Where(n => string.Compare(n.IdLi, localInstalacaoInicio) >= 0);
            ordensQuery = ordensQuery.Where(o => string.Compare(o.IdLi, localInstalacaoInicio) >= 0);
        }

        var listaAtivosRaw = buscaType.GetProperty("ListaAtivos")?.GetValue(busca);
        if (listaAtivosRaw != null)
        {
            // converte par uma lista de ComponentType?
            var listaAtivosConvertida = ((IEnumerable)listaAtivosRaw)
                .Cast<ComponentType?>()
                .ToList();

            notasQuery = notasQuery.Where(n => listaAtivosConvertida.Contains(n.IdAtivo));
            ordensQuery = ordensQuery.Where(o => listaAtivosConvertida.Contains(o.IdAtivo));
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

        var prioridadeInicioVal = buscaType.GetProperty("PrioridadeInicio")?.GetValue(busca);
        Prioridade? prioridadeInicio = prioridadeInicioVal as Prioridade?;
        if (prioridadeInicio.HasValue)
        {
            notasQuery = notasQuery.Where(n => n.Prioridade >= prioridadeInicio.Value);
        }

        var prioridadeFimVal = buscaType.GetProperty("PrioridadeFim")?.GetValue(busca);
        Prioridade? prioridadeFim = prioridadeFimVal as Prioridade?;
        if (prioridadeFim.HasValue)
        {
            notasQuery = notasQuery.Where(n => n.Prioridade <= prioridadeFim.Value);
        }

        var classificacaoVal = buscaType.GetProperty("Classificacao")?.GetValue(busca);
        Classificacao? classificacao = classificacaoVal as Classificacao?;
        if (classificacao.HasValue)
        {
            notasQuery = notasQuery.Where(n => n.Classificacao == classificacao.Value);
            ordensQuery = ordensQuery.Where(o => o.Classificacao == classificacao.Value);
        }

        StatusFichas? statusOrdem = buscaType.GetProperty("StatusOrdem")?.GetValue(busca);
        if (statusOrdem.HasValue)
        {
            ordensQuery = ordensQuery.Where(o => o.StatusOrdem == statusOrdem.Value);
        }

        var listaUnica = new List<object>();

        if (Tfichas == Fichas.Ambas || Tfichas == Fichas.Notas)
        {
            notasQuery = notasQuery.AplicarOrdenacao(_context);
            var todasNotas = await notasQuery.ToListAsync().ConfigureAwait(false);
            listaUnica.AddRange(todasNotas);
        }

        // Busca Ordens apenas se for Ambas (0) ou Ordens (2)
        if (Tfichas == Fichas.Ambas || Tfichas == Fichas.Ordens)
        {
            ordensQuery = ordensQuery.AplicarOrdenacao(_context);
            var todasOrdens = await ordensQuery.ToListAsync().ConfigureAwait(false);
            listaUnica.AddRange(todasOrdens);
        }

        //Cálculos da paginação
        int totalGeral = listaUnica.Count;
        int p = pagina ?? 1;
        int t = tamanho ?? 20;
        int totalPaginas = (int)Math.Ceiling((double)totalGeral / t);

        //Pega os 'n' primeiros itens dependendo da página
        var itensPaginados = listaUnica.Skip((p - 1) * t).Take(t).ToList();

        //Separa de volta para preencher o NotaOrdemSearchResult
        var notasResult = itensPaginados.OfType<NotaModel>().ToList();
        var ordensResult = itensPaginados.OfType<OrdemModel>().ToList();

        var result = new NotaOrdemSearchResult(notasResult, ordensResult);

        return new
        {
            Total = totalGeral,
            TotalPaginas = totalPaginas,
            pagina = p,
            Tamanho = t,
            Result = result
        };
    }

    private async Task<object> BuscarInternoAsync<T>(
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
        query = query.AplicarOrdenacao(_context);
        int count = query.Count();

        if (pagina.HasValue || tamanho.HasValue)
        {
            int p = pagina ?? 1;
            int t = tamanho ?? 20;
            query = query.Skip((p - 1) * t).Take(t);
        }

        var result = await query.Cast<object>().ToListAsync().ConfigureAwait(false);
        return new { Total = count, Result = result };
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
                codigoPlanoParaBuscar =
                    ultimaOrdem.IdNota.HasValue ? (long?)ultimaOrdem.IdNota.Value : null; // search using NotaId
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


    /// <summary>
    /// Obtém uma timeline de fichas de um ativo para um range de data específico
    /// </summary>
    public async Task<IReadOnlyList<TimelineItem>> BuscarTimelineAsync(
        DateTime dataInicio,
        DateTime dataFim,
        string idLi)
    {
        // projeção usando Tipo Anônimo (Nomes e tipos devem ser IDÊNTICOS nas duas queries)
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
                Id = o.IdOrdem,
                Classificacao = o.Classificacao,
                Descricao = o.DescricaoBreve,
                Data = o.DataCriacao
            });

        var combinadosDb = await notas
            .Concat(ordens)
            .OrderBy(r => r.Data)
            .ToListAsync()
            .ConfigureAwait(false);

        var timeline = combinadosDb.Select(x => new TimelineItem(
            x.Id,
            x.Classificacao,
            x.Descricao,
            x.Data
        )).ToList();

        return timeline;
    }
}