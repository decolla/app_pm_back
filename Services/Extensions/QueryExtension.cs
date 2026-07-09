using System.Linq.Expressions;
using app_pm_back.Data;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Extensions;

using System.Linq;

public static class QueryExtensions
{
    /// <summary>
    /// Aplica uma ordenação padrão a uma consulta com base na chave primária da entidade.
    /// </summary>
    public static IQueryable<T> AplicarOrdenacao<T>(this IQueryable<T> query, DbContext context) where T : class
    {
        var entityType = context.Model.FindEntityType(typeof(T));
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
}