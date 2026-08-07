using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using teste_catboost_cs;

namespace app_pm_back.Services.FichasServices;

public class CatboostModelProviderService
{
    private readonly IMemoryCache _cache;
    private readonly string _basePath;

    public CatboostModelProviderService(IMemoryCache cache)
    {
        _cache = cache;
        _basePath = Path.Combine(AppContext.BaseDirectory, "CatBoost", "Modelos");
    }

    /// <summary>
    /// Lê as subpastas e devolve os nomes dos modelos.
    /// </summary>
    /// <returns>Lista dos modelos disponíveis.</returns>
    public List<string?> ListarModelosDisponiveis()
    {
        if (!Directory.Exists(_basePath))
        {
            return new List<string?>();
        }

        return Directory.GetDirectories(_basePath).Select(Path.GetFileName).ToList();
    }

    /// <summary>
    /// Obtém um modelo da cahce da RAM, ou carrega do disco da primeira vez que for chamado
    /// </summary>
    /// <param name="modelo">Modelo a ser obtido</param>
    /// <returns>Tupla de modelo catboost e meta</returns>
    public (Cbm Model, ModelMeta) ObterModelo(string modelo)
    {
        var cacheKey = $"catboost_model_{modelo}";

        // tenta pegar do cache
        if (!_cache.TryGetValue(cacheKey, out (Cbm Model, ModelMeta) modelTuple))
        {
            // se não estiver no cache, carrega do disco
            var modelPath = Path.Combine(_basePath, modelo, "catboost_model.cbm");
            var metaPath = Path.Combine(_basePath, modelo, "model_meta.json");

            if (!File.Exists(modelPath) || !File.Exists(metaPath))
            {
                throw new FileNotFoundException($"Os ficheiros não foram encontrados para o modelo: {modelo}");
            }

            var cbm = new Cbm(modelPath);
            var metaJson = File.ReadAllText(metaPath);
            var meta = JsonSerializer.Deserialize<ModelMeta>(metaJson);

            modelTuple = (cbm, meta)!;

            // adiciona ao cache
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.High);

            _cache.Set(cacheKey, modelTuple, cacheOptions);
        }

        return modelTuple;
    }
}