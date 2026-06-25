using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace app_pm_back.Data;

/// <summary>
/// Interceptador de exceções do banco de dados para retornar mensagens amigáveis globais.
/// </summary>
public class DatabaseExceptionInterceptor : SaveChangesInterceptor
{
    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        var friendlyException = GetFriendlyException(eventData.Exception);
        if (friendlyException != null)
        {
            throw friendlyException;
        }
        base.SaveChangesFailed(eventData);
    }

    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        var friendlyException = GetFriendlyException(eventData.Exception);
        if (friendlyException != null)
        {
            throw friendlyException;
        }
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    private Exception? GetFriendlyException(Exception exception)
    {
        if (exception is DbUpdateException dbEx && dbEx.InnerException is PostgresException pgEx)
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
        return null;
    }
}
