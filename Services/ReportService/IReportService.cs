using app_pm_back.Models;

namespace app_pm_back.Services;

public interface IReportService
{
    // Register a critical exception
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="status">Parâmetro do método.</param>
    public void CriticalException(Status status);

    // Register a moderate exception
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="status">Parâmetro do método.</param>
    public void ModerateException(Status status);

    // Register a light exception
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="running">Parâmetro do método.</param>
    public void LightException(Status running);
    
    // Reset the report service, clearing all recorded exceptions and statuses
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    public void Reset();

    // Get the current status of the report service, which may include information about recorded exceptions and overall health
    /// <summary>
    /// Obtém um registro específico.
    /// </summary>
    /// <returns>Resultado da operação.</returns>
    public Status GetStatus();

    // Get a report of the current status, which may include detailed information about recorded exceptions and overall health
    /// <summary>
    /// Obtém um registro específico.
    /// </summary>
    /// <returns>Resultado da operação.</returns>
    public ReportService GetReport();
}