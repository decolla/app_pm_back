using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace app_pm_back.Data;

/// <summary>
/// Interceptador de exceções do banco de dados para retornar mensagens globais.
/// </summary>
public class DatabaseExceptionInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// Exception caso ocorra algum erro no banco de dados durante mudança de dados
    /// </summary>
    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        var friendlyException = GetFriendlyException(eventData.Exception);
        if (friendlyException != null)
        {
            throw friendlyException;
        }
        base.SaveChangesFailed(eventData);
    }

    
    /// <summary>
    /// Exception caso ocorra erro no banco de dados durante a mudança de dados assíncrona
    /// </summary>
    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        var friendlyException = GetFriendlyException(eventData.Exception);
        if (friendlyException != null)
        {
            throw friendlyException;
        }
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }
    

    /// <summary>
    /// Gera uma exception amigável para o usuário com base na exceção original do banco de dados
    /// </summary>
    /// <param name="exception">Exception ocorrida</param>
    private Exception? GetFriendlyException(Exception exception)
    {
        if (exception is DbUpdateException dbEx)
        {
            if (dbEx.InnerException is PostgresException pgEx)
            {
                if (pgEx.SqlState == "23503")
                {
                    return new InvalidOperationException("A chave estrangeira passada não existe", exception);
                }
                if (pgEx.SqlState == "23505")
                {
                    return new InvalidOperationException("Este registro já existe no sistema", exception);
                }
            }
            else if (dbEx.InnerException is SqlException sqlEx)
            {
                if (sqlEx.Number == 547)
                {
                    return new InvalidOperationException("A chave estrangeira passada não existe", exception);
                }
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                {
                    return new InvalidOperationException("Este registro já existe no sistema", exception);
                }
            }
        }
        return null;
    }
}
