using app_pm_back.Data;
using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe OperacaoService.
/// </summary>
public class OperacaoService : BaseCrudService<OperacaoModel>
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="OperacaoService"/>.
    /// </summary>
    public OperacaoService(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtém um registro específico.
    /// </summary>
    /// <param name="tipoManutencaoId">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    public async Task<List<OperacaoModel>> BuscarPorTipoManutencaoAsync(long tipoManutencaoId)
    {
        return await _context.PM_Operacoes
            .Where(t => t.IdTipoManutencao == tipoManutencaoId)
            .OrderBy(t => t.IdOperacao)
            .ToListAsync();
    }
    
    /// <summary>
    /// Importa operações em lote de forma segura, garantindo o relacionamento com a Ordem.
    /// </summary>
    public async Task ImportarOperacoesEmLote(List<OperacaoModel> operacoes, bool sobrescrever, bool adicionarNovas, CancellationToken cancellationToken = default)
    {
        if (operacoes == null || !operacoes.Any()) return;

        var novasOperacoes = new List<OperacaoModel>();

        // Busca as operações que já existem no banco baseadas nas Ordens que estamos processando
        var idsOrdens = operacoes.Select(o => o.IdOrdem).Distinct().ToList();
        var operacoesExistentes = await _context.PM_Operacoes
            .Where(op => idsOrdens.Contains(op.IdOrdem))
            .ToListAsync(cancellationToken);

        foreach (var opPlanilha in operacoes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Verifica se a operação já existe para esta ordem (usando o Nome como critério)
            var existente = operacoesExistentes.FirstOrDefault(op => 
                op.IdOrdem == opPlanilha.IdOrdem && op.Nome == opPlanilha.Nome);

            if (existente != null)
            {
                if (sobrescrever)
                {
                    // Evita o erro de chave primária mantendo o Id real do banco
                    opPlanilha.IdOperacao = existente.IdOperacao;
                    _context.Entry(existente).CurrentValues.SetValues(opPlanilha);
                }
            }
            else if (adicionarNovas)
            {
                // Limpa a navegação para evitar que o EF tente salvar a Ordem pai novamente por engano
                opPlanilha.Ordem = null; 
                novasOperacoes.Add(opPlanilha);
            }
        }

        if (novasOperacoes.Any())
        {
            await _context.PM_Operacoes.AddRangeAsync(novasOperacoes, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<OperacaoModel>> ObterOperacoesSemOrdemAsync(int? pagina, int? tamanho)
    {
        int p = pagina ?? 1;
        int t = tamanho ?? 20;
        
        return await _context.PM_Operacoes 
            .Where(o => o.IdOrdem == null)
            .OrderBy(o => o.IdOperacao)
            .Skip((p - 1) * t).Take(t)
            .ToListAsync();
    }
    
    public async Task<OperacaoModel?> UpdateCompleteAsync(OperacaoModel operacao)
    {
        // limpa o change tracker
        _context.ChangeTracker.Clear();

        // busca a ordem no banco de dados
        var operacaoBanco = await _dbSet
            .Include(n => n.Ferramentas)
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.IdOperacao == operacao.IdOperacao);

        if (operacaoBanco == null)
            throw new Exception("Tarefa não encontrada.");

        // atualiza o cabeçalho da ordem
        _context.Entry(operacaoBanco).CurrentValues.SetValues(operacao);
        _context.Entry(operacaoBanco).State = EntityState.Modified;

        // sincroniza operações
        if (operacao.Ferramentas != null)
        {
            var idOperacoes = operacao.Ferramentas.Select(v => v.IdFerramenta).ToList();

            // deleta
            if (operacaoBanco.Ferramentas != null)
                foreach (var opBanco in operacaoBanco.Ferramentas.Where(v =>
                             !idOperacoes.Contains(v.IdFerramenta)))
                {
                    _context.Entry(opBanco).State = EntityState.Deleted;
                }
        }

        if (operacao.Ferramentas != null)
            foreach (var opTela in operacao.Ferramentas)
            {
                if (opTela.IdFerramenta == 0)
                {
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

        return operacaoBanco;
    }
    
    public async Task<OperacaoModel?> GetTarefaCompletaAsync(long idTarefa)
    {
        var tarefa = await _dbSet
            .Include(n => n.Ferramentas)
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.IdOperacao == idTarefa);

        if (tarefa == null) return null;

        return tarefa;
    }
    
}