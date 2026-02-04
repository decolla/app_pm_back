using app_pm_back.Models;
using app_pm_back.Services;
using Microsoft.AspNetCore.Diagnostics;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddCors(option => option.AddPolicy("AllowAngularApp", policy =>
{
    policy.WithOrigins("http://localhost:4200", "http://localhost");
    policy.AllowAnyMethod();
    policy.AllowAnyHeader();
}));
string connectionString = builder.Configuration.GetSection("MongoDb").GetSection("ConnectionString").Value;
Console.WriteLine(connectionString);
string databaseName = "app_pm_back";
string collectionName = "pm_data";

builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
builder.Services.AddScoped<IMongoDatabase>(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));
builder.Services.AddScoped<IMongoCollection<NotaModel>>(sp =>
    sp.GetRequiredService<IMongoDatabase>().GetCollection<NotaModel>(collectionName));
builder.Services.AddScoped<NotasService>();

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

app.UseHttpsRedirection();

// endpoint essencial para o cadastro de ativos
app.MapPost("/notas", async (NotaModel novaNota, app_pm_back.Services.NotasService service) =>
{
    var nota = await service.CreateNotaAsync(novaNota);

    if (nota == null)
    {
        return Results.BadRequest(new { message = "Erro ao criar o ativo." });
    }
    else
    {
        return Results.Created($"/notas/{nota}", nota);
    }
});

app.MapGet("/notas", async (app_pm_back.Services.NotasService service) =>
{
    try
    {
        var notas = await service.GetAllNotasAsync();
        return Results.Ok(notas);
    }
    catch (Exception e)
    {
        Console.WriteLine($"Erro ao procurar notas: {e.Message}");
        throw;
    }
});

app.MapGet("/notas/search", async (string id ,app_pm_back.Services.NotasService service) =>
{
    try
    {
        var nota = await service.GetNotaByIdAsync(id);
        return Results.Ok(nota);
    }
    catch (Exception e)
    {
        Console.WriteLine($"Erro ao procurar notas: {e.Message}");
        throw;
    }
});

app.MapGet("/notas/count", async (app_pm_back.Services.NotasService service) =>
{
    try
    {
        var notas = await service.CountNotasAsync();
        return Results.Ok(new { count = notas });
    }
    catch (Exception e)
    {
        Console.WriteLine($"Erro ao procurar notas: {e.Message}");
        throw;
    }
});

app.MapPut("/notas", async (NotaModel nota, app_pm_back.Services.NotasService service) =>
{
    try
    {
        var result = await service.UpdateNotaAsync(nota);
        return Results.Ok(new { message = "Nota atualizada com sucesso: " + result.IdMongo });
    }
    catch (Exception e){
        Console.WriteLine($"Erro ao atualizar nota: {e.Message}");
        return Results.BadRequest(new { message = "Erro ao atualizar nota." });
    }
});

app.MapDelete("/notas", async (string id, app_pm_back.Services.NotasService service) =>
{
    try
    {
        var result = await service.DeleteNotaAsync(id);
        return Results.Ok(new { message = "Nota deletada com sucesso: " + result.DeletedCount });
    }
    catch (Exception e){
        Console.WriteLine($"Erro ao deletar nota: {e.Message}");
        return Results.BadRequest(new { message = "Erro ao deletar nota." });
    }
});

app.MapControllers();

app.Run();