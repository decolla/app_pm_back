using app_pm_back.Models.Domain.Diretorios;

namespace app_pm_back.DTO;

public class DiretorioDto
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public long? IdPai { get; set; }
    
    public List<DiretorioDto> Subdiretorios { get; set; } = new List<DiretorioDto>();
    
    public List<FolhaDto> Notas { get; set; } = new List<FolhaDto>();
    public List<FolhaDto> Ordens { get; set; } = new List<FolhaDto>();
}

public class FolhaDto
{
    public long Id { get; set; }
    public string? Nome { get; set; }
    public string Tipo { get; set; } = string.Empty; // nota ou ordem
}