using app_pm_back.Services;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

/// <summary>
/// Controller para anexos
/// </summary>
[ApiController]
[Route("api/annex")]
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
    /// <param name="arquivo">Arquivo a ser passado</param>
    /// <param name="referenciaId">Id de referência</param>
    /// <param name="tipoReferencia">Tipo de referência a ser usada</param>
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
    /// Download de um arquivo com base no id
    /// </summary>
    /// <param name="id">Id regsitrado no arquivo</param>
    /// <returns></returns>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> DownloadArquivo(long id)
    {
        var anexo = await _annexService.DownloadAnexo(id);
        if (anexo == null || anexo.Dados == null) return NotFound();

        return File(anexo.Dados, anexo.MimeType, anexo.NomeAnexo);
    }
    
    /// <summary>
    /// Realiza a busca de um arquivo no banco de dados com base no Id e Tipo relacionados 
    /// </summary>
    /// <param name="referenciaId">Id da referência de busca</param>
    /// <param name="tipoReferencia">Tipo da referência de busca</param>
    /// <returns></returns>
    [HttpGet("referencia/{referenciaId:long}/{tipoReferencia}")]
    public async Task<IActionResult> BuscarArquivo(long referenciaId, string tipoReferencia)
    {
        var list = await _annexService.BuscarAnexo(referenciaId, tipoReferencia);
        
        if (list == null) return NotFound();
        return Ok(list);
    }
}