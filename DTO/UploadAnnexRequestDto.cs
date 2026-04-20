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
    public IFormFile Arquivo { get; set; } = null!;

    /// <summary>
    /// Id da entidade referenciada.
    /// </summary>
    public long ReferenciaId { get; set; }

    /// <summary>
    /// Tipo da entidade referenciada.
    /// </summary>
    public string TipoReferencia { get; set; } = string.Empty;
}
