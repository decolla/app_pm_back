using app_pm_back.Interfaces;
using Microsoft.Extensions.Configuration;

namespace app_pm_back.Services;

/// <summary>
/// Centraliza o schema da aplicação para que o código nunca confie em schemas enviados nas requisições
/// </summary>
public class ApplicationSchemaProvider : IApplicationSchemaProvider
{
    public ApplicationSchemaProvider(IConfiguration configuration)
    {
        // captura o schema da forma correta
        var rawSchema = configuration["SCHEMA"]
            ?? configuration["DATABASE_SCHEMA"]
            ?? Environment.GetEnvironmentVariable("SCHEMA")
            ?? Environment.GetEnvironmentVariable("DATABASE_SCHEMA");

        if (string.IsNullOrWhiteSpace(rawSchema))
            throw new InvalidOperationException("A variavel de SCHEMA está faltando.");

        var schema = rawSchema.Trim('/');
        if (schema.Contains('/'))
        {
            schema = schema.Split('/')[0];
        }

        if (string.IsNullOrWhiteSpace(schema))
            throw new InvalidOperationException("A variável de SCHEMA está faltando.");

        Schema = schema;
    }

    public string Schema { get; }
}
