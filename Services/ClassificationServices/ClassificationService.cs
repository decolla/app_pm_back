using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using app_pm_back.Data;
using app_pm_back.DTO;
using app_pm_back.Interfaces;
using app_pm_back.Models.Domain;
using app_pm_back.Services.GenericServices;
using Microsoft.EntityFrameworkCore;
using teste_catboost_cs;

namespace app_pm_back.Services.FichasServices;

/// <summary>
/// Classe para classificar notas/ordens usando regras de palavras e catboost.
/// </summary>
/// <typeparam name="T">Tipo genérico para qualquer parâmetro classificável (nota ou ordem)</typeparam>
public class ClassificationService<T> where T : class, IClassifiable
{
    protected readonly AppDbContext _context;
    protected readonly ConfiguracaoGlobalService _configuracaoService;
    protected readonly DbSet<T> _dbSet;
    protected readonly DbSet<RegraClassificacaoModel> _classificationModel;
    protected readonly CatboostModelProviderService _catboostProvider;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="ClassificationService{T}"/>.
    /// </summary>
    public ClassificationService(
        AppDbContext context,
        ConfiguracaoGlobalService configuracaoService,
        CatboostModelProviderService catboostProvider)
    {
        _context = context;
        _configuracaoService = configuracaoService;
        _dbSet = context.Set<T>();
        _catboostProvider = catboostProvider;
        _classificationModel = context.Set<RegraClassificacaoModel>();
    }

    /// <summary>
    /// Classificação manual de notas/ordens.
    /// </summary>
    public async Task<T?> HumanClassification(long id, Classificacao classificacao, string? usuario)
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
    /// Executa a classificação com base no que está aplicado nas configurações globais.
    /// </summary>
    /// <returns>Listas classificadas</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<List<T>> AutoClassify()
    {
        var config = await _configuracaoService.ObterConfiguracaoAsync();
        if (config.ClassificacaoAutomatica == ModoClassificacaoAutomatica.ClassificarCatBoost)
        {
            return await AutoClassifyCatboost();
        }

        if (config.ClassificacaoAutomatica == ModoClassificacaoAutomatica.ClassificarPalavrasChave)
        {
            return await AutoClassifyWords();
        }

        if (config.ClassificacaoAutomatica == ModoClassificacaoAutomatica.NaoClassificar)
        {
            return new List<T>();
        }

        throw new InvalidOperationException(
            "Modo de classificação automática não habilitada nas classificações globais.");
    }

    public async Task<List<T>> AutoClassifyWords()
    {
        var notasSemClassificacao =
            await _dbSet.Where(n =>
                    n.Classificacao == Classificacao.NAOCLASSIFICADA
                    || n.MetodoClassificacao == MetodoClassificacao.CATBOOST
                    || n.MetodoClassificacao == MetodoClassificacao.ALGORITMO)
                .ToListAsync();

        return await WordsClassification(notasSemClassificacao);
    }

    /// <summary>
    /// Função para classificar notas/ordens com base nas regras de palavras.
    /// A busca será feita no controller e passará todas que estiverem sem classificar,
    /// e quais deverão ser reclassificadas.
    /// </summary>
    /// <param name="fichas"> Lista de tipo genérico de notas/ordens já classificadas para serem analisadas pelo algoritmo</param>
    /// <returns>Todas as notas que não tinham sido classificadas juntamente as reclassificadas</returns>
    public async Task<List<T>> WordsClassification(List<T> fichas)
    {
        // var config = await _configuracaoService.ObterConfiguracaoAsync();
        // if (config.ClassificacaoAutomatica != ModoClassificacaoAutomatica.ClassificarPalavrasChave)
        // {
        //     throw new InvalidOperationException(
        //         "Modo de classificação automática não habilitada nas classificações globais.");
        // }

        // busco todas as notas/ordems sem classificação
        // remover comentário caso deve reavaliar regras já avaladas
        DateTimeOffset dataOffset = DateTimeOffset.FromUnixTimeMilliseconds(10000);

        // le o campo alvo da regra e busca a palavra chave 
        List<RegraClassificacaoModel> regras = await _classificationModel.OrderBy(r => r.OrdemPrioridade).ToListAsync();

        // dicionário para armazenar um cache das propriedades
        Dictionary<string, PropertyInfo> cache = new Dictionary<string, PropertyInfo>();

        List<string> camposAlvos;

        // realiza os campos alvos dependendo do tipo de modelo
        if (typeof(T) == typeof(NotaModel))
            camposAlvos =
            [
                "TipoNota", "Finalidade", "EquipamentoDisponivel", "AnomaliaObservada", "ObservacoesComplementares",
                "DescricaoBreve"
            ];
        else if (typeof(T) == typeof(OrdemModel))
            camposAlvos =
            [
                "TipoOrdem", "DescricaoBreve", "Criticidade"
            ];
        else throw new Exception("Tipo de modelo não para classificação de palavras");

        foreach (var campo in camposAlvos)
        {
            var prop = typeof(T).GetProperty(campo);
            if (prop != null)
            {
                cache[campo] = prop;
            }
        }

        // caso a palavra exista atualiza a ficha com o método "FILTRO_PALAVRAS"
        foreach (var nota in fichas)
        {
            bool classificada = false;

            foreach (var regra in regras)
            {
                if (classificada) break;

                // procura para campo alvo
                foreach (var campoAlvo in camposAlvos)
                {
                    // captura a propriedade alvo da nota e a palavra chave
                    if (!cache.TryGetValue(campoAlvo, out var propriedadeAlvo))
                        continue;

                    string palavraChave = regra.PalavraChave;

                    string? valorAlvo = propriedadeAlvo.GetValue(nota)?.ToString();

                    // verifica se a palavra chave existe no valor alvo
                    if (valorAlvo != null && valorAlvo.Contains(palavraChave, StringComparison.OrdinalIgnoreCase))
                    {
                        // caso a regra for proibida 
                        if (!regra.Permitida)
                        {
                            nota.Classificacao = Classificacao.NAOCLASSIFICADA;
                            nota.MetodoClassificacao = MetodoClassificacao.ALGORITMO;
                            nota.DataClassificacao = DateTime.UtcNow;
                            classificada = true;
                            break;
                        }

                        nota.Classificacao = regra.ClassificacaoAplicada;
                        nota.ClassificadoPor = null;
                        nota.DataClassificacao = DateTime.UtcNow;
                        nota.MetodoClassificacao = MetodoClassificacao.ALGORITMO;
                        classificada = true;
                        break;
                    }
                }
            }
        }

        await _context.SaveChangesAsync();

        // retorna as notas/ordens atualizadas
        return fichas;
    }

