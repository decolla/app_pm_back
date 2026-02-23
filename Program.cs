using app_pm_back.DTO;
using app_pm_back.Models;
using app_pm_back.Services;
using AutoMapper;
using Microsoft.AspNetCore.Diagnostics;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(option => option.AddPolicy("AllowAngularApp", policy =>
{
    policy.WithOrigins("http://localhost:4200", "http://localhost");
    policy.AllowAnyMethod();
    policy.AllowAnyHeader();
}));
string? connectionString = builder.Configuration.GetSection("MongoDb").GetSection("ConnectionString").Value;
string databaseName = "app_pm_back";

builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
builder.Services.AddScoped<IMongoDatabase>(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));
builder.Services.AddScoped<IMongoCollection<NotaModel>>(sp =>
    sp.GetRequiredService<IMongoDatabase>().GetCollection<NotaModel>("notas"));
builder.Services.AddScoped<NotasService>();
builder.Services.AddScoped<IMongoCollection<OrdemModel>>(sp =>
    sp.GetRequiredService<IMongoDatabase>().GetCollection<OrdemModel>("ordens"));
builder.Services.AddScoped<OrdemService>();

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// captura erros
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var error = context.Features.Get<IExceptionHandlerFeature>();
        if (error != null)
        {
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Ocorreu um erro inesperado.",
                detail = error.Error.Message
            });
        }
    });
});


app.UseSwagger();
app.UseSwaggerUI();

// permite que o angular realize 
app.UseCors("AllowAngularApp");

// app.UseHttpsRedirection();

// endpoint para o cadastro de notas
app.MapPost("app/notas", async (CreateNotaDto dto, IMapper mapper, NotasService service) =>
{
    try
    {
        // transforma o dto em notaModel
        var novaNota = mapper.Map<NotaModel>(dto);
        novaNota.DadosGerais.Status = "ABERTA";

        var notaCriada = await service.CreateNotaAsync(novaNota);
        return Results.Created($"app/notas/{notaCriada.Id}", notaCriada);
    }
    catch (Exception e)
    {
        return Results.BadRequest(new { message = $"Erro ao criar a nota: {e.Message}." });
    }
});

// endpoint que retorna uma lista todas as notas
app.MapGet("app/notas", async (int? pagina, int? tamanho, NotasService service) =>
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
        return Results.BadRequest(new { message = "Erro ao processar notas:." });
    }
});

// endpoint para buscar uma nota por Id
app.MapGet("app/notas/{id}", async (string id, NotasService service) =>
{
    try
    {
        var nota = await service.GetNotaByIdAsync(id);

        if (nota.Equals(null)) return Results.NotFound(new { message = "Nota não encontrada" });

        return Results.Ok(nota);
    }
    catch (Exception e)
    {
        Console.WriteLine($"Erro ao procurar nota: {e.Message}");
        return Results.NotFound(new { message = "Erro ao procurar nota." });
    }
});

// endpoint para retornar a quantidade de notas no banco
app.MapGet("app/notas/count", async (NotasService service) =>
{
    try
    {
        var notas = await service.CountNotasAsync();
        return Results.Ok(new { count = notas });
    }
    catch (Exception e)
    {
        Console.WriteLine($"Erro ao procurar notas: {e.Message}");
        return Results.NotFound(new { message = "Erro ao procurar ordens." });
    }
});

// endpoint para atualizar uma nota
app.MapPut("app/notas", async (NotaModel nota, NotasService service) =>
{
    try
    {
        var result = await service.UpdateNotaAsync(nota);
        return Results.Ok(new { message = "Nota atualizada com sucesso: " + result.Id });
    }
    catch (Exception e)
    {
        Console.WriteLine($"Erro ao atualizar nota: {e.Message}");
        return Results.BadRequest(new { message = "Erro ao atualizar nota." });
    }
});

// endpoint para deletar uma nota por Id
app.MapDelete("app/notas/{id}", async (string id, NotasService service) =>
{
    try
    {
        var result = await service.DeleteNotaAsync(id);
        return Results.Ok(new { message = "Nota deletada com sucesso: " + result.DeletedCount });
    }
    catch (Exception e)
    {
        Console.WriteLine($"Erro ao deletar nota: {e.Message}");
        return Results.BadRequest(new { message = "Erro ao deletar nota." });
    }
});

// ========================================= ORDENS =========================================


// endpoint essencial para o cadastro de ordens
app.MapPost("app/ordem", async (OrdemModel ordemNova, OrdemService service, IMapper mapper) =>
{
    try
    {
        var ordem = await service.CreateOrdemAsync(ordemNova);
        return Results.Created($"app/ordem/{ordem.Id}", ordem);
    }
    catch (Exception e)
    {
        Console.WriteLine($"Erro ao criar a ordem: {e.Message}");
        return Results.BadRequest(new { message = "Erro ao criar a ordem." });
    }
});

// endpoint que retorna uma lista todas as ordens
app.MapGet("app/ordem", async (int? pagina, int? tamanho, OrdemService service) =>
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
app.MapGet("app/ordem/{id}", async (string id, OrdemService service) =>
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
app.MapGet("app/ordem/count", async (OrdemService service) =>
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
app.MapPut("app/ordem", async (OrdemModel ordem, OrdemService service) =>
{
    try
    {
        var result = await service.UpdateOrdemAsync(ordem);
        return Results.Ok(new { message = "Ordem atualizada com sucesso: " + result.Id });
    }
    catch (Exception e)
    {
        Console.WriteLine($"Erro ao atualizar ordem: {e.Message}");
        return Results.BadRequest(new { message = "Erro ao atualizar ordem." });
    }
});

// endpoint para deletar uma ordem por Id
app.MapDelete("app/ordem/{id}", async (string id, OrdemService service) =>
{
    try
    {
        var result = await service.DeleteOrdemAsync(id);
        return Results.Ok(new { message = "Ordem deletada com sucesso: " + result.DeletedCount });
    }
    catch (Exception e)
    {
        Console.WriteLine($"Erro ao deletar ordem: {e.Message}");
        return Results.BadRequest(new { message = "Erro ao deletar ordem." });
    }
});


app.Run();