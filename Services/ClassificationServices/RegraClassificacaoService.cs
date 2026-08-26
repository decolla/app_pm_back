using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.FichasServices;

public class RegraClassificacaoService : BaseCrudService<RegraClassificacaoModel>
{
    public RegraClassificacaoService(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtém todas as regras divididas por sua classificação e permissão
    /// </summary>
    /// <returns>Objeto com fichas divididas</returns>
    public async Task<object> ObterRegrasDivididasAsync()
    {
        var query = _dbSet.AsQueryable();

        // filtra regras preventivas e corretivas
        var fichasCorretivas = await query
            .Where(n => n.ClassificacaoAplicada == Classificacao.CORRETIVA)
            .ToListAsync();

        var fichasPreventivas = await query
            .Where(n => n.ClassificacaoAplicada == Classificacao.PREVENTIVA)
            .ToListAsync();

        // filtra as regras dado a permissão
        var regrasCorretivasPermitidas = fichasCorretivas.Where(n => n.Permitida);
        var regrasCorretivasProibidas = fichasCorretivas.Where(n => !n.Permitida);

        var regrasPreventivasPermitidas = fichasPreventivas.Where(n => n.Permitida);
        var regrasPreventivasProibidas = fichasPreventivas.Where(n => !n.Permitida);

        return new
        {
            regrasCorretivasPermitidas,
            regrasCorretivasProibidas,
            regrasPreventivasPermitidas,
            regrasPreventivasProibidas
        };
    }
}