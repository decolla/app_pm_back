using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

public class TarefaService : BaseCrudService<OperacaoModel>
{
    public TarefaService(AppDbContext context) : base(context)
    {
    }

    public async Task<List<OperacaoModel>> BuscarPorTipoManutencaoAsync(int tipoManutencaoId)
    {
        return await _context.PM_Operacoes
            .Where(t => t.IdTipoManutencao == tipoManutencaoId)
            .OrderBy(t => t.IdOperacao)
            .ToListAsync();
    }
}
