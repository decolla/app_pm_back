using System.Text.Json.Serialization;
using app_pm_back.Models;
using app_pm_back.Models.ErrorReport;

namespace app_pm_back.Services;

public class ReportService : IReportService
{
    [JsonConverter(typeof(UnixMillisecondsDateTimeConverter))]
    public DateTime Start { get; } = DateTime.UtcNow;
    [JsonConverter(typeof(UnixMillisecondsDateTimeConverter))]
    public DateTime Last { get; set; } = DateTime.UtcNow;
    public Status Status { get; private set; } = Status.RUNNING;
    public ExceptionInfo ExceptionInfo { get; private set; } = new();
    public string ApplicationName { get; private set; } = "app_pm_back";
    public ResetInfo? ResetInfo;
    
    // Get the current report instance
    public ReportService GetReport()
    {
        return this;
    }

    // Register a critical exception
    public void CriticalException(Status status)
    {
        if (Status < status)
            Status = status;

        Last = DateTime.UtcNow;
        ExceptionInfo.IncrementCritical();
    }

    // Register a moderate exception
    public void ModerateException(Status status)
    {
        if (Status < status)
            Status = status;

        Last = DateTime.UtcNow;
        ExceptionInfo.IncrementModerate();
    }

    // Register a light exception
    public void LightException(Status running)
    {
        Last = DateTime.UtcNow;
        ExceptionInfo.IncrementLight();
    }

    // Reset the report to its initial state
    public void Reset()
    {
        Last = DateTime.UtcNow;
        ExceptionInfo = new ExceptionInfo();
    }

    // Get the current status of the report
    public Status GetStatus()
    {
        return Status;
    }
}