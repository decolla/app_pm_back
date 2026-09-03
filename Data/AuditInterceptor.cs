using app_pm_back.Interfaces;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace app_pm_back.Data;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IUserContext _userContext;

    // guarda o "diff" capturado em SavingChangesAsync até poder ser persistido em SavedChangesAsync
    private List<(EntityEntry Entry, AuditoriaModel Auditoria)> _pendentes = new();

    // trava contra reentrância: o SaveChangesAsync chamado dentro de SavedChangesAsync
    // dispara este mesmo interceptor de novo; sem essa trava, se algum dia AuditoriaModel
    // vier a implementar IAuditable, isso vira loop infinito
    private bool _gravandoAuditoria;

    public AuditInterceptor(IUserContext userContext)
    {
        _userContext = userContext;
    }

    // SavingChangesAsync captura o diff (PKs de Modified/Deleted e OriginalValue só existem antes do save);
    // SavedChangesAsync resolve a PK dos Added e persiste o log, pois exige um 2º SaveChanges separado.
    // Caso a aplicação caia entre os dois saves, o dado principal fica gravado sem o log correspondente.
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbSchema = Config.RouteConfig.GetDatabaseSchema();
        
        var context = eventData.Context;
        if (context == null || _gravandoAuditoria)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        _pendentes = new List<(EntityEntry, AuditoriaModel)>();
        // TODO: anotar o padrão para capturar usuários a partir do http
        string? usuarioId = _userContext.GetUsuarioIdentificador() ?? _userContext.GetUsuarioIdAtual()?.ToString();

        foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State is EntityState.Detached or EntityState.Unchanged)
                continue;
            
            if (dbSchema == "cg" && entry.State == EntityState.Added)
                continue;
            
            var valoresAntigos = new Dictionary<string, object?>();
            var valoresNovos = new Dictionary<string, object?>();
            PropertyValues? valoresNoBanco = null;
            
            //Busca do banco de dados na hora de interceptar em caso de modified
            if (entry.State == EntityState.Modified)
            {
                valoresNoBanco = await entry.GetDatabaseValuesAsync(cancellationToken);
            }
            
            var camposIgnorados = new HashSet<string>(StringComparer.OrdinalIgnoreCase) 
            { 
                "DataModificacao", 
                "DataCriacao", 
                "UsuarioModificacao"
            };
            
            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary) continue; // ex: PK de identity ainda não gerada pelo banco
                string nome = property.Metadata.Name;
                if (camposIgnorados.Contains(nome)) continue;

                switch (entry.State)
                {
                    case EntityState.Added:
                        valoresNovos[nome] = property.CurrentValue;
                        break;
                    case EntityState.Modified:
        
                        if (valoresNoBanco != null)
                        {
                            // Pega o valor original que estava no banco e o valor novo que veio da API
                            var valorAntigoReal = valoresNoBanco[nome];
                            var valorNovo = property.CurrentValue;

                            //Compara se realmente mudou
                            if (!Equals(valorAntigoReal, valorNovo))
                            {
                                valoresAntigos[nome] = valorAntigoReal;
                                valoresNovos[nome] = valorNovo;
                            }
                        }
                        break;
                    case EntityState.Deleted:
                        valoresAntigos[nome] = property.OriginalValue;
                        break;
                }
            }
            
            if (entry.State == EntityState.Modified && valoresNovos.Count == 0)
            {
                continue;
            }

            // em Modified/Deleted a PK já é conhecida agora; em Added ela só existe
            // depois que o banco gerar o identity, então fica null e é resolvida em SavedChangesAsync
            string? chave = null;
            if (entry.State != EntityState.Added)
            {
                var pk = entry.Metadata.FindPrimaryKey();
                if (pk?.Properties.Count == 1)
                    chave = entry.Property(pk.Properties[0].Name).CurrentValue?.ToString();
            }

            var auditoria = new AuditoriaModel
            {
                IdUsuario = usuarioId,
                NomeTabela = entry.Metadata.GetTableName() ?? entry.Metadata.Name,
                Acao = entry.State,
                ChavePrimaria = chave,
                ValoresAntigos = valoresAntigos.Count > 0 ? System.Text.Json.JsonSerializer.Serialize(valoresAntigos) : null,
                ValoresNovos = valoresNovos.Count > 0 ? System.Text.Json.JsonSerializer.Serialize(valoresNovos) : null,
                DataAlteracao = DateTime.UtcNow
            };

            _pendentes.Add((entry, auditoria));
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null || _pendentes.Count == 0 || _gravandoAuditoria)
            return await base.SavedChangesAsync(eventData, result, cancellationToken);

        // resolve a PK dos registros Added agora que o INSERT já rodou e o identity foi gerado
        foreach (var (entry, auditoria) in _pendentes)
        {
            if (auditoria.ChavePrimaria != null) continue;
            var pk = entry.Metadata.FindPrimaryKey();
            if (pk?.Properties.Count == 1)
                auditoria.ChavePrimaria = entry.Property(pk.Properties[0].Name).CurrentValue?.ToString() ?? "N/A";
        }

        var auditorias = _pendentes.Select(p => p.Auditoria).ToList();
        _pendentes.Clear();

        try
        {
            _gravandoAuditoria = true; // impede reentrância no SaveChangesAsync abaixo
            context.Set<AuditoriaModel>().AddRange(auditorias);
            await context.SaveChangesAsync(cancellationToken); // segunda ida ao banco, só para o log
        }
        finally
        {
            _gravandoAuditoria = false;
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    // garante que o estado não fique "sujo" para a próxima operação em caso de erro no save
    public override Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        _pendentes.Clear();
        _gravandoAuditoria = false;
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        _pendentes.Clear();
        _gravandoAuditoria = false;
        base.SaveChangesFailed(eventData);
    }
}