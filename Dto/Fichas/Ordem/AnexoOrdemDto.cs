namespace app_pm_back.DTO;

public class AnexoOrdemDto
{
    public long IdAnexoOrdem { get; set; }
    
    public required string Nome { get; set; }

    public required string TipoArquivo { get; set; }
    
    public string? TipoAnexo { get; set; }

    public byte[]? Dados { get; set; }

    public long? IdOrdem { get; set; }
}