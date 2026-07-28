using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace app_pm_back.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Deixa a requisição seguir o seu caminho
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException || ex.InnerException is SqlException)
        {
            // Intercepta erros do banco de dados globalmente
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            string mensagemAmigavel = "Ocorreu um erro ao salvar os dados no banco.";

            if (ex.InnerException is PostgresException pgEx)
            {
                mensagemAmigavel = pgEx.SqlState switch
                {
                    "23503" => "Operação recusada: O registro faz referência a um dado que não existe (Ex: Equipamento ou Local inválido).",
                    "23505" => "Operação recusada: Já existe um registro com esta mesma identificação.",
                    "22001" => "Operação recusada: Um dos campos de texto enviados ultrapassa o limite máximo de caracteres permitido.",
                    _ => "Ocorreu um erro ao salvar os dados no banco."
                };
            }
            else if (ex.InnerException is SqlException sqlEx)
            {
                mensagemAmigavel = sqlEx.Number switch
                {
                    547 => "Operação recusada: O registro faz referência a um dado que não existe (Ex: Equipamento ou Local inválido).",
                    2627 or 2601 => "Operação recusada: Já existe um registro com esta mesma identificação.",
                    2628 or 8152 => "Operação recusada: Um dos campos de texto enviados ultrapassa o limite máximo de caracteres permitido.",
                    _ => "Ocorreu um erro ao salvar os dados no banco."
                };
            }

            await context.Response.WriteAsJsonAsync(new { message = mensagemAmigavel });
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { message = "Erro interno do servidor.", detalhe = ex.Message });
        }
    }
}