using app_pm_back.DTO.Buscas;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using app_pm_back.Services.GenericServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace app_pm_back.Controllers.FichasControllers;

[ApiController]
[Route(Config.RouteConfig.BaseRoute + "/diretorios")]
[Authorize(Policy = "View")]
public class DiretoriosController : ControllerBase
{
    private readonly DiretorioService _diretorioService;
    private readonly GenericSearchService _searchService;

    public DiretoriosController(DiretorioService diretorioService, GenericSearchService searchService)
    {
        _diretorioService = diretorioService;
        _searchService = searchService;
    }

    /// <summary>
    /// Obtém a árvore completa de diretórios
    /// </summary>
    /// <returns>Lista em formato de árvore</returns>
    [HttpGet]
    [Route("arvore")]
    public async Task<IActionResult> ObterArvore()
    {
        try
        {
            var arvore = await _diretorioService.ObterArvoreAsync();
            return Ok(arvore);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "Ocorreu um erro ao obter a árvore de diretórios.", details = ex.Message });
        }
    }

    /// <summary>
    /// Cria um novo diretório
    /// </summary>
    /// <param name="nome">Nome do diretório</param>
    /// <param name="idDiretorioPai">ID do diretório pai</param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = "Edit")]
    [Route("criar")]
    public async Task<IActionResult> CriarDiretorio([FromQuery] string nome, [FromQuery] long? idDiretorioPai)
    {
        try
        {
            var diretorio = await _diretorioService.CriarDiretorioAsync(nome, idDiretorioPai);
            return Ok(diretorio);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "Ocorreu um erro ao criar o diretório.", details = ex.Message });
        }
    }

    /// <summary>
    /// Move um diretório para outro
    /// </summary>
    /// <param name="idDiretorio">ID do diretório a ser movido</param>
    /// <param name="idDiretorioPai">ID do diretório de destino</param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = "Edit")]
    [Route("mover")]
    public async Task<IActionResult> MoverDiretorio([FromQuery] long idDiretorio, [FromQuery] long? idDiretorioPai)
    {
        try
        {
            var diretorio = await _diretorioService.MoverDiretorioAsync(idDiretorio, idDiretorioPai);
            return Ok(diretorio);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "Ocorreu um erro ao mover o diretório.", details = ex.Message });
        }
    }

    /// <summary>
    /// Vincula uma ordem ou nota a um diretório
    /// </summary>
    /// <param name="idDiretorio">ID do diretório a ser vinculado</param>
    /// <param name="idFicha">ID da ficha a ser vinculada</param>
    /// <param name="tipo">Tipo da ficha (Notas ou Ordens)</param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = "Edit")]
    [Route("vincular")]
    public async Task<IActionResult> VincularFicha(
        [FromQuery] long idDiretorio,
        [FromQuery] long idFicha,
        [FromQuery] Fichas tipo)
    {
        try
        {
            if (tipo == Fichas.Notas)
            {
                var resultado = await _diretorioService.VincularNotaAoDiretorioAsync(idDiretorio, idFicha);
                return Ok(resultado);
            }
            else if (tipo == Fichas.Ordens)
            {
                var resultado = await _diretorioService.VincularOrdemAoDiretorioAsync(idDiretorio, idFicha);
                return Ok(resultado);
            }
            else return BadRequest(new { message = "Tipo de ficha invalido." });
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "Ocorreu um erro ao vincular o ativo ao diretório.", details = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza o nome de um diretório 
    /// </summary>
    /// <param name="idDiretorio">ID do diretório a ser atualizado</param>
    /// <param name="nome">Novo nome do diretório</param>
    /// <returns></returns>
    [HttpPut]
    [Authorize(Policy = "Edit")]
    [Route("atualizar")]
    public async Task<IActionResult> AtualizarDiretorio([FromQuery] long idDiretorio, [FromQuery] string nome)
    {
        try
        {
            var diretorio = await _diretorioService.AtualizarDiretorioAsync(idDiretorio, nome);
            return Ok(diretorio);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "Ocorreu um erro ao atualizar o diretório.", details = ex.Message });
        }
    }

    /// <summary>
    /// Remove um diretório da árvore
    /// </summary>
    /// <param name="idDiretorio">ID do diretório a ser removido</param>
    /// <returns></returns>
    [HttpDelete]
    [Authorize(Policy = "Edit")]
    [Route("remover")]
    public async Task<IActionResult> RemoverDiretorio([FromQuery] long idDiretorio)
    {
        try
        {
            await _diretorioService.RemoverDiretorioAsync(idDiretorio);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "Ocorreu um erro ao remover o diretório.", details = ex.Message });
        }
    }

    /// <summary>
    /// Remove a conexão entre uma nota/ordem a um diretório
    /// </summary>
    /// <param name="idDiretorio">ID do diretório</param>
    /// <param name="idFicha">ID da ficha</param>
    /// <param name="tipo">Tipo da ficha (Notas ou Ordens)</param>
    /// <returns></returns>
    [HttpDelete]
    [Authorize(Policy = "Edit")]
    [Route("remover/ficha")]
    public async Task<IActionResult> DesvincularFicha(
        [FromQuery] long idDiretorio,
        [FromQuery] long idFicha,
        [FromQuery] Fichas tipo)
    {
        try
        {
            if (tipo == Fichas.Notas)
            {
                await _diretorioService.RemoverNotaDoDiretorioAsync(idDiretorio, idFicha);
                return Ok();
            }
            else if (tipo == Fichas.Ordens)
            {
                await _diretorioService.RemoverOrdemDoDiretorioAsync(idDiretorio, idFicha);
                return Ok();
            }
            else return BadRequest(new { message = "Tipo de ficha invalido." });
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "Ocorreu um erro ao desvincular a ficha do diretório.", details = ex.Message });
        }
    }

    /// <summary>
    /// Obtém todas as folhas (notas e ordens) vinculadas a um diretório específico
    /// </summary>
    /// <param name="idDiretorio">ID do diretório de busca</param>
    /// <returns></returns>
    [HttpGet]
    [Route("folhas")]
    public async Task<IActionResult> ObterFolhasDoDiretorio([FromQuery] long idDiretorio)
    {
        try
        {
            var folhas = await _diretorioService.ObterFolhasDoDiretorioAsync(idDiretorio);
            return Ok(folhas);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "Ocorreu um erro ao obter as folhas do diretório.", details = ex.Message });
        }
    }

    [HttpGet("fichas")]
    public async Task<IActionResult> ObterFichasDiretorio([FromQuery] long idDiretorio, [FromQuery] Fichas tipo,
        [FromQuery] int pagina, [FromQuery] int tamanho, [FromQuery] BuscaDiretorioDto? busca)
    {
        try
        {
            List<object> fichas = new List<object>();

            if (tipo == Fichas.Notas)
            {
                // TODO: melhorar essa parte, obter e filtrar ao mesmo tempo, com paginação já aplicada
                var notas = await _diretorioService.ObterNotasDiretorioAsync(idDiretorio);

            }else if (tipo == Fichas.Ordens)
            {
                
            }
            else
            {
                
            }
            
            
            return Ok(fichas);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "Ocorreu um erro ao obter as fichas do diretório.", details = ex.Message });
        }
    }

    /// <summary>
    /// Obtém todos os diretórios cadastrados em banco
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("todos")]
    public async Task<IActionResult> ObterTodosDiretorios()
    {
        try
        {
            var diretorios = await _diretorioService.ObterTodosDiretoriosAsync();
            return Ok(diretorios);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "Ocorreu um erro ao obter todos os diretórios.", details = ex.Message });
        }
    }
}