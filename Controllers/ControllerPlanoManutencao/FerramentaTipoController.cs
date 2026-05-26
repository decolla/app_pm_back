using app_pm_back.DTO.ComgasDTO;
using app_pm_back.Models.Domain;
using app_pm_back.Services.CongasServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace app_pm_back.Controllers.ControllerPlanoManutencao;

/// <summary>
/// Endpoints de ferramentas de tipo de manutencao.
/// </summary>
[ApiController]
[Route("api/V1/Comgas/ferramenta-tipo")]
public class FerramentaTipoController : ControllerBase
{
    private readonly FerramentaService _service;
    private readonly IMapper _mapper;

    public FerramentaTipoController(FerramentaService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] FerramentaTipoComgasDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var model = _mapper.Map<FerramentaModel>(dto);
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
        if (obj == null) return NotFound(new { message = "Ferramenta nao encontrada." });
        return Ok(obj);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] FerramentaTipoComgasDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (id <= 0) return BadRequest(new { message = "Id invalido na rota." });

        if (dto.IdFerramenta <= 0) dto.IdFerramenta = id;
        if (dto.IdFerramenta != id) return BadRequest(new { message = "Id da rota deve ser igual ao Id do corpo." });

        var model = _mapper.Map<FerramentaModel>(dto);
        var atualizado = await _service.UpdateNotaAsync(model);
        return Ok(atualizado);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deletar([FromRoute] int id)
    {
        var deleted = await _service.DeleteNotaAsync(id);
        if (!deleted) return NotFound(new { message = "Ferramenta nao encontrada." });
        return Ok(new { message = "Ferramenta removida com sucesso." });
    }
}
