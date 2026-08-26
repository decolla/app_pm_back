using app_pm_back.Data;
using app_pm_back.DTO;
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

        var todasRegras = await _dbSet.AsNoTracking().ToListAsync();

        return new
        {
            regrasCorretivasPermitidas = todasRegras.Where(n => n.ClassificacaoAplicada == Classificacao.CORRETIVA && n.Permitida),
            regrasCorretivasProibidas = todasRegras.Where(n => n.ClassificacaoAplicada == Classificacao.CORRETIVA && !n.Permitida),
            regrasPreventivasPermitidas = todasRegras.Where(n => n.ClassificacaoAplicada == Classificacao.PREVENTIVA && n.Permitida),
            regrasPreventivasProibidas = todasRegras.Where(n => n.ClassificacaoAplicada == Classificacao.PREVENTIVA && !n.Permitida)
        };
    }

    /// <summary>
    /// Cadastra ou atualiza as palavras
    /// </summary>
    /// <param name="palavras">Lista de palavras a serem cadastradas</param>
    /// <returns>Lista de palavras cadastradas</returns>
    public async Task<List<RegraClassificacaoDto>> CadastrarPalavrasAsync(List<RegraClassificacaoDto> palavras)
    {
        _context.ChangeTracker.Clear();
        
        // ids recebidos
        var idsFrontend = palavras.Select(p => p.IdRegra).ToList();

        // estado atual
        var regrasBanco = await _dbSet.AsNoTracking().ToListAsync();

        // remove o que não foi recebido
        foreach (var itemBanco in regrasBanco.Where(i => !idsFrontend.Contains(i.IdRegra)))
        {
            _context.Entry(itemBanco).State = EntityState.Deleted;
        }

        // mapeia e processa atualizações
        foreach (var itemTela in palavras)
        {
            // realiza mapeamento
            var model = new RegraClassificacaoModel
            {
                IdRegra = itemTela.IdRegra,
                PalavraChave = itemTela.PalavraChave,
                ClassificacaoAplicada = itemTela.ClassificacaoAplicada,
                Permitida = itemTela.Permitida,
                OrdemPrioridade = itemTela.OrdemPrioridade
            };

            if (model.IdRegra == 0)
            {
                _context.Entry(model).State = EntityState.Added;
            }
            else
            {
                _context.Entry(model).State = EntityState.Modified;
            }
        }

        await _context.SaveChangesAsync();

        return palavras;
    }
}