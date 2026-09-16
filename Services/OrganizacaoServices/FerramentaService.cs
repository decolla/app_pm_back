using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe FerramentaService.
/// </summary>
public class FerramentaService : BaseCrudService<FerramentaModel>
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="FerramentaService"/>.
    /// </summary>
    public FerramentaService(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtém um registro específico.
    /// </summary>
    /// <param name="tipoManutencaoId">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    public async Task<List<FerramentaModel>> BuscarPorTipoManutencaoAsync(long tipoManutencaoId, int? pagina, int? tamanho)
    {
        int p = pagina ?? 1;
        int t = tamanho ?? 20;
        
        return await _context.Set<FerramentaModel>()
            .Where(f => f.Operacoes.Any(o => o.IdTipoManutencao == tipoManutencaoId))
            .OrderBy(f => f.IdFerramenta)
            .Skip((p - 1) * t)
            .Take(t)
            .ToListAsync();
    }
}