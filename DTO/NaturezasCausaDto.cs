namespace app_pm_back.DTO;

public class NaturezasCausaDto
{
    /// <summary>
    /// Id da Regra
    /// </summary>
    public long IdRegra { get; set; }

    /// <summary>
    /// Palavra chave usada para a filtragem 
    /// </summary>
    /// <example>Inundação, vandalismo, sujeira, etc</example>
    public string PalavraChave { get; set; }
    
    /// <summary>
    /// Qual o tipo de classificação, só pode ser Corretiva (true) ou Ocorrencia Alheia (False)
    /// </summary>
    public bool EhCorretiva { get; set; }
}