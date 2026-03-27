using System.Reflection;
using app_pm_back.Data;
using app_pm_back.Interfaces;
using app_pm_back.Models;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services;

/// <summary>
/// Classe para classificar notas/ordens
/// Usando regras de palavras e catboost
/// É usado um tipo genérico para que possa ser usado em qualquer tipo de nota/ordem,
/// desde que ele herde de IClassifiable
/// </summary>
/// <typeparam name="T"></typeparam>
public class ClassificationService<T> where T : class, IClassifiable
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<RuleClassificationModel> _classificationModel;
    protected readonly DbSet<T> _dbSet;

    public ClassificationService(AppDbContext context)
    {
        _context = context;
        _classificationModel = context.Set<RuleClassificationModel>();
        _dbSet = context.Set<T>();
    }

    public async Task<T> HumanClassification(long id, string classificacao, string usuario)
    {
        var nota = await _dbSet.FindAsync(id);
        if (nota == null) return null;

        nota.Classificacao = classificacao;
        nota.ClassificadoPor = usuario;
        nota.DataClassificacao = DateTime.Now;
        nota.MetodoClassificacao = "MANUAL";

        _dbSet.Update(nota);
        await _context.SaveChangesAsync();
        return nota;
    }

    public async Task<RuleClassificationModel> CreateRule(RuleClassificationModel rule)
    {
        await _classificationModel.AddAsync(rule);
        await _context.SaveChangesAsync();
        return rule;
    }

    /// <summary>
    /// Função para classificar notas/ordens com base nas regras de palavras.
    /// A busca será feita no controller e passará todas que estiverem sem classificar,
    /// e quais deverão ser reclassificadas.
    /// </summary>
    /// <param name="regra"></param>
    /// <param name="notasSemClassficacao"></param>
    /// <param name="notasComClassificacao"></param>
    /// <returns></returns>
    public async Task<List<T>> WordsClassification(List<T>? notasComClassificacao)
    {
        // busco todas as notas/ordems sem classificação
        // remover comentário caso deve reavaliar regras já avaladas
        var notasSemClassificacao =
            _dbSet.Where(n => n.Classificacao == null /* || n.ClassificadoPor == null*/).ToList();

        // leio o campo alvo da regra e busco a palavra chave 
        List<RuleClassificationModel> regras = await _classificationModel.OrderBy(r => r.OrdemPrioridade).ToListAsync();

        // dicionário para armazenar um cache das propriedades
        Dictionary<string, PropertyInfo> cache = new Dictionary<string, PropertyInfo>();

        foreach (var regra in regras)
        {
            if (!cache.ContainsKey(regra.CampoAlvo))
            {
                var prop = typeof(T).GetProperty(regra.CampoAlvo);
                if (prop != null)
                {
                    cache[regra.CampoAlvo] = prop;
                }
            }
        }

        // caso a palavra exista atualiza a ficha com o método "FILTRO_PALAVRAS"
        foreach (var nota in notasSemClassificacao)
        {
            foreach (var regra in regras)
            {
                if (!cache.TryGetValue(regra.CampoAlvo, out var propriedadeAlvo))
                    continue;

                string palavraChave = regra.PalavraChave;

                string? valorAlvo = propriedadeAlvo.GetValue(nota)?.ToString();

                if (valorAlvo != null && valorAlvo.Contains(palavraChave, StringComparison.OrdinalIgnoreCase))
                {
                    nota.Classificacao = regra.ClassificacaoAplicada;
                    nota.ClassificadoPor = null;
                    nota.DataClassificacao = DateTime.Now;
                    nota.MetodoClassificacao = "FILTRO_PALAVRAS";
                    break;
                }
            }
        }

        _context.SaveChanges();
        return notasSemClassificacao;
    }

    public async Task<List<T>> CatbosstClassification(List<T> notasSemClassficacao, List<T> notasComClassificacao)
    {
        return null;
    }
}