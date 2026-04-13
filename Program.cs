using System.Reflection;
using System.Text.Json;
using api.Communications.Service.Impl.Data;
using app_pm_back.Data;
using app_pm_back.Models;
using app_pm_back.Models.CongasModel;
using app_pm_back.Routes;
using app_pm_back.Services;
using app_pm_back.Services.CongasServices;
using app_pm_back.Services.PetroServices;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using teste_catboost_cs;

// apagar depois essa gambiarra aqui
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

builder.Services.AddCors(option => option.AddPolicy("AllowAll", policy =>
{
    // policy.WithOrigins("http://localhost:4200", "http://localhost");
    
    policy.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
}));

// aumenta o limite de ficheiros multipart para 200 MB
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 209715200;
});

// aumenta o limite do servidor para 200 MB
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 209715200;
});

// ignora ciclos no json, impedindo de erros de retorno MUDAR DPS
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// criar o construtor da fonte de dados do PostgreSQL
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

// dar permissão para JSON dinâmico
dataSourceBuilder.EnableDynamicJson();

// criar a fonte de dados
var dataSource = dataSourceBuilder.Build();

// adicionar o contexto do PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dataSource));

builder.Services.AddScoped<ConvertService>();
builder.Services.AddScoped<SpreadsheetService>();
builder.Services.AddScoped<NotesPetroService>();
builder.Services.AddScoped<OrderPetroService>();
builder.Services.AddScoped<NoteCongasService>();
builder.Services.AddScoped<OrderCongasService>();
builder.Services.AddScoped<ItemNoteCongasService>();
builder.Services.AddScoped<EquipmentService>();
builder.Services.AddScoped<AcaoNotaComgasService>();
builder.Services.AddScoped<ClassificationService<NoteCongasModel>>();
builder.Services.AddScoped<ClassificationService<NotaPetroModel>>();
builder.Services.AddScoped<ClassificationService<OrderCongasModel>>();
builder.Services.AddScoped<ClassificationService<OrdemPetroModel>>();
builder.Services.AddScoped<AnnexService>();

var modelPath = Path.Combine(AppContext.BaseDirectory, "Data", "catboost_model.cbm");
var metaPath = Path.Combine(AppContext.BaseDirectory, "Data", "model_meta.json");

// verifica se os ficheiros existem antes de tentar ler (evita crash no 'dotnet ef')
if (File.Exists(metaPath) && File.Exists(modelPath))
{
    var metaJson = File.ReadAllText(metaPath);
    var modelMeta = JsonSerializer.Deserialize<ModelMeta>(metaJson);
    builder.Services.AddSingleton(modelMeta);

    var catboostModel = new Cbm(modelPath);
    builder.Services.AddSingleton(catboostModel);
}
else
{
    Console.WriteLine("Aviso: Ficheiros do CatBoost não encontrados. O modelo IA não será carregado.");
}



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
app.UseCors("AllowAll");

// app.UseHttpsRedirection();

app.MapControllers();
app.MapSpreadsheetEndpoints();

app.Run();
