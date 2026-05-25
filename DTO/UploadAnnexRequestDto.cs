using app_pm_back.Models;
using Microsoft.AspNetCore.Http;

namespace app_pm_back.DTO;

/// <summary>
/// Dados de upload de anexo via multipart/form-data.
/// </summary>
public class UploadAnnexRequestDto
{
    /// <summary>
    /// Arquivo enviado.
    /// </summary>
    public IFormFile? Arquivo { get; set; }

    /// <summary>
    /// Id da nota associada.
    /// </summary>
    public long? NotaId { get; set; }

    /// <summary>
    /// Id da ordem associada.
    /// </summary>
    public long? OrdemId { get; set; }

    /// <summary>
    /// Id da entidade referenciada (legado).
    /// </summary>
    public long? ReferenciaId { get; set; }

    /// <summary>
    /// Tipo da entidade referenciada.
    /// </summary>
    public string TipoReferencia { get; set; } = string.Empty;
    
    /// <summary>
    /// Categoria do anexo.
    /// </summary>
    public CategoriaAnexo Categoria { get; set; } = CategoriaAnexo.Outros;
}
