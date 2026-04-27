using app_pm_back.DTO.ComgasDTO;
using app_pm_back.Models.CongasModel.PlanoManutencaoModels.ManutencaoModels;
using app_pm_back.Services.CongasServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace app_pm_back.Controllers.ControllerPlanoManutencao;

/// <summary>
/// Endpoints de itens de renovacao por tipo de manutencao.
/// </summary>
[ApiController]
[Route("api/V1/Comgas/item-renovacao-tipo")]
public class ItemRenovacaoTipoController : ControllerBase
{
    private readonly ItemRenovacaoTipoService _service;
    private readonly IMapper _mapper;

    public ItemRenovacaoTipoController(ItemRenovacaoTipoService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ItemRenovacaoTipoComgasDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (dto.TipoManutencaoId <= 0) return BadRequest(new { message = "TipoManutencaoId obrigatorio." });
        if (string.IsNullOrWhiteSpace(dto.CodigoItem)) return BadRequest(new { message = "CodigoItem obrigatorio." });

        var model = _mapper.Map<ItemRenovacaoTipoModel>(dto);
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
        if (obj == null) return NotFound(new { message = "Item de renovacao nao encontrado." });
        return Ok(obj);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] ItemRenovacaoTipoComgasDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (id <= 0) return BadRequest(new { message = "Id invalido na rota." });

        if (dto.IdItemRenovacaoTipo <= 0) dto.IdItemRenovacaoTipo = id;
        if (dto.IdItemRenovacaoTipo != id) return BadRequest(new { message = "Id da rota deve ser igual ao Id do corpo." });

        var model = _mapper.Map<ItemRenovacaoTipoModel>(dto);
        var atualizado = await _service.UpdateNotaAsync(model);
        return Ok(atualizado);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deletar([FromRoute] int id)
    {
        var deleted = await _service.DeleteNotaAsync(id);
        if (!deleted) return NotFound(new { message = "Item de renovacao nao encontrado." });
        return Ok(new { message = "Item de renovacao removido com sucesso." });
    }
}

