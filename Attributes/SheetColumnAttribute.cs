namespace app_pm_back.Attributes;

/// <summary>
/// Anexa ediquetas a colunas de planilhas
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SheetColumnAttribute : Attribute
{
    /// <summary>
    /// Nome da coluna da planilha
    /// </summary>
    public string NomeDaColuna { get; }
    
    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="nomeDaColuna"></param>
    public SheetColumnAttribute(string nomeDaColuna)
    {
        NomeDaColuna = nomeDaColuna;
    }
}