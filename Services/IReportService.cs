using app_pm_back.Models;

namespace app_pm_back.Services;

public interface IReportService
{
    // Register a critical exception
    public void CriticalException(Status status);

    // Register a moderate exception
    public void ModerateException(Status status);

    // Register a light exception
    public void LightException(Status running);
    
    // Reset the report service, clearing all recorded exceptions and statuses
    public void Reset();

    // Get the current status of the report service, which may include information about recorded exceptions and overall health
    public Status GetStatus();

    // Get a report of the current status, which may include detailed information about recorded exceptions and overall health
    public ReportService GetReport();
}