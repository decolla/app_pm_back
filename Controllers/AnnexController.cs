using app_pm_back.Models;
using app_pm_back.Services;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

[ApiController]
[Route("api/annex")]
public class AnnexController : ControllerBase
{
    private readonly AnnexService _annexService;

    public AnnexController(AnnexService annexService)
    {
        _annexService = annexService;
    }

    /// <summary>
    /// Upload de um arquivo
    /// </summary>
    /// <param name="arquivo"></param>
    /// <param name="referenciaId"></param>
    /// <param name="tipoReferencia"></param>
    /// <returns></returns>
    [HttpPost("upload")]
    public async Task<IActionResult> UploadArquivo(IFormFile arquivo, [FromForm] long referenciaId,
        [FromForm] string tipoReferencia)
    {
        var result = await _annexService.UploadAnexo(arquivo, referenciaId, tipoReferencia);
        if (result == null) return BadRequest(new { message = "Erro ao enviar arquivo" });
        return Ok(new { message = $"Arquivo enviado com sucesso {result.IdAnexo}" });
    }

    /// <summary>
    /// Download de um arquivo
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> DownloadArquivo(long id)
    {
        var anexo = await _annexService.DownloadAnexo(id);
        if (anexo == null || anexo.Dados == null) return NotFound();

        return File(anexo.Dados, anexo.MimeType, anexo.NomeAnexo);
    }

    [HttpGet("referencia/{referenciaId:long}/{tipoReferencia}")]
    public async Task<IActionResult> BuscarArquivo(long referenciaId, string tipoReferencia)
    {
        var list = await _annexService.BuscarAnexo(referenciaId, tipoReferencia);
        
        if (list == null) return NotFound();
        return Ok(list);
    }
}