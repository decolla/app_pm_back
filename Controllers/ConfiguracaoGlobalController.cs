using app_pm_back.DTO;
using app_pm_back.Interfaces;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using app_pm_back.Services.FichasServices;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers;

/// <summary>
/// Controller para configurações globais do sistema.
/// </summary>
[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/configuracao-global")]
public class ConfiguracaoGlobalController : ControllerBase
{
    private readonly ConfiguracaoGlobalService _configuracaoService;
    private readonly IUserContext _userContext;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="ConfiguracaoGlobalController"/>.
    /// </summary>
    public ConfiguracaoGlobalController(ConfiguracaoGlobalService configuracaoService, IUserContext userContext)
    {
        _userContext = userContext;
        _configuracaoService = configuracaoService;
    }

    /// <summary>
    /// Obtém a configuração global do sistema (linha única).
    /// </summary>
    /// <returns>O objeto de configuração global cadastrado.</returns>
    /// <response code="200">Sucesso ao obter a configuração global.</response>
    /// <response code="400">Erro ao processar a requisição.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetConfiguracao()
    {
        try
        {
            var config = await _configuracaoService.ObterConfiguracaoAsync();
            return Ok(config);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao obter configuração global: {e.Message}" });
        }
    }

    /// <summary>
    /// Salva/atualiza a configuração global do sistema.
    /// </summary>
    /// <param name="dto">DTO contendo os novos dados de configuração global.</param>
    /// <returns>Mensagem de confirmação da atualização.</returns>
    /// <response code="200">Configuração global salva com sucesso.</response>
    /// <response code="400">Erros na validação do modelo ou dados fornecidos inválidos.</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateConfiguracao([FromBody] ConfiguracaoGlobalDto dto)
    {
        try
        {
            var usuarioId = _userContext.GetUsuarioIdAtual()?.ToString() ?? "Sistema";
            await _configuracaoService.SalvarConfiguracaoAsync(dto, usuarioId);
            
            // // instancia automaticamente a classificação automática em segundo plano se a configuração permitir
            // if (dto.ClassificacaoAutomatica != ModoClassificacaoAutomatica.NaoClassificar)
            // {
            //     BackgroundJob.Enqueue<ClassificationService<NotaModel>>(service => service.AutoClassify());
            //     BackgroundJob.Enqueue<ClassificationService<OrdemModel>>(service => service.AutoClassify());
            // }
            
            return Ok(new { message = "Configuração global salva com sucesso. A reclassificação foi iniciada em segundo plano." });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = $"Erro ao salvar configuração global: {e.Message}" });
        }
    }
}
