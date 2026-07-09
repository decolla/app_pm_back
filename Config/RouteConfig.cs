using System.Text.RegularExpressions;
using System.Security;

namespace app_pm_back.Config;

/// <summary>
/// Configura a rota base para os endpoints da API, incluindo a versão e o nome do cliente.
/// </summary>
public static class RouteConfig
{
    public const string ClientName = "";
    public const string BaseRoute = "api/V1" + ClientName;

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
    /// Retorna a URL do serviço APM com base na variável de ambiente.
    /// </summary>
    public static string GetApmBaseUrl(IConfiguration configuration)
    {
        var isHml = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Homologation";

        if (isHml)
        {
            var schema = GetDatabaseSchema() ?? "public";
            return $"http://10.3.192.202/{schema}/apm_back";
        }
        else
        {
            var serviceName = GetServiceName();

            // o serviço especificado na env var for reliability_back, usa a URL de reliability
            if (serviceName != null && serviceName.Contains("reliability"))
            {
                return configuration["APIs:Reliability"] ?? "http://10.3.192.200:60170";
            }

            // caso contrário, ou por padrão se for apm_back, usamos a do APM
            return configuration["APIs:APM"] ?? "http://10.3.192.200:60142";
        }
    }

    /// <summary>
    /// Retorna a URL do serviço com base na variável de ambiente.
    /// </summary>
    public static string GetTwinsBaseUrl(IConfiguration configuration)
    {
        var isHml = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Homologation";
        if (isHml)
        {
            var schema = GetDatabaseSchema() ?? "public";
            return $"http://10.3.192.202/{schema}/tw_back";
        }
        else
        {
            var url = configuration["APIs:Twins"] ?? "http://10.3.192.200:60131";
            return url + "/api/v1";
        }
    }

    /// <summary>
    /// Retorna a URL do serviço Reliability com base na variável de ambiente.
    /// </summary>
    public static string GetReliabilityBaseUrl(IConfiguration configuration)
    {
        var isHml = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Homologation";
        if (isHml)
        {
            var schema = GetDatabaseSchema() ?? "public";
            return $"http://10.3.192.202/{schema}/reliability_back";
        }
        else
        {
            var serviceName = GetServiceName();

            // o serviço especificado na env var for apm_back, usamos a URL do Twins
            if (serviceName != null && serviceName.Contains("twins_back"))
            {
                return configuration["APIs:APM"] ?? "http://10.3.192.200:60141";
            }

            // caso contrário, ou por padrão se for reliability_back
            return configuration["APIs:Reliability"] ?? "http://10.3.192.200:60170";
        }
    }
}