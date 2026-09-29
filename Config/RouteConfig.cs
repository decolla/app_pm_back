using System.Text.RegularExpressions;
using System.Security;

namespace app_pm_back.Config;

/// <summary>
/// Configura a rota base para os endpoints da API, incluindo a versão e o nome do cliente.
/// </summary>
public static class RouteConfig
{
    public const string ClientName = "";
    public const string BaseRoute = "v1" + ClientName;

    /// <summary>
    /// Obtém o valor bruto da variável de ambiente SCHEMA
    /// </summary>
    private static string? GetRawSchemaEnv()
    {
        var raw = Environment.GetEnvironmentVariable("DATABASE_SCHEMA");
        if (string.IsNullOrWhiteSpace(raw))
        {
            raw = Environment.GetEnvironmentVariable("SCHEMA");
        }

        return raw?.Trim('/');
    }

    /// <summary>
    /// Obtém a parte do schema do banco de dados
    /// </summary>
    public static string? GetDatabaseSchema()
    {
        var raw = GetRawSchemaEnv();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        var parts = raw.Split('/');
        var schema = parts[0];

        // segurança 
        if (!string.IsNullOrWhiteSpace(schema) && !Regex.IsMatch(schema, "^[a-zA-Z0-9_-]+$"))
        {
            throw new SecurityException("Nome de schema de banco de dados inválido ou inseguro.");
        }

        return schema;
    }

    /// <summary>
    /// Obtém o nome do serviço a partir da variável de ambiente
    /// </summary>
    public static string? GetServiceName()
    {
        var raw = GetRawSchemaEnv();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        var parts = raw.Split('/');
        if (parts.Length > 1)
        {
            var service = parts[1];
            // validação dos caracteres permitidos para a url
            if (!string.IsNullOrWhiteSpace(service) && !Regex.IsMatch(service, "^[a-zA-Z0-9_-]+$"))
            {
                throw new SecurityException("Nome de serviço inválido ou inseguro.");
            }

            return service;
        }

        return null;
    }

    /// <summary>
    /// Monta a URL base de um serviço. Prioridade: variáveis de ambiente (SERVER_IP e SCHEMA);
    /// </summary>
    /// <param name="configuration">Configuração da aplicação (appsettings).</param>
    /// <param name="configKey">Chave no appsettings, ex: "APIs:Twins".</param>
    /// <param name="servicePath">Caminho do serviço na URL, ex: "tw/api".</param>
    private static string BuildServiceUrl(IConfiguration configuration, string configKey, string servicePath)
    {
        var isProd = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
        var serverIp = Environment.GetEnvironmentVariable("SERVER_IP");
        var schema = GetDatabaseSchema();

        // Em produção sempre usa o domínio público
        if (isProd)
        {
            var prodSchema = schema ?? configuration["SCHEMA"] ?? "comgas";
            return $"https://physisiot.com.br/{prodSchema}/{servicePath}";
        }

        // Se SERVER_IP e SCHEMA foram definidos no ambiente, eles montam a URL
        if (!string.IsNullOrWhiteSpace(serverIp) && !string.IsNullOrWhiteSpace(schema))
        {
            return $"http://{serverIp}/{schema}/{servicePath}";
        }

        // Caso contrário, usa o valor do appsettings
        return configuration[configKey]
            ?? throw new InvalidOperationException(
                $"Configuração '{configKey}' não encontrada e SERVER_IP/SCHEMA não definidos.");
    }

    /// <summary>
    /// Retorna a URL do serviço APM com base na variável de ambiente.
    /// </summary>
    public static string GetApmBaseUrl(IConfiguration configuration)
        => BuildServiceUrl(configuration, "APIs:APM", "apm/api");

    public static string GetTwinsBaseUrl(IConfiguration configuration)
        => BuildServiceUrl(configuration, "APIs:Twins", "tw/api");

    public static string GetReliabilityBaseUrl(IConfiguration configuration)
        => BuildServiceUrl(configuration, "APIs:Reliability", "reliability/api");
}