using app_pm_back.DTO;
using app_pm_back.Models;
using app_pm_back.Services;
using AutoMapper;

namespace app_pm_back.Routes;

public static class NoteEndpoints
{
    public static void MapNoteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/notas");
        
        // endpoint para o cadastro de notas
        group.MapPost("/", async (CreateNotaPetroDto dto, IMapper mapper, NotasService service) =>
        {
            try
            {
                // realiza mapeamento automático
                var novaNota = mapper.Map<NotaPetroModel>(dto);

                // cria a nota
                var notaCriada = await service.CreateNotaAsync(novaNota);
                return Results.Created($"notes/{notaCriada.IdNota}", notaCriada);
            }
            catch (Exception e)
            {
                return Results.BadRequest(new { message = $"Erro ao criar a nota: {e.Message}." });
            }
        });

        // endpoint que retorna uma lista todas as notas com sistema de páginas
        group.MapGet("/", async (int? pagina, int? tamanho, NotasService service) =>
        {
            try
            {
                if (pagina.HasValue || tamanho.HasValue)
                {
                    int p = pagina ?? 1;
                    int t = tamanho ?? 20;
                    var notasPaginadas = await service.GetNotasPaginadasAsync(p, t);
                    return Results.Ok(notasPaginadas);
                }

                var todasNotas = await service.GetAllNotasAsync()!;
                return Results.Ok(todasNotas);
            }
            catch (Exception)
            {
                return Results.BadRequest(new { message = "Erro ao processar notas." });
            }
        });

        // endpoint para buscar uma nota por Id
        group.MapGet("/{id:int}", async (int id, NotasService service) =>
        {
            try
            {
                var nota = await service.GetNotaByIdAsync(id);
                if (nota == null) return Results.NotFound(new { message = "Nota não encontrada" });
                return Results.Ok(nota);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao procurar nota: {e.Message}");
                return Results.NotFound(new { message = "Erro ao procurar nota." });
            }
        });

        // endpoint para retornar a quantidade de notas no banco
        group.MapGet("/count", async (NotasService service) =>
        {
            try
            {
                var notas = await service.CountNotasAsync();
                return Results.Ok(new { count = notas });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao procurar notas: {e.Message}");
                return Results.NotFound(new { message = "Erro ao procurar notas." });
            }
        });

        // endpoint para atualizar uma nota
        group.MapPut("/", async (NotaPetroModel nota, NotasService service) =>
        {
            try
            {
                var result = await service.UpdateNotaAsync(nota);
                return Results.Ok(new { message = "Nota atualizada com sucesso: " + result.IdNota });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao atualizar nota: {e.Message}");
                return Results.BadRequest(new { message = "Erro ao atualizar nota." });
            }
        });

        // endpoint para deletar uma nota por Id
        group.MapDelete("/{id:int}", async (int id, NotasService service) =>
        {
            try
            {
                var result = await service.DeleteNotaAsync(id);
                return Results.Ok(new { message = "Nota deletada com sucesso"});
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao deletar nota: {e.Message}");
                return Results.BadRequest(new { message = "Erro ao deletar nota." });
            }
        });
        
        // endpoint para listar as fichas de um ativo, o filtro é opcional
        group.MapGet("ativo/{ativo}", async (string ativo, string? tipo, NotasService service) =>
        {
            try
            {
                var resultado = await service.GetFichasPorAtivoAsync(ativo, tipo);
                return Results.Ok(resultado);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao buscar fichas do ativo: {e.Message}");
                return Results.BadRequest(new { message = "Erro ao buscar fichas do ativo." });
            }
        });
    }
}