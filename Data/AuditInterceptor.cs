using app_pm_back.Interfaces;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace app_pm_back.Data;

/// <summary>
/// Interceptor de auditoria. Captura compliance e logs pré salvamento em banco
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IUserContext _userContext;

    public AuditInterceptor(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        // verifica se o contexto existe
        var context = eventData.Context;
        if (context == null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        var auditorias = new List<AuditoriaModel>();
        int? usuarioId = _userContext.GetUsuarioIdAtual();

        // varre as entidades que contém a interface Auditable
        foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            // base do log
            var auditoria = new AuditoriaModel
            {
                IdUsuario = usuarioId,
                NomeTabela = entry.Metadata.GetTableName() ?? entry.Metadata.Name,
                Acao = entry.State.ToString(),
                DataAlteracao = DateTime.UtcNow
            };

            // procura a chave primária
            var primaryKey = entry.Metadata.FindPrimaryKey();
            if (primaryKey != null && primaryKey.Properties.Count == 1)
            {
                var pkName = primaryKey.Properties[0].Name;
                auditoria.ChavePrimaria = entry.Property(pkName).CurrentValue?.ToString() ?? "N/A";
            }

            var valoresAntigos = new Dictionary<string, object?>();
            var valoresNovos = new Dictionary<string, object?>();

            // varre as propriedades para capturar os valores antigos e novos
            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary) continue; // ignora propriedades temporárias

                string nome = property.Metadata.Name;

                switch (entry.State)
                {
                    case EntityState.Added:
                        valoresNovos[nome] = property.CurrentValue;
                        break;
                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            valoresAntigos[nome] = property.OriginalValue;
                            valoresNovos[nome] = property.CurrentValue;
                        }
                        break;
                    case EntityState.Deleted:
                        valoresAntigos[nome] = property.OriginalValue;
                        break;
                }
                
                // serializa os valores antigos e novos para o log
                auditoria.ValoresAntigos = valoresAntigos.Count > 0 ? System.Text.Json.JsonSerializer.Serialize(valoresAntigos) : null;
                auditoria.ValoresNovos = valoresNovos.Count > 0 ? System.Text.Json.JsonSerializer.Serialize(valoresNovos) : null;
                
                auditorias.Add(auditoria);
            }
        }

        if (auditorias.Count > 0)
        {
            // adiciona as auditorias no contexto
            context.Set<AuditoriaModel>().AddRange(auditorias);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}