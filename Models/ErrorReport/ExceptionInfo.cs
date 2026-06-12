namespace app_pm_back.Models.ErrorReport;
/// <summary>
/// Classe ExceptionInfo.
/// </summary>

public class ExceptionInfo
{
    private int _light;
    private int _moderate;
    private int _critical;

    public int Light => _light;
    public int Moderate => _moderate;
    public int Critical => _critical;
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="_light">Parâmetro do método.</param>

    public void IncrementLight() => Interlocked.Increment(ref _light);
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="_moderate">Parâmetro do método.</param>

    public void IncrementModerate() => Interlocked.Increment(ref _moderate);
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="_critical">Parâmetro do método.</param>

    public void IncrementCritical() => Interlocked.Increment(ref _critical);
}