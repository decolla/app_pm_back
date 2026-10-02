using app_pm_back.Attributes;

namespace app_pm_back.DTO;

#pragma warning disable CS1591

public class AnexoNotaDto
{
    public long IdAnexoNota { get; set; }
    
    public long IdNota { get; set; }
    
    public string? Nome { get; set; }
    
    public string? TipoArquivo { get; set; }


    public byte[]? Dados { get; set; }
    
}
#pragma warning restore CS1591