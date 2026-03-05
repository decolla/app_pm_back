using app_pm_back.DTO;
using app_pm_back.Models;
using app_pm_back.Services;
using AutoMapper;


namespace app_pm_back.Routes;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/ordem");

        // endpoint para o cadastro de ordens
        group.MapPost("/dto", async (CreateOrdemPetroDto dto, IMapper mapper, OrdemService service) =>
        {
            try
            {
                var novaOrdem = mapper.Map<OrdemPetroModel>(dto);
                
                var notaCriada = await service.CreateOrdemAsync(novaOrdem);
                return Results.Created($"ordem/{notaCriada.IdOrdem}", notaCriada);
            }catch (Exception e)
            {
                return Results.BadRequest(new { message = $"Erro ao criar a ordem: {e.Message}." });
            }
        });

        // endpoint essencial para o cadastro de ordens
        group.MapPost("/", async (OrdemPetroModel ordemNova, OrdemService service, IMapper mapper) =>
        {
            try
            {
                var ordem = await service.CreateOrdemAsync(ordemNova);
                return Results.Created($"ordem/{ordem.IdOrdem}", ordem);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao criar a ordem: {e.Message}");
                return Results.BadRequest(new { message = "Erro ao criar a ordem." });
            }
        });

        // endpoint que retorna uma lista todas as ordens
        group.MapGet("/", async (int? pagina, int? tamanho, OrdemService service) =>
        {
            try
            {
                if (pagina.HasValue || tamanho.HasValue)
                {
                    int p = pagina ?? 1;
                    int t = tamanho ?? 20;
                    var notasPaginadas = await service.GetOrdensPaginadasAsync(p, t);
                    return Results.Ok(notasPaginadas);
                }

                var todasNotas = await service.GetAllOrdensAsync();
                return Results.Ok(todasNotas);
            }
            catch (Exception)
            {
                return Results.BadRequest(new { message = "Erro ao processar notas." });
            }
        });

        // endpoint para buscar uma ordem por Id
        group.MapGet("/{id:long}", async (long id, OrdemService service) =>
        {
            try
            {
                var ordem = await service.GetOrdemByIdAsync(id);

                if (ordem.Equals(null)) return Results.NotFound(new { message = "Ordem não encontrada" });

                return Results.Ok(ordem);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao procurar ordem: {e.Message}");
                return Results.NotFound(new { message = "Erro ao procurar ordem." });
            }
        });

        // endpoint para retornar a quantidade de ordens no banco
        group.MapGet("/count", async (OrdemService service) =>
        {
            try
            {
                var ordens = await service.CountOrdensAsync();
                return Results.Ok(new { count = ordens });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao procurar ordens: {e.Message}");
                return Results.BadRequest(new { message = "Erro ao procurar ordens." });
            }
        });

        // endpoint para atualizar uma ordem
        group.MapPut("/", async (OrdemPetroModel ordem, OrdemService service) =>
        {
            try
            {
                var result = await service.UpdateOrdemAsync(ordem);
                return Results.Ok(new { message = "Ordem atualizada com sucesso: " + result.IdOrdem });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao atualizar ordem: {e.Message}");
                return Results.BadRequest(new { message = "Erro ao atualizar ordem." });
            }
        });

        // endpoint para deletar uma ordem por Id
        group.MapDelete("/{id:long}", async (long id, OrdemService service) =>
        {
            try
            {
                var result = await service.DeleteOrdemAsync(id);
                return Results.Ok(new { message = "Ordem deletada com sucesso" });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao deletar ordem: {e.Message}");
                return Results.BadRequest(new { message = "Erro ao deletar ordem." });
            }
        });
    }
}