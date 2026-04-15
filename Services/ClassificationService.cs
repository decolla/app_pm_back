using System.Globalization;
using System.Reflection;
using System.Text;
using app_pm_back.Data;
using app_pm_back.Interfaces;
using app_pm_back.Models;
using app_pm_back.Models.CongasModel;
using Microsoft.EntityFrameworkCore;
using teste_catboost_cs;

namespace app_pm_back.Services;

/// <summary>
/// Classe para classificar notas/ordens
/// Usando regras de palavras e catboost
/// É usado um tipo genérico para que possa ser usado em qualquer tipo de nota/ordem,
/// desde que ele herde de IClassifiable
/// </summary>
/// <typeparam name="T">Tipo genérico para qualquer parâmetro classificável (nota ou ordem)</typeparam>
public class ClassificationService<T> where T : class, IClassifiable
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<RuleClassificationModel> _classificationModel;
    protected readonly DbSet<T> _dbSet;

    // varáveis para catboost 
    protected readonly Cbm? _cbm;
    protected readonly ModelMeta? _meta;

    public ClassificationService(AppDbContext context, Cbm? modeloCatboost = null, ModelMeta? meta = null)
    {
        _context = context;
        _classificationModel = context.Set<RuleClassificationModel>();
        _dbSet = context.Set<T>();
        _cbm = modeloCatboost;
        _meta = meta;
    }

    /// <summary>
    /// Cria uma regra de palavras
    /// </summary>
    /// <param name="rule">Regra no formato de RuleClassificationModel</param>
    /// <returns>Regra passada como parâmetro</returns>
    public async Task<RuleClassificationModel> CreateRule(RuleClassificationModel rule)
    {
        await _classificationModel.AddAsync(rule);
        await _context.SaveChangesAsync();
        return rule;
    }

    /// <summary>
    /// Classificação manual de notas/ordens. Um usuário pode classificar manualmente uma nota/ordem.
    /// </summary>
    /// <param name="id">Id da nota/ordem a ser classificada</param>
    /// <param name="classificacao">Classificação dada a nota/ordem</param>
    /// <param name="usuario">Id do usuário que está realizando a classificação</param>
    /// <returns>Nota atualizada</returns>
    public async Task<T?> HumanClassification(long id, string classificacao, string usuario)
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
            await _dbSet.Where(n => n.Classificacao == null /* || n.ClassificadoPor == null*/).ToListAsync();

        // leio o campo alvo da regra e busco a palavra chave 
        List<RuleClassificationModel> regras = await _classificationModel.OrderBy(r => r.OrdemPrioridade).ToListAsync();

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
                    nota.MetodoClassificacao = "FILTRO_PALAVRAS";
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
    /// Com base no modelo Catboost, classifica as notas/ordens com base nas probabilidades,
    /// utilizando um tradutor para os inputs da predição.
    /// </summary>
    /// <returns>Notas/ordens atualizadas com base no cáculo de probabilidades do modelo catboost</returns>
    public async Task<List<T>> CatboostClassification()
    {
        if (_cbm == null || _meta == null)
            throw new InvalidOperationException("Modelo CatBoost não carregado. Verifique os arquivos do modelo e reinicie a aplicação.");

        var notasAtualizadas = new List<T>();
        int tamanhoLote = 500; // processa 500 por vez

        // descobre se é a classificação de notas ou ordens 
        bool isNota = typeof(T) == typeof(NoteCongasModel);

        // mapeamento de nomes para o modelo catboost
        Dictionary<string, string> tradutor = new Dictionary<string, string>();

        if (isNota)
        {
            tradutor = new Dictionary<string, string>()
            {
                // NotaRow
                { "Tipo de nota", "TipoNota" },
                { "Txt.grp.codific", "TxtGrpCodific" },
                { "Descrição", "Descricao" },
                { "Tipo prioridade", "TipoPrioridade" },
                { "Txt.code codif.", "TxtCodeCodif" },
                { "Equipamento", "Equipamento" },
                { "Codificação", "CodificacaoX" },
                { "Conseqüência", "Consequencia" },
                { "Grupo de codes", "GrupoDeCodesX" },

                // ItemRow
                { "Nº de ordenação", "NumOrdenacao" },
                { "TxtGrpCodPartOb", "TxtGrpCodPartOb" },
                { "Txt.cod.part.ob", "TxtCodPartOb" },
                { "Txt.grp.cod.pr.", "TxtGrpCodPr" },
                { "Txt.code prob.", "TxtCodeProb" },
                { "Texto grp.causa", "TextoGrpCausa" },
                { "Txt.code causa", "TxtCodeCausa" }
            };
        }
        else
        {
            tradutor = new Dictionary<string, string>()
            {
                // OrdemRow
                { "Tipo de nota", "TipoOrdem" },
                { "Descrição", "TextoBreve" },
                { "Tipo prioridade", "TipoPrioridade" },
                { "Equipamento", "Equipamento" },
            };
        }

        // cache de propriedades para melhorar o desempenho usando reflection
        var cachePropsNotas = new Dictionary<string, PropertyInfo>();
        var cachePropsItens = new Dictionary<string, PropertyInfo>();

        foreach (var kvp in tradutor)
        {
            var propNota = typeof(T).GetProperty(kvp.Value);
            if (propNota != null) cachePropsNotas[kvp.Key] = propNota;
            else if (isNota) // só procura no item se for nota
            {
                var propItem = typeof(ItemNotaCongasModel).GetProperty(kvp.Value);
                if (propItem != null) cachePropsItens[kvp.Key] = propItem;
            }
        }

        while (true)
        {
            // query condicional
            var query = _dbSet.Where(n => n.Classificacao == null || n.Classificacao == "").AsQueryable();

            if (isNota)
            {
                query = query.Include("Itens");
            }

            // busca apenas um lote de notas sem classificação
            var lote = await query.Take(tamanhoLote).ToListAsync();

            // se não encontrou mais nenhuma, sai do loop
            if (lote.Count == 0)
                break;

            // matrizes com tamanho exato do LOTE ATUAL
            var catFeatures = new string[lote.Count][];
            var textFeatures = new string[lote.Count][];

            // percorre cada nota e preenche a matriz de catFeatures
            for (int i = 0; i < lote.Count; i++)
            {
                var nota = lote[i];

                // tenta pegar o primeiro item associado a nota
                object? primeiroItem = null;
                var propItens = nota.GetType().GetProperty("Itens");
                if (propItens != null)
                {
                    var colecaoItens = propItens.GetValue(nota) as IEnumerable<object>;
                    primeiroItem = colecaoItens?.FirstOrDefault();
                }

                // array de strings para cada coluna do catFeatures
                var rowCat = new string[_meta.cat_features.Count];
                for (int j = 0; j < _meta.cat_features.Count; j++)
                {
                    var feature = _meta.cat_features[j];
                    string? valor = null;

                    // le o valor da propriedade da nota em cache 
                    if (cachePropsNotas.TryGetValue(feature, out var prop))
                    {
                        valor = prop.GetValue(nota)?.ToString();
                    }
                    else if (primeiroItem != null && cachePropsItens.TryGetValue(feature, out var propItem))
                    {
                        valor = propItem.GetValue(primeiroItem)?.ToString();
                    }

                    // aplica normalização de ASCII e "missing" para cada valor
                    rowCat[j] = NormalizeAsciiLower(valor, feature);
                }

                // salva a linha na matriz
                catFeatures[i] = rowCat;

                // preenche a matriz de textFeatures
                var rowText = new string[_meta.text_features.Count];
                for (int j = 0; j < _meta.text_features.Count; j++)
                {
                    string feature = _meta.text_features[j];
                    string? valor = null;

                    // le o valor da propriedade da nota em cache 
                    if (cachePropsNotas.TryGetValue(feature, out var prop))
                    {
                        valor = prop.GetValue(nota)?.ToString();
                    }
                    else if (primeiroItem != null && cachePropsItens.TryGetValue(feature, out var propItem))
                    {
                        valor = propItem.GetValue(primeiroItem)?.ToString();
                    }

                    // aplica normalização de ASCII e "missing" para cada valor
                    rowText[j] = NormalizeAsciiLower(valor, feature);
                }

                // salva a linha na matriz
                textFeatures[i] = rowText;
            }

            // extrai o array de catFeatures e o array de textFeatures
            // gera um array de probabilidades
            double[] probabilidades = _cbm.Predict([], catFeatures, textFeatures);

            // classifica as notas com base nas probabilidades
            for (int i = 0; i < lote.Count; i++)
            {
                var nota = lote[i];
                var chanceDeSerCorretiva = probabilidades[i];

                if (chanceDeSerCorretiva > 0.5)
                    nota.Classificacao = "CORRETIVA";
                else
                    nota.Classificacao = "PREVENTIVA";

                nota.MetodoClassificacao = "IA_CATBOOST";
                nota.DataClassificacao = DateTime.UtcNow;

                notasAtualizadas.Add(nota);
            }

            // salva o lote no banco de dados
            await _context.SaveChangesAsync();

            // limpeza de memória 
            _context.ChangeTracker.Clear();
        }

        return notasAtualizadas;
    }

    /// <summary>
    /// Normaliza uma string para minúsculas e ASCII, removendo acentos.
    /// Retorna "nan" ou "missing" caso o valor seja nulo ou vazio, dependendo da coluna.
    /// </summary>
    /// <param name="s">String a ser normalizada.</param>
    /// <param name="columnName">Nome da coluna para tratamento especial de valores nulos.</param>
    /// <returns>String normalizada.</returns>
    public static string NormalizeAsciiLower(string? s, string columnName)
    {
        if (string.IsNullOrEmpty(s))
        {
            // O modelo foi treinado com "nan" para essas features específicas. Verificar se faz sentido e talvez alterar o modelo.
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

    // ADICIONAR DPS PARA CLASSIFICAR NOTAS/ORDENS JÁ CLASSIFICADAS ANTERIORMENTE
}