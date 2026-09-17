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

//
// public class FileTreeNodeDto
// {
//     // Id gerado como string para evitar conflitos no front-end (ex: "dir_1", "nota_5", "ordem_10")
//     public string Id { get; set; } = string.Empty;
//
//     // O Id real do banco de dados
//     public long? BackendId { get; set; }
//
//     public string Name { get; set; } = string.Empty;
//
//     // "folder", "nota" ou "ordem"
//     public string Type { get; set; } = string.Empty;
//
//     // 0 para pasta, 1 para nota, 2 para ordem (Mapeando o OrganizeAssetElementCategory)
//     public int ElementCategory { get; set; } 
//
//     // Propriedades opcionais de controle da UI no Frontend
//     public bool? Expanded { get; set; }
//     public bool? ChildrenLoaded { get; set; }
//     public bool? Loading { get; set; }
//
//     // Lista unificada contendo Subdiretórios, Notas e Ordens
//     public List<FileTreeNodeDto>? Children { get; set; }
// }