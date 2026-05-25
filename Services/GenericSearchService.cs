using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using app_pm_back.Data;
using app_pm_back.Models;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services;

/// <summary>
/// Classe para busca genérica entre as tabelas do banco de dados 
/// </summary>
public class GenericSearchService
{
    private readonly AppDbContext _context;
    private readonly DbSet<NotaManutencaoModel> _notaManutencaoModels;
    private readonly DbSet<OrdemManutencaoModel> _ordemManutencaoModels;
    private readonly DbSet<PlanoManutencaoModel> _planosManutencaoModels;
    private static readonly Dictionary<string, Type> Tabelas = BuildTabelas();

    public record NotaOrdemSearchResult(
        IReadOnlyList<object> Notas,
        IReadOnlyList<object> Ordens);

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="context"></param>
    public GenericSearchService(
        AppDbContext context)
    {
        _context = context;
        _notaManutencaoModels = context.Set<NotaManutencaoModel>();
        _ordemManutencaoModels = context.Set<OrdemManutencaoModel>();
        _planosManutencaoModels = context.Set<PlanoManutencaoModel>();
    }
    
    /// <summary>
    /// Recebe os parâmetros para a busca
    /// </summary>
    /// <param name="tabela">Nome da tabela que será buscada</param>
    /// <param name="campo">Campo a ser buscado</param>
    /// <param name="nome">Nome a ser buscado no campo</param>
    /// <param name="pagina">Página de exibição</param>
    /// <param name="tamanho">Tamanho de cada página</param>
    /// <returns></returns>
    public Task<IReadOnlyList<object>> BuscarPorNomeAsync(
        string tabela,
        string campo,
        string nome,
        int? pagina,
        int? tamanho)
    {
        return BuscarPorCampoAsync(tabela, campo, nome, true, pagina, tamanho);
    }

    /// <summary>
    /// Lista as tabelas e seus campos disponíveis para busca.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Realiza uma busca em nota e ordem com base nos filtros selecionados
    /// </summary>
    /// <param name="busca">Modelo de dado de busca com campos comparáveis</param>
    /// <param name="pagina">Pagina a ser exibida</param>
    /// <param name="tamanho">Tamanho da página</param>
    /// <returns>Lista de Nota e Ordem</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public async Task<NotaOrdemSearchResult> BuscarNotaOrdemAsync(
        BuscaOrdemNotaModel busca,
        int? pagina,
        int? tamanho)
    {
        if (busca == null)
        {
            throw new ArgumentNullException(nameof(busca));
        }

        var notasQuery = _notaManutencaoModels.AsNoTracking().AsQueryable();
        var ordensQuery = _ordemManutencaoModels.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca.StatusNota))
        {
            notasQuery = notasQuery.Where(n => n.StatusNota == busca.StatusNota);
        }

        if (busca.NotaIdInicio.HasValue)
        {
            long inicio = busca.NotaIdInicio.Value;
            notasQuery = notasQuery.Where(n => n.NumeroNota >= inicio);
        }

        if (busca.NotaIdFim.HasValue)
        {
            long fim = busca.NotaIdFim.Value;
            notasQuery = notasQuery.Where(n => n.NumeroNota <= fim);
        }

        if (busca.OrdemIdInicio.HasValue)
        {
            long inicio = busca.OrdemIdInicio.Value;
            ordensQuery = ordensQuery.Where(o => o.IdOrdem >= inicio);
        }

        if (busca.OrdemIdFim.HasValue)
        {
            long fim = busca.OrdemIdFim.Value;
            ordensQuery = ordensQuery.Where(o => o.IdOrdem <= fim);
        }

        if (busca.LocalInstalacaoInicio.HasValue)
        {
            int inicio = busca.LocalInstalacaoInicio.Value;
            notasQuery = notasQuery.Where(n => n.LocalInstalacaoCodigo >= inicio);
            ordensQuery = ordensQuery.Where(o => o.LocalInstalacaoCodigo >= inicio);
        }

        if (busca.LocalInstalacaoFim.HasValue)
        {
            int fim = busca.LocalInstalacaoFim.Value;
            notasQuery = notasQuery.Where(n => n.LocalInstalacaoCodigo <= fim);
            ordensQuery = ordensQuery.Where(o => o.LocalInstalacaoCodigo <= fim);
        }
        
        // if (!string.IsNullOrWhiteSpace(busca.AtivoInicio))
        // {
        //     var inicio = busca.AtivoInicio.Trim();
        //     notasQuery = notasQuery.Where(n => n.EquipamentoTag != null
        //                                        && string.Compare(n.EquipamentoTag, inicio) >= 0);
        //     ordensQuery = ordensQuery.Where(o => o.EquipamentoTag != null
        //                                         && string.Compare(o.EquipamentoTag, inicio) >= 0);
        // }
        //
        // if (!string.IsNullOrWhiteSpace(busca.AtivoFim))
        // {
        //     var fim = busca.AtivoFim.Trim();
        //     notasQuery = notasQuery.Where(n => n.EquipamentoTag != null
        //                                        && string.Compare(n.EquipamentoTag, fim) <= 0);
        //     ordensQuery = ordensQuery.Where(o => o.EquipamentoTag != null
        //                                         && string.Compare(o.EquipamentoTag, fim) <= 0);
        // }

        if (busca.NotaDataInicio.HasValue)
        {
            var inicio = busca.NotaDataInicio.Value;
            notasQuery = notasQuery.Where(n => n.DataNota >= inicio);
        }

        if (busca.NotaDataFim.HasValue)
        {
            var fim = busca.NotaDataFim.Value;
            notasQuery = notasQuery.Where(n => n.DataNota <= fim);
        }

        if (busca.OrdemDataInicio.HasValue)
        {
            var inicio = busca.OrdemDataInicio.Value;
            ordensQuery = ordensQuery.Where(o => o.DataCriacao.HasValue && o.DataCriacao.Value >= inicio);
        }

        if (busca.OrdemDataFim.HasValue)
        {
            var fim = busca.OrdemDataFim.Value;
            ordensQuery = ordensQuery.Where(o => o.DataCriacao.HasValue && o.DataCriacao.Value <= fim);
        }

        if (busca.GpmInicio.HasValue)
        {
            int inicio = busca.GpmInicio.Value;
            ordensQuery = ordensQuery.Where(o => o.GpmResponsavelId >= inicio);
        }

        if (busca.GpmFim.HasValue)
        {
            int fim = busca.GpmFim.Value;
            ordensQuery = ordensQuery.Where(o => o.GpmResponsavelId <= fim);
        }

        if (!string.IsNullOrWhiteSpace(busca.PrioridadeInicio))
        {
            if (!int.TryParse(busca.PrioridadeInicio, out var inicio))
            {
                throw new ArgumentException("PrioridadeInicio invalida.", nameof(busca));
            }

            notasQuery = notasQuery.Where(n => n.Prioridade >= inicio);
            ordensQuery = ordensQuery.Where(o => o.Prioridade >= inicio);
        }

        if (!string.IsNullOrWhiteSpace(busca.PrioridadeFim))
        {
            if (!int.TryParse(busca.PrioridadeFim, out var fim))
            {
                throw new ArgumentException("PrioridadeFim invalida.", nameof(busca));
            }

            notasQuery = notasQuery.Where(n => n.Prioridade <= fim);
            ordensQuery = ordensQuery.Where(o => o.Prioridade <= fim);
        }

        if (!string.IsNullOrWhiteSpace(busca.CriadoPorInicio))
        {
            if (!int.TryParse(busca.CriadoPorInicio, out var inicio))
            {
                throw new ArgumentException("CriadoPorInicio invalido.", nameof(busca));
            }

            notasQuery = notasQuery.Where(n => n.CriadoPorUsuarioId >= inicio);
            ordensQuery = ordensQuery.Where(o => o.CriadaPorUsuarioId >= inicio);
        }

        if (!string.IsNullOrWhiteSpace(busca.CriadoPorFim))
        {
            if (!int.TryParse(busca.CriadoPorFim, out var fim))
            {
                throw new ArgumentException("CriadoPorFim invalido.", nameof(busca));
            }

            notasQuery = notasQuery.Where(n => n.CriadoPorUsuarioId <= fim);
            ordensQuery = ordensQuery.Where(o => o.CriadaPorUsuarioId <= fim);
        }

        if (!string.IsNullOrWhiteSpace(busca.TipoNota))
        {
            notasQuery = notasQuery.Where(n => n.TipoNota == busca.TipoNota || n.TipoDeNota == busca.TipoNota);
        }

        if (!string.IsNullOrWhiteSpace(busca.TipoOrdem))
        {
            ordensQuery = ordensQuery.Where(o => o.TipoOrdem == busca.TipoOrdem);
        }

        if (!string.IsNullOrWhiteSpace(busca.StatusOrdem))
        {
            ordensQuery = ordensQuery.Where(o => o.StatusOrdem == busca.StatusOrdem);
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

    /// <summary>
    /// Realiza a busca interna utilizando expressão dinâmica para filtrar pelo campo e valor informados, com suporte a busca parcial em strings e paginação.
    /// </summary>
    /// <param name="campo">Campo a ser feito a busca</param>
    /// <param name="valor">Valor a ser buscado dentro do campo</param>
    /// <param name="parcial">Se a busca será parcial ou não</param>
    /// <param name="pagina"></param>
    /// <param name="tamanho"></param>
    /// <typeparam name="T">Tabela a ser feita a busca</typeparam>
    /// <returns>Lista de elementos buscados</returns>
    /// <exception cref="ArgumentException"></exception>
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

    /// <summary>
    /// Busca a propriedade da tabela pelo nome do campo, ignorando maiúsculas e minúsculas. Lança exceção se a propriedade não for encontrada.
    /// </summary>
    /// <param name="tipo">Tipo da propriedade a ser buscado</param>
    /// <param name="campo"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
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

    private static bool CampoSuportado(Type tipo, string campo)
    {
        var property = tipo
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(p => string.Equals(p.Name, campo, StringComparison.OrdinalIgnoreCase));

        if (property == null)
        {
            return false;
        }

        return TipoSuportado(ObterTipoBase(property.PropertyType));
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
