using System.Globalization;
using System.Reflection;
using System.Text;
using app_pm_back.Data;
using app_pm_back.DTO;
using app_pm_back.Interfaces;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;
using teste_catboost_cs;

namespace app_pm_back.Services;

/// <summary>
/// Classe para classificar notas/ordens usando regras de palavras e catboost.
/// </summary>
/// <typeparam name="T">Tipo genérico para qualquer parâmetro classificável (nota ou ordem)</typeparam>
public class ClassificationService<T> where T : class, IClassifiable
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly DbSet<RuleClassificationDto> _classificationModel;
    protected readonly Cbm? _cbm;
    protected readonly ModelMeta? _meta;
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="ClassificationService{T}"/>.
    /// </summary>

    public ClassificationService(AppDbContext context, Cbm? modeloCatboost = null, ModelMeta? meta = null)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _classificationModel = context.Set<RuleClassificationDto>();
        _cbm = modeloCatboost;
        _meta = meta;
    }

    /// <summary>
    /// Classificação manual de notas/ordens.
    /// </summary>
    public async Task<T?> HumanClassification(long id, Classificacao classificacao, string usuario)
    {
        var nota = await _dbSet.FindAsync(id);
        if (nota == null) return null;

        nota.Classificacao = classificacao;
        nota.ClassificadoPor = usuario;
        nota.DataClassificacao = DateTime.Now;
        nota.MetodoClassificacao = MetodoClassificacao.HUMANA;

        _dbSet.Update(nota);
        await _context.SaveChangesAsync();
        return nota;
    }
    
    /// <summary>
    /// Cria uma regra de palavras
    /// </summary>
    /// <param name="rule">Regra no formato de RuleClassificationModel</param>
    /// <returns>Regra passada como parâmetro</returns>
    public async Task<RuleClassificationDto> CreateRule(RuleClassificationDto rule)
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
    /// <param name="notasComClassificacao"> Lista de tipo genérico de notas/ordens já classificadas para serem analisadas pelo algoritmo</param>
    /// <returns>Todas as notas que não tinham sido classificadas juntamente as reclassificadas</returns>
    public async Task<List<T>> WordsClassification(List<T>? notasComClassificacao)
    {
        // busco todas as notas/ordems sem classificação
        // remover comentário caso deve reavaliar regras já avaladas
        var notasSemClassificacao =
            await _dbSet.Where(n => n.Classificacao == Classificacao.NAOCLASSIFICADA).ToListAsync();
        DateTimeOffset dataOffset = DateTimeOffset.FromUnixTimeMilliseconds(10000);

        // leio o campo alvo da regra e busco a palavra chave 
        List<RuleClassificationDto> regras = await _classificationModel.OrderBy(r => r.OrdemPrioridade).ToListAsync();

        // dicionário para armazenar um cache das propriedades
        Dictionary<string, PropertyInfo> cache = new Dictionary<string, PropertyInfo>();

        foreach (var regra in regras)
        {
            // verifica se a regra está ou não na lista de regras
            if (!cache.ContainsKey(regra.CampoAlvo))
            {
                // pega a propriedade da nota por Reflection
                var prop = typeof(T).GetProperty(regra.CampoAlvo);
                if (prop != null)
                {
                    // adiciona a propriedade ao cache
                    cache[regra.CampoAlvo] = prop;
                }
            }
        }

        // caso a palavra exista atualiza a ficha com o método "FILTRO_PALAVRAS"
        foreach (var nota in notasSemClassificacao)
        {
            foreach (var regra in regras)
            {
                // captura a propriedade alvo da nota e a palavra chave
                if (!cache.TryGetValue(regra.CampoAlvo, out var propriedadeAlvo))
                    continue;

                string palavraChave = regra.PalavraChave;

                string? valorAlvo = propriedadeAlvo.GetValue(nota)?.ToString();

                // verifica se a palavra chave existe no valor alvo
                if (valorAlvo != null && valorAlvo.Contains(palavraChave, StringComparison.OrdinalIgnoreCase))
                {
                    nota.Classificacao = regra.ClassificacaoAplicada;
                    nota.ClassificadoPor = null;
                    nota.DataClassificacao = DateTime.Now;
                    nota.MetodoClassificacao = MetodoClassificacao.ALGORITMO;
                    break;
                }
            }
        }

        await _context.SaveChangesAsync();

        // retorna as notas/ordens atualizadas
        return notasSemClassificacao.Concat(notasComClassificacao ?? []).ToList();
    }

    /// <summary>
    /// Classificação de notas/ordens com base no modelo Catboost.
    /// </summary>
    public async Task<List<T>> CatboostClassification()
    {
        if (_cbm == null || _meta == null)
            throw new InvalidOperationException("Modelo CatBoost não carregado.");

        var notasAtualizadas = new List<T>();
        int tamanhoLote = 500;

        bool isNota = typeof(T) == typeof(NotaModel);

        Dictionary<string, string> tradutor = isNota ? new Dictionary<string, string>()
        {
            { "Tipo de nota", "StatusNota" },
            { "Txt.grp.codific", "StatusNota" },
            { "Descrição", "DescricaoBreve" },
            { "Tipo prioridade", "Prioridade" },
            { "Txt.code codif.", "StatusNota" },
            { "Equipamento", "IdAtivo" }
        } : new Dictionary<string, string>()
        {
            { "Tipo de nota", "StatusOrdem" },
            { "Descrição", "DescricaoBreve" },
            { "Tipo prioridade", "Criticidade" },
            { "Equipamento", "IdAtivo" }
        };

        var cachePropsNotas = new Dictionary<string, PropertyInfo>();
        foreach (var kvp in tradutor)
        {
            var propNota = typeof(T).GetProperty(kvp.Value);
            if (propNota != null) cachePropsNotas[kvp.Key] = propNota;
        }

        while (true)
        {
            var query = _dbSet.Where(n => n.Classificacao == Classificacao.NAOCLASSIFICADA).AsQueryable();

            if (isNota)
            {
                query = query.Include("Itens");
            }

            var lote = await query.Take(tamanhoLote).ToListAsync();
            if (lote.Count == 0)
                break;

            var catFeatures = new string[lote.Count][];
            var textFeatures = new string[lote.Count][];

            for (int i = 0; i < lote.Count; i++)
            {
                var nota = lote[i];
                var rowCat = new string[_meta.cat_features.Count];
                for (int j = 0; j < _meta.cat_features.Count; j++)
                {
                    var feature = _meta.cat_features[j];
                    string? valor = null;

                    if (cachePropsNotas.TryGetValue(feature, out var prop))
                    {
                        valor = prop.GetValue(nota)?.ToString();
                    }

                    rowCat[j] = NormalizeAsciiLower(valor, feature);
                }
                catFeatures[i] = rowCat;

                var rowText = new string[_meta.text_features.Count];
                for (int j = 0; j < _meta.text_features.Count; j++)
                {
                    string feature = _meta.text_features[j];
                    string? valor = null;

                    if (cachePropsNotas.TryGetValue(feature, out var prop))
                    {
                        valor = prop.GetValue(nota)?.ToString();
                    }

                    rowText[j] = NormalizeAsciiLower(valor, feature);
                }
                textFeatures[i] = rowText;
            }

            double[] probabilidades = _cbm.Predict([], catFeatures, textFeatures);

            for (int i = 0; i < lote.Count; i++)
            {
                var nota = lote[i];
                var chanceDeSerCorretiva = probabilidades[i];

                if (chanceDeSerCorretiva > 0.8)
                    nota.Classificacao = Classificacao.CORRETIVA;
                else if (chanceDeSerCorretiva < 0.7)
                    nota.Classificacao = Classificacao.PREVENTIVA;
                else nota.Classificacao = Classificacao.NAOCLASSIFICADA;

                nota.MetodoClassificacao = MetodoClassificacao.CATBOOST;
                nota.DataClassificacao = DateTime.UtcNow;

                notasAtualizadas.Add(nota);
            }

            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
        }

        return notasAtualizadas;
    }
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="columnName">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>

    public static string NormalizeAsciiLower(string? s, string columnName)
    {
        if (string.IsNullOrEmpty(s))
        {
            return ((string[])["Txt.grp.cod.pr.", "Txt.code prob.", "Texto grp.causa", "Txt.code causa"])
                .Contains(columnName)
                    ? "nan"
                    : "missing";
        }

        s = s.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var t in s.Where(t => CharUnicodeInfo.GetUnicodeCategory(t) != UnicodeCategory.NonSpacingMark))
            sb.Append(t);

        return sb.ToString().ToLower();
    }
}