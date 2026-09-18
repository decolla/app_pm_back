using System.Diagnostics.Eventing.Reader;
using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe EstrategiaService.
/// </summary>
public class EstrategiaService : BaseCrudService<EstrategiaModel>
{
    private readonly DbSet<TipoManutencaoModel> _tipoManutencaoModels;
    private readonly DbSet<EstrategiaModel> _estrategiaModels;
    private readonly DbSet<TipoEstrategiaModel> _tipoEstrategiaModels;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="EstrategiaService"/>.
    /// </summary>
    public EstrategiaService(AppDbContext context) : base(context)
    {
        _tipoManutencaoModels = context.PM_TiposManutencao;
        _estrategiaModels = context.PM_Estrategias;
        _tipoEstrategiaModels = context.PM_TiposEstrategia;
    }

    /// <summary>
    /// Obtém um registro específico.
    /// </summary>
    /// <returns>Resultado da operação.</returns>
    public async Task<List<EstrategiaModel>> BuscarAsync(
        long? id,
        string? nome,
        string? tipoAtivo,
        string? visao,
        int? pagina,
        int? tamanho)
    {
        var query = _context.PM_Estrategias
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
            query = query.Where(e => e.TipoLi != null && e.TipoLi.ToString()!.ToLower().Contains(tipoAtivoNormalizado));
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

    /// <summary>
    /// Cria um tipo manutenção, tipo estratégia e uma estratégia para cada tipo de LI
    /// </summary>
    public async Task<object?> GerarTemplates()
    {
        try
        {
            foreach (TiposLI tipo in Enum.GetValues<TiposLI>())
            {
                var novoTManutencao = new TipoManutencaoModel
                {
                    Nome = $"mock{tipo}",
                    TipoLI = tipo,
                    Categoria = CategoriaTipoManutencao.Renovacao,
                    Confiabilidade = 1.0,
                    Habilitada = true
                };
                _tipoManutencaoModels.Add(novoTManutencao);

                var novaEstrategia = new EstrategiaModel
                {
                    Nome = $"mock{tipo}",
                    TipoLi = tipo,
                    Habilitada = true,
                    Ciclos = 7300,
                    CicloContinuo = true,
                    NumeroCiclos = 1
                };
                _estrategiaModels.Add(novaEstrategia);

                await _context.SaveChangesAsync();

                var novoTEstrategia = new TipoEstrategiaModel
                {
                    Categoria = CategoriaTipoManutencao.Renovacao,
                    Dias = 365,
                    ToleranciaPositiva = 5,
                    ToleranciaNegativa = 5,
                    Continua = true,
                    IdEstrategia = novaEstrategia.IdEstrategia,
                    IdTipoManutencao = novoTManutencao.IdTipoManutencao
                };
                _tipoEstrategiaModels.Add(novoTEstrategia);
            }

            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    public override async Task<EstrategiaModel?> GetByIdAsync(long id)
    {
        EstrategiaModel? estrategia = await _dbSet
            .Include(e => e.TiposEstrategia)
            .FirstOrDefaultAsync(e => e.IdEstrategia == id);

        return estrategia;
    }

    public async Task<EstrategiaModel?> UpdateCompleteAsync(EstrategiaModel estrategia)
    {
        // limpa o change tracker
        _context.ChangeTracker.Clear();

        // busca a ordem no banco de dados
        var estrategiaBanco = await _dbSet
            .Include(n => n.TiposEstrategia)
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.IdEstrategia == estrategia.IdEstrategia);

        if (estrategiaBanco == null)
            throw new Exception("Estratégia não encontrada.");

        // atualiza o cabeçalho da ordem
        _context.Entry(estrategiaBanco).CurrentValues.SetValues(estrategia);
        _context.Entry(estrategiaBanco).State = EntityState.Modified;

        // sincroniza operações
        if (estrategia.TiposEstrategia != null)
        {
            var idOperacoes = estrategia.TiposEstrategia.Select(v => v.IdTipoManutencao).ToList();

            // deleta
            if (estrategiaBanco.TiposEstrategia != null)
                foreach (var opBanco in estrategiaBanco.TiposEstrategia.Where(v =>
                             !idOperacoes.Contains(v.IdTipoManutencao)))
                {
                    _context.Entry(opBanco).State = EntityState.Deleted;
                }
        }

        if (estrategia.TiposEstrategia != null)
            foreach (var opTela in estrategia.TiposEstrategia)
            {
                if (opTela.IdTipoManutencao == 0)
                {
                    // caso de nova variável
                    opTela.IdEstrategia = estrategiaBanco.IdEstrategia;
                    _context.Entry(opTela).State = EntityState.Added;
                }
                else
                {
                    // caso de update
                    _context.Entry(opTela).State = EntityState.Modified;
                }
            }

        // salva bb
        await _context.SaveChangesAsync();

        return estrategiaBanco;
    }

    public async Task<List<PlanoModel>> GeneratePlanosAsync(long idEstrategia, List<string> sapNames)
    {
        if (sapNames == null || sapNames.Count == 0)
        {
            throw new ArgumentException("A lista de nomes SAP não pode ser nula ou vazia.", nameof(sapNames));
        }
        if(GetByIdAsync(idEstrategia).Result == null)
        {
            throw new ArgumentException("A estratégia não foi encontrada.", nameof(idEstrategia));
        }
        var planos = new List<PlanoModel>();
        var planosExistentes = await _context.PM_Planos
            .Where(p => p.IdEstrategia == idEstrategia && sapNames.Contains(p.IdLi))
            .Select(p => p.IdLi)
            .ToListAsync();

        foreach (var sap in sapNames)
        {
            // cria um plano para cada nome SAP, se não existir
            var plano = new PlanoModel
            {
                Nome = "Plano de Manutenção gerado: " + sap,
                Prioridade = 0,
                DataInicios = DateTime.Now,
                GerarAutomaticamente = GerarAutomaticamente.Ordem,
                Antecedencia = 5,
                DataConclusao = DateTime.Now.AddMonths(6),
                IdEstrategia = idEstrategia,
                IdLi = sap
            };
            
            if(planosExistentes.Contains(sap))
            {
                continue; // pula a criação do plano se já existir
            }
            
            planos.Add(plano);
            
            await _context.PM_Planos.AddAsync(plano);
        }
        await _context.SaveChangesAsync();
        return planos;
    }
}