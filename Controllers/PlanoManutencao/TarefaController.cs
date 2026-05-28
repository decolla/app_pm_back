using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services.Manutencao;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace app_pm_back.Controllers.PlanoManutencao;

/// <summary>
/// Endpoints de tarefas por tipo de manutencao.
/// </summary>
[ApiController]
[Route(app_pm_back.Config.RouteConfig.BaseRoute + "/tarefa-tipo-manutencao")]
public class TarefaController : ControllerBase
{
    private readonly TarefaService _service;
    private readonly IMapper _mapper;

    public TarefaController(TarefaService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] TarefaDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (!dto.IdTipoManutencao.HasValue || dto.IdTipoManutencao <= 0)
            return BadRequest(new { message = "IdTipoManutencao obrigatorio." });
        if (string.IsNullOrWhiteSpace(dto.Nome)) return BadRequest(new { message = "Nome da tarefa obrigatorio." });

        var model = _mapper.Map<OperacaoModel>(dto);
        var criado = await _service.CreateAsync(model);
        return Ok(criado);
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int? tipoManutencaoId)
    {
        if (tipoManutencaoId.HasValue)
        {
            var filtrado = await _service.BuscarPorTipoManutencaoAsync(tipoManutencaoId.Value);
            return Ok(filtrado);
        }

        return Ok(await _service.GetAllNotasAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId([FromRoute] int id)
    {
        var obj = await _service.GetNotaByIdAsync(id);
        if (obj == null) return NotFound(new { message = "Tarefa nao encontrada." });
        return Ok(obj);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] TarefaDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (id <= 0) return BadRequest(new { message = "Id invalido na rota." });

        if (dto.IdOperacao <= 0) dto.IdOperacao = id;
        if (dto.IdOperacao != id) return BadRequest(new { message = "Id da rota deve ser igual ao Id do corpo." });

        var model = _mapper.Map<OperacaoModel>(dto);
        var atualizado = await _service.UpdateNotaAsync(model);
        return Ok(atualizado);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deletar([FromRoute] int id)
    {
        var deleted = await _service.DeleteNotaAsync(id);
        if (!deleted) return NotFound(new { message = "Tarefa nao encontrada." });
        return Ok(new { message = "Tarefa removida com sucesso." });
    }
}