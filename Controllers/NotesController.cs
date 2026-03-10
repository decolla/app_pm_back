using app_pm_back.DTO;
using app_pm_back.Models;
using app_pm_back.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

[ApiController]
[Route("api/notes")]
public class NotesController : ControllerBase
{
    private readonly NotesPetroService _petroService;
    private readonly IMapper _mapper;

    public NotesController(NotesPetroService petroService, IMapper mapper)
    {
        _petroService = petroService;
        _mapper = mapper;
    }

    /// <summary>
    /// Cadastra uma nota pelo Dto
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CriarNota([FromBody] CreateNotaPetroDto dto)
    {
        try
        {
            var novaNota = _mapper.Map<NotaPetroModel>(dto);
            var notaCriada = await _petroService.CreateNotaAsync(novaNota);

            return Created($"api/notas/{notaCriada.IdNota}", notaCriada);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao criar a nota: {e.Message}." });
        }
    }

    /// <summary>
    /// Retorna uma lista todas as notas com sistema de páginas
    /// </summary>
    /// <param name="pagina"></param>
    /// <param name="tamanho"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> ListarNotas([FromQuery] int? pagina, [FromQuery] int? tamanho)
    {
        try
        {
            if (pagina.HasValue || tamanho.HasValue)
            {
                int p = pagina ?? 1;
                int t = tamanho ?? 20;
                var notasPaginadas = await _petroService.GetNotasPaginadasAsync(p, t);
                return Ok(notasPaginadas);
            }

            var todasNotas = await _petroService.GetAllNotasAsync();
            return Ok(todasNotas);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = "Erro ao processar notas." });
        }
    }

    /// <summary>
    /// Conta a quantidade de notas no banco
    /// </summary>
    /// <returns></returns>
    [HttpGet("count")]
    public async Task<IActionResult> ContarNotas()
    {
        try
        {
            var notas = await _petroService.CountNotasAsync();
            return Ok(new { count = notas });
        }
        catch (Exception e)
        {
            return NotFound(new { message = "Erro ao procurar notas." });
        }
    }
    
    /// <summary>
    /// Retorna uma lista todas as notas com sistema de páginas
    /// </summary>
    /// <param name="active"></param>
    /// <param name="tipo"></param>
    /// <returns></returns>
    [HttpGet("active/{active}")]
    public async Task<IActionResult> ListarFichasPorAtivo(string active, [FromQuery] string? tipo)
    {
        try
        {
            var resultado = await _petroService.GetFichasPorAtivoAsync(active, tipo);
            return Ok(resultado);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = "Erro ao buscar fichas do ativo." });
        }
    }

    /// <summary>
    /// Atualiza uma nota de mesmo Id
    /// </summary>
    /// <param name="nota"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> AtualizarNota([FromBody] NotaPetroModel nota)
    {
        try
        {
            var result = await _petroService.UpdateNotaAsync(nota);
            return Ok(new { message = "Nota atualizada com sucesso: " + result.IdNota });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = "Erro ao atualizar nota." });
        }
    }

    /// <summary>
    /// Delete uma nota de mesmo Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletarNota(int id)
    {
        try
        {
            var result = await _petroService.DeleteNotaAsync(id);
            return Ok(new { message = "Nota deletada com sucesso" });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = "Erro ao deletar nota." });
        }
    }
}