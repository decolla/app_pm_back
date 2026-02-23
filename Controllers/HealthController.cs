using app_pm_back.Services;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

[ApiController]
[Route("health/")]
public class HealthController : ControllerBase
{
    private readonly IReportService _reportService;
    
    public HealthController(IReportService reportService)
    {
        _reportService = reportService;
    }
    
    /// <summary>
    ///  Return the current status of the application. This endpoint is useful for monitoring the health of the application and getting insights into its performance and any issues that may have occurred since the last report was retrieved.
    /// </summary>
    /// <returns></returns>
    [HttpGet("")]
    public IActionResult GetStatus()
    {
        return Ok(_reportService.GetStatus());
    }

    /// <summary>
    ///  Return the last report and reset the current report to start a new one. This endpoint is useful for monitoring the health of the application and getting insights into its performance and any issues that may have occurred since the last report was retrieved.
    /// </summary>
    /// <returns></returns>
    [HttpGet("report")]
    public IActionResult GetLastReport()
    {
        ReportService lastReportService = _reportService.GetReport();
        _reportService.Reset();
        return Ok(lastReportService);
    }
}