namespace app_pm_back.Interfaces;

/// <summary>
/// Provedor centralizado do schema da aplicação.
/// </summary>
public interface IApplicationSchemaProvider
{
    /// <summary>
    /// Nome do schema configurado para esta instância da aplicação.
    /// </summary>
    string Schema { get; }
}