    public async Task<List<T>> AutoClassifyCatboost()
    {
        List<T> query = await _dbSet.Where(n => (
                                                    n.Classificacao == Classificacao.NAOCLASSIFICADA &&
                                                    n.MetodoClassificacao != MetodoClassificacao.CATBOOST) ||
                                                n.MetodoClassificacao == MetodoClassificacao.CATBOOST)
            .ToListAsync();
        return await CatboostClassification(query, null);
    }

    /// <summary>
    /// Classificação de notas/ordens com base no modelo Catboost.
    /// </summary>
    public async Task<List<T>> CatboostClassification(List<T> query, string? nomeModelo)
    {
        // var config = await _configuracaoService.ObterConfiguracaoAsync();
        // if (config.ClassificacaoAutomatica != ModoClassificacaoAutomatica.ClassificarCatBoost)
        //     throw new InvalidOperationException(
        //         "Modo de classificação por catboost não habilitada nas classificações globais.");

        if (string.IsNullOrEmpty(nomeModelo))
        {
            var config = await _configuracaoService.ObterConfiguracaoAsync();
            nomeModelo = config.ModeloCatboostPadrao;

            if (string.IsNullOrWhiteSpace(nomeModelo))
            {
                // se houver o modelo padrão no disco, usa-o
                var directModelPath = Path.Combine(AppContext.BaseDirectory, "CatBoost", "Modelos", "catboost_model.cbm");
                if (File.Exists(directModelPath))
                {
                    nomeModelo = "catboost_model.cbm";
                }
                else
                {
                    throw new InvalidOperationException(
                        "Nenhum modelo CatBoost foi selecionado e nenhum modelo padrão está configurado.");
                }
            }
        }

        var (cbm, meta) = _catboostProvider.ObterModelo(nomeModelo);
        if (cbm == null || meta == null)
            throw new InvalidOperationException("Modelo CatBoost não carregado.");

        var notasAtualizadas = new List<T>();
        int tamanhoLote = 500;

        bool isNota = typeof(T) == typeof(NotaModel);

        // atribui a tradução de colunas para campos
        Dictionary<string, string> tradutor = isNota
            ? new Dictionary<string, string>()
            {
                { "Tipo de nota", "StatusNota" },
                { "Txt.grp.codific", "StatusNota" },
                { "Descrição", "DescricaoBreve" },
                { "Tipo prioridade", "Prioridade" },
                { "Txt.code codif.", "StatusNota" },
                { "Equipamento", "IdAtivo" }
            }
            : new Dictionary<string, string>()
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
            List<T> lote = query.Take(tamanhoLote).ToList();
            if (lote.Count == 0)
                break;

            var catFeatures = new string[lote.Count][];
            var textFeatures = new string[lote.Count][];

            for (int i = 0; i < lote.Count; i++)
            {
                var nota = lote[i];
                var rowCat = new string[meta.cat_features.Count];
                for (int j = 0; j < meta.cat_features.Count; j++)
                {
                    var feature = meta.cat_features[j];
                    string? valor = null;

                    if (cachePropsNotas.TryGetValue(feature, out var prop))
                    {
                        valor = prop.GetValue(nota)?.ToString();
                    }

                    rowCat[j] = NormalizeAsciiLower(valor, feature);
                }

                catFeatures[i] = rowCat;

                var rowText = new string[meta.text_features.Count];
                for (int j = 0; j < meta.text_features.Count; j++)
                {
                    string feature = meta.text_features[j];
                    string? valor = null;

                    if (cachePropsNotas.TryGetValue(feature, out var prop))
                    {
                        valor = prop.GetValue(nota)?.ToString();
                    }

                    rowText[j] = NormalizeAsciiLower(valor, feature);
                }

                textFeatures[i] = rowText;
            }

            double[] probabilidades = cbm.Predict([], catFeatures, textFeatures);

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