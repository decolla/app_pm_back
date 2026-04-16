using app_pm_back.Data;
using app_pm_back.Models;
using app_pm_back.Models.CongasModel;

namespace app_pm_back.Services;

public class EquipmentService : BaseCrudService<EquipmentComgasModel>
{
    public EquipmentService(AppDbContext context) : base(context)
    {
    }

    public async Task CreateEquipamentosEmMassaAsync(List<EquipmentComgasModel> equipamentos)
    {
        var idsExistentes = _context.Equipamentos
            .Select(e => e.IdEquipamento)
            .ToHashSet();

        var novosEquipamentos = equipamentos
            .Where(e => !idsExistentes.Contains(e.IdEquipamento))
            .ToList();

        if (novosEquipamentos.Any())
        {
            await _context.AddRangeAsync(novosEquipamentos);
            await _context.SaveChangesAsync();
        }
    }
}