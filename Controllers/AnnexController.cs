using app_pm_back.DTO;
using app_pm_back.Services;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

/// <summary>
/// Controller para anexos
/// </summary>
[ApiController]
[Route("api/V1/annex")]
public class AnnexController : ControllerBase
{
    private readonly AnnexService _annexService;

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="annexService"></param>
    public AnnexController(AnnexService annexService)
    {
        _annexService = annexService;
    }

    /// <summary>
    /// Upload de um arquivo em IFormFile 
    /// </summary>
    /// <param name="request">Dados de upload do anexo.</param>
    /// <returns></returns>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadArquivo([FromForm] UploadAnnexRequestDto request)
    {
        if (request.Arquivo == null || request.Arquivo.Length == 0)
        {
            return BadRequest(new { message = "Arquivo invalido para upload." });
        }

        if (string.IsNullOrWhiteSpace(request.TipoReferencia))
        {
            return BadRequest(new { message = "TipoReferencia e obrigatorio." });
        }

        if (!TryResolveReferencia(request, out var notaId, out var ordemId, out var error))
        {
            return BadRequest(new { message = error });
        }

        var result = await _annexService.UploadAnexo(request.Arquivo, notaId, ordemId, request.TipoReferencia, request.Categoria);
        return Ok(new { message = $"Arquivo enviado com sucesso {result.IdAnexo}" });
    }

    /// <summary>
    /// Download de um arquivo com base no id
    /// </summary>
    /// <param name="id">Id regsitrado no arquivo</param>
    /// <returns></returns>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> DownloadArquivo([FromRoute] long id)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "Id do anexo invalido." });
        }

        var anexo = await _annexService.DownloadAnexo(id);
        if (anexo == null) return NotFound();

        return File(anexo.Dados, anexo.MimeType, anexo.NomeAnexo);
    }
    
    /// <summary>
    /// Realiza a busca de um arquivo no banco de dados com base no Id e Tipo relacionados 
    /// </summary>
    /// <param name="referenciaId">Id da referência de busca</param>
    /// <param name="tipoReferencia">Tipo da referência de busca</param>
    /// <returns></returns>
    [HttpGet("referencia/{referenciaId:long}/{tipoReferencia}")]
    public async Task<IActionResult> BuscarArquivo([FromRoute] long referenciaId, [FromRoute] string tipoReferencia)
    {
        if (referenciaId <= 0 || string.IsNullOrWhiteSpace(tipoReferencia))
        {
            return BadRequest(new { message = "ReferenciaId e tipoReferencia sao obrigatorios." });
        }

        if (!TryResolveReferencia(referenciaId, tipoReferencia, out var notaId, out var ordemId, out var error))
        {
            return BadRequest(new { message = error });
        }

        var list = await _annexService.BuscarAnexo(notaId, ordemId, tipoReferencia);
        return Ok(list);
    }

    private static bool TryResolveReferencia(UploadAnnexRequestDto request, out long? notaId, out long? ordemId, out string? error)
    {
        notaId = request.NotaId;
        ordemId = request.OrdemId;
        error = null;

        if (notaId.HasValue && ordemId.HasValue)
        {
            error = "Informe apenas NotaId ou OrdemId.";
            return false;
        }

        if (notaId.HasValue || ordemId.HasValue)
        {
            return true;
        }

        if (!request.ReferenciaId.HasValue || request.ReferenciaId <= 0)
        {
            error = "Informe NotaId ou OrdemId (ou ReferenciaId legado).";
            return false;
        }

        return TryResolveReferencia(request.ReferenciaId.Value, request.TipoReferencia, out notaId, out ordemId, out error);
    }

    private static bool TryResolveReferencia(long referenciaId, string tipoReferencia, out long? notaId, out long? ordemId, out string? error)
    {
        notaId = null;
        ordemId = null;
        error = null;

        if (IsTipoReferenciaOrdem(tipoReferencia))
        {
            ordemId = referenciaId;
            return true;
        }

        if (IsTipoReferenciaNota(tipoReferencia))
        {
            notaId = referenciaId;
            return true;
        }

        error = "TipoReferencia invalido para associacao de nota ou ordem.";
        return false;
    }

    private static bool IsTipoReferenciaOrdem(string tipoReferencia)
    {
        return tipoReferencia.Contains("ORDEM", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsTipoReferenciaNota(string tipoReferencia)
    {
        return tipoReferencia.Contains("NOTA", StringComparison.OrdinalIgnoreCase);
    }
}

