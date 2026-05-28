using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

/// <summary>
/// Controller para anexos
/// </summary>
[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/anexo")]
public class AnnexController : ControllerBase
{
    private readonly AnnexService _annexService;

    public AnnexController(AnnexService annexService)
    {
        _annexService = annexService;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadArquivo([FromForm] UploadAnnexRequestDto request)
    {
        if (request.Arquivo == null || request.Arquivo.Length == 0)
        {
            return BadRequest(new { message = "Arquivo invalido para upload." });
        }

        if (!TryResolveReferencia(request, out var notaId, out var ordemId, out var error))
        {
            return BadRequest(new { message = error });
        }

        dynamic result = await _annexService.UploadAnexo(request.Arquivo, notaId, ordemId, request.TipoReferencia, request.Categoria);
        return Ok(new { message = $"Arquivo enviado com sucesso {result.IdAnexo}" });
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> DownloadArquivo([FromRoute] long id)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "Id do anexo invalido." });
        }

        var anexo = await _annexService.DownloadAnexo(id);
        if (anexo == null) return NotFound();

        return File(anexo.Value.Dados, anexo.Value.MimeType, anexo.Value.Nome);
    }
    
    [HttpGet("referencia/{referenciaId:long}/{tipoReferencia}")]
    public async Task<IActionResult> BuscarArquivo([FromRoute] long referenciaId, [FromRoute] string tipoReferencia)
    {
        if (referenciaId <= 0)
        {
            return BadRequest(new { message = "ReferenciaId e obrigatorio." });
        }

        if (!Enum.TryParse<TipoReferencia>(tipoReferencia, true, out var tipo))
        {
            return BadRequest(new { message = "TipoReferencia invalido." });
        }

        long? notaId = tipo == TipoReferencia.Nota ? referenciaId : null;
        long? ordemId = tipo == TipoReferencia.Ordem ? referenciaId : null;

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

        if (request.ReferenciaId.HasValue && request.ReferenciaId > 0 && !string.IsNullOrWhiteSpace(request.TipoReferencia))
        {
            if (Enum.TryParse<TipoReferencia>(request.TipoReferencia, true, out var tipo))
            {
                if (tipo == TipoReferencia.Nota)
                {
                    notaId = request.ReferenciaId.Value;
                    return true;
                }

                if (tipo == TipoReferencia.Ordem)
                {
                    ordemId = request.ReferenciaId.Value;
                    return true;
                }
            }
        }

        error = "Informe NotaId, OrdemId ou um par de ReferenciaId/TipoReferencia valido.";
        return false;
    }
    
    public enum TipoReferencia
    {
        Nota,
        Ordem
    }
}
