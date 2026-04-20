using app_pm_back.Data;
using app_pm_back.Models.Domain;

namespace app_pm_back.Services;

/// <summary>
/// Servico de CRUD e operacoes em lote para equipamentos.
/// </summary>
public class EquipmentService : BaseCrudService<EquipamentoAtivoModel>
{
    /// <summary>
    /// Construtor.
    /// </summary>
    public EquipmentService(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Cria equipamentos em massa ignorando tags ja existentes.
    /// </summary>
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

    /// <summary>
    /// Remove um equipamento pela tag (chave primaria textual).
    /// </summary>
    public async Task<bool> DeleteByTagAsync(string tagEquipamento)
    {
        var entity = await _context.EquipamentosAtivos.FindAsync(tagEquipamento);
        if (entity == null)
        {
            return false;
        }

        _context.EquipamentosAtivos.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }
}