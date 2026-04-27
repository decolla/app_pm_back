using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

/// <summary>
/// Servico de regras e consultas de estrategia de manutencao.
/// </summary>
public class EstrategiaComgasService : BaseCrudService<EstrategiaManutencaoModel>
{
    /// <summary>
    /// Inicializa o servico de estrategia.
    /// </summary>
    public EstrategiaComgasService(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Busca estrategias por filtros opcionais da tela de pesquisa.
    /// </summary>
    /// <param name="id">Id da estratégia.</param>
    /// <param name="nome">Nome (busca parcial).</param>
    /// <param name="tipoAtivo">Tipo de ativo aplicavel (busca parcial).</param>
    /// <param name="visao">Visao/denominacao (busca parcial).</param>
    /// <param name="pagina">Pagina desejada (opcional).</param>
    /// <param name="tamanho">Quantidade por pagina (opcional).</param>
    public async Task<List<EstrategiaManutencaoModel>> BuscarAsync(
        int? id,
        string? nome,
        string? tipoAtivo,
        string? visao,
        int? pagina,
        int? tamanho)
    {
        var query = _context.Set<EstrategiaManutencaoModel>()
            .AsNoTracking()
            .AsQueryable();

        if (id.HasValue)
        {
            query = query.Where(e => e.IdEstrategia == id.Value);
        }

        if (!string.IsNullOrWhiteSpace(nome))
        {
            var nomeNormalizado = nome.Trim().ToLower();
            query = query.Where(e => e.Nome.ToLower().Contains(nomeNormalizado));
        }

        if (!string.IsNullOrWhiteSpace(tipoAtivo))
        {
            var tipoAtivoNormalizado = tipoAtivo.Trim().ToLower();
            query = query.Where(e => e.TipoAtivoAplicavel.ToLower().Contains(tipoAtivoNormalizado));
        }

        if (!string.IsNullOrWhiteSpace(visao))
        {
            var visaoNormalizada = visao.Trim().ToLower();
            query = query.Where(e => e.Denominacao != null && e.Denominacao.ToLower().Contains(visaoNormalizada));
        }

        query = query.OrderBy(e => e.IdEstrategia);

        if (pagina.HasValue || tamanho.HasValue)
        {
            var p = pagina ?? 1;
            var t = tamanho ?? 20;
            query = query.Skip((p - 1) * t).Take(t);
        }

        return await query.ToListAsync();
    }
}