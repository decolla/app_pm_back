namespace app_pm_back.Models;

public class BuscaOrdemNotaModel
{
    /// <summary>
    /// Status da nota
    /// </summary>
    /// <example>Aberta</example>
    public string? StatusNota { get; set; } 
    
    /// <summary>
    /// Id de início da nota
    /// </summary>
    public int? NotaIdInicio { get; set; } 
    
    /// <summary>
    /// Id de fim da nota
    /// </summary>
    public int? NotaIdFim { get; set; }

    public int? OrdemIdInicio { get; set; }
    public int? OrdemIdFim { get; set; }
    
    public int? LocalInstalacaoInicio { get; set; }
    public int? LocalInstalacaoFim { get; set; }
    
    public string? AtivoInicio { get; set; }
    public string? AtivoFim { get; set; }
    
    public DateTime? NotaDataInicio { get; set; }
    public DateTime? NotaDataFim { get; set; }
    
    public DateTime? OrdemDataInicio { get; set; }
    public DateTime? OrdemDataFim { get; set; }
    
    public int? GpmInicio { get; set; }
    public int? GpmFim { get; set; }
    
    public string? PrioridadeInicio { get; set; }
    public string? PrioridadeFim { get; set; }
    
    public string? CriadoPorInicio { get; set; }
    public string? CriadoPorFim { get; set; }
    
    public string? TipoNota { get; set; }
    public string? TipoOrdem { get; set; }
    
    public string? StatusOrdem { get; set; }
}