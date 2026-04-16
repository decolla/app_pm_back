using app_pm_back.Data;
using app_pm_back.Models;
using app_pm_back.Models.CongasModel;
using app_pm_back.Models.Domain;

namespace app_pm_back.Services;

public class EquipmentService : BaseCrudService<EquipamentoAtivoModel>
{
    public EquipmentService(AppDbContext context) : base(context)
    {
    }

    public async Task CreateEquipamentosEmMassaAsync(List<EquipamentoAtivoModel> equipamentos)
    {
        var idsExistentes = _context.EquipamentosAtivos
            .Select(e => e.TagEquipamento)
            .ToHashSet();

        var novosEquipamentos = equipamentos
            .Where(e => !idsExistentes.Contains(e.TagEquipamento))
            .ToList();

        if (novosEquipamentos.Any())
        {
            await _context.AddRangeAsync(novosEquipamentos);
            await _context.SaveChangesAsync();
        }
    }
}