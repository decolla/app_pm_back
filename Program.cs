using System.Reflection;
using System.Text.Json;
using api.Communications.Service.Impl.Data;
using app_pm_back.Data;
using app_pm_back.Middlewares;
using app_pm_back.Models;
using app_pm_back.Models.Domain;
using app_pm_back.Routes;
using app_pm_back.Services;
using app_pm_back.Services.Manutencao;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using teste_catboost_cs;

// apagar depois essa gambiarra aqui
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://+:60135");

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
builder.Services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = 209715200; });

// aumenta o limite do servidor para 200 MB
builder.WebHost.ConfigureKestrel(serverOptions => { serverOptions.Limits.MaxRequestBodySize = 209715200; });

// ignora ciclos no json, impedindo de erros de retorno MUDAR DPS
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    // mostra os nomes dos enums (ao invés dos números) no Swagger e na serialização JSON
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// criar o construtor da fonte de dados do PostgreSQL
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

// dar permissão para JSON dinâmico
dataSourceBuilder.EnableDynamicJson();

// criar a fonte de dados
var dataSource = dataSourceBuilder.Build();

// adicionar o contexto do PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(dataSource));

builder.Services.AddScoped<ConvertService>();
builder.Services.AddScoped<SpreadsheetService>();
builder.Services.AddScoped<GenericSearchService>();
builder.Services.AddScoped<NotaManutencaoService>();
builder.Services.AddScoped<OrdemService>();
builder.Services.AddScoped<ItemNotaService>();
builder.Services.AddScoped<PlanoManutencaoService>();
builder.Services.AddScoped<EstrategiaService>();
builder.Services.AddScoped<TipoEstrategiaService>();
builder.Services.AddScoped<TipoManutencaoService>();
builder.Services.AddScoped<TipoPlanoService>();
builder.Services.AddScoped<TipoManutencaoCepService>();
builder.Services.AddScoped<DadosRenovacaoService>();
builder.Services.AddScoped<TarefaService>();
builder.Services.AddScoped<FerramentaService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<CpmService>();
builder.Services.AddScoped<CtmService>();
builder.Services.AddHttpClient<DadosRenovacaoService>();
builder.Services.AddScoped<GpmService>();
builder.Services.AddScoped<ImobilizadoService>();
builder.Services.AddScoped<AlmoxarifadoService>();
builder.Services.AddScoped<ClassificationService<NotaModel>>();
builder.Services.AddScoped<ClassificationService<OrdemModel>>();
builder.Services.AddScoped<AnnexService>();
builder.Services.AddScoped<IReportService, ReportService>();

// configuração de clientes http
builder.Services.AddHttpClient<ExternIntegrationService>(client =>
    {
        // busca a URL do APM
        var urlBase = builder.Configuration.GetValue<string>("APIs:APM");

        if (string.IsNullOrEmpty(urlBase))
        {
            throw new InvalidOperationException(
                "A URL do APM não está configurada. Verifique as configurações de 'APIs:APM'.");
        }

        client.BaseAddress = new Uri(urlBase);

        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.Timeout = TimeSpan.FromSeconds(30); // aborta se demorar mais de 30seg
    }
);

var modelPath = Path.Combine(AppContext.BaseDirectory, "Data", "catboost_model.cbm");
var metaPath = Path.Combine(AppContext.BaseDirectory, "Data", "model_meta.json");

// verifica se os ficheiros existem antes de tentar ler (evita crash no 'dotnet ef')
if (File.Exists(metaPath) && File.Exists(modelPath))
{
    var metaJson = File.ReadAllText(metaPath);
    var modelMeta = JsonSerializer.Deserialize<ModelMeta>(metaJson);
    if (modelMeta != null)
    {
        builder.Services.AddSingleton(modelMeta);
    }

    var catboostModel = new Cbm(modelPath);
    builder.Services.AddSingleton(catboostModel);
}
else
{
    Console.WriteLine("Aviso: Ficheiros do CatBoost não encontrados. O modelo IA não será carregado.");
}

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();
var schema = Environment.GetEnvironmentVariable("SCHEMA") ?? "(not set)";
app.Logger.LogInformation("SCHEMA={Schema}", schema);

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        Console.WriteLine("A tentar aplicar as migrations no banco de dados...");
        // REMOVER DEPOIS: APAGA TODAS AS TABELAS
        // dbContext.Database.ExecuteSqlRaw("DROP SCHEMA public CASCADE; CREATE SCHEMA public;");
        dbContext.Database.Migrate();
        Console.WriteLine("Migrations aplicadas com sucesso!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERRO CRÍTICO NO BANCO]: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"[DETALHE]: {ex.InnerException.Message}");
        }
    }
}

// captura erros
app.UseMiddleware<app_pm_back.Middlewares.GlobalExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

// permite que o angular realize 
app.UseCors("AllowAll");

// app.UseHttpsRedirection();

app.MapControllers();
app.MapSpreadsheetEndpoints();


app.Run();