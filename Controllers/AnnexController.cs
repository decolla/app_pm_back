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

        if (request.ReferenciaId <= 0 || string.IsNullOrWhiteSpace(request.TipoReferencia))
        {
            return BadRequest(new { message = "ReferenciaId e tipoReferencia sao obrigatorios." });
        }

        var result = await _annexService.UploadAnexo(request.Arquivo, request.ReferenciaId, request.TipoReferencia);
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

        var list = await _annexService.BuscarAnexo(referenciaId, tipoReferencia);
        return Ok(list);
    }
}