namespace app_pm_back.Models.ErrorReport;

public class ExceptionInfo
{
    private int _light;
    private int _moderate;
    private int _critical;

    public int Light => _light;
    public int Moderate => _moderate;
    public int Critical => _critical;

    public void IncrementLight() => Interlocked.Increment(ref _light);

    public void IncrementModerate() => Interlocked.Increment(ref _moderate);

    public void IncrementCritical() => Interlocked.Increment(ref _critical);
}