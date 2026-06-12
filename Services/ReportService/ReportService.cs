using System.Text.Json.Serialization;
using app_pm_back.Models;
using app_pm_back.Models.ErrorReport;

namespace app_pm_back.Services;
/// <summary>
/// Classe ReportService.
/// </summary>

public class ReportService : IReportService
{
    [JsonConverter(typeof(UnixMillisecondsDateTimeConverter))]
    public DateTime Start { get; } = DateTime.UtcNow;
    /// <summary>
    /// Obtém ou define o valor de Last.
    /// </summary>
    [JsonConverter(typeof(UnixMillisecondsDateTimeConverter))]
    public DateTime Last { get; set; } = DateTime.UtcNow;
    public Status Status { get; private set; } = Status.RUNNING;
    public ExceptionInfo ExceptionInfo { get; private set; } = new();
    public string ApplicationName { get; private set; } = "app_pm_back";
    public ResetInfo? ResetInfo;
    
    // Get the current report instance
    /// <summary>
    /// Obtém um registro específico.
    /// </summary>
    /// <returns>Resultado da operação.</returns>
    public ReportService GetReport()
    {
        return this;
    }

    // Register a critical exception
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="status">Parâmetro do método.</param>
    public void CriticalException(Status status)
    {
        if (Status < status)
            Status = status;

        Last = DateTime.UtcNow;
        ExceptionInfo.IncrementCritical();
    }

    // Register a moderate exception
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="status">Parâmetro do método.</param>
    public void ModerateException(Status status)
    {
        if (Status < status)
            Status = status;

        Last = DateTime.UtcNow;
        ExceptionInfo.IncrementModerate();
    }

    // Register a light exception
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="running">Parâmetro do método.</param>
    public void LightException(Status running)
    {
        Last = DateTime.UtcNow;
        ExceptionInfo.IncrementLight();
    }

    // Reset the report to its initial state
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    public void Reset()
    {
        Last = DateTime.UtcNow;
        ExceptionInfo = new ExceptionInfo();
    }

    // Get the current status of the report
    /// <summary>
    /// Obtém um registro específico.
    /// </summary>
    /// <returns>Resultado da operação.</returns>
    public Status GetStatus()
    {
        return Status;
    }
}