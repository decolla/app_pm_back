using System.Reflection;
using System.Text.Json;
using api.Communications.Service.Impl.Data;
using app_pm_back.Data;
using app_pm_back.DTO;
using app_pm_back.Interfaces;
using app_pm_back.Middlewares;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using app_pm_back.Services.ExternIntegationsServices;
using app_pm_back.Services.Manutencao;
using app_pm_back.Services.FichasServices;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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
    c.CustomSchemaIds(type => type.FullName ?? type.Name);
});

builder.Services.AddCors(option => option.AddPolicy("AllowAll", policy =>
{
    // policy.WithOrigins("http://localhost:4200", "http://localhost");

    policy.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
}));

// aumenta o limite de ficheiros multipart para 200 MB
builder.Services.Configure<FormOptions>(options => 
{ 
    options.MultipartBodyLengthLimit = 209715200; // 200 MB de limite de tamanho
    options.MemoryBufferThreshold = 209715200;    // Mantém arquivos de até 200MB 100% na Memória RAM
});

// aumenta o limite do servidor para 200 MB
builder.WebHost.ConfigureKestrel(serverOptions => { serverOptions.Limits.MaxRequestBodySize = 209715200; });

// ignora ciclos no json, impedindo de erros de retorno MUDAR DPS
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    // mostra os nomes dos enums (ao invés dos números) no Swagger e na serialização JSON
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

//  Memory Cache para LinhaTemporal
builder.Services.AddMemoryCache();

var databaseSchema = app_pm_back.Config.RouteConfig.GetDatabaseSchema();

if (databaseSchema == "cg")
{
    var connectionString = builder.Configuration.GetConnectionString("DBConnection")
                           ?? builder.Configuration.GetConnectionString("DefaultConnection");

    // ef core para sql server
    builder.Services.AddDbContext<AppDbContext, SqlServerDbContext>(options =>
        options.UseSqlServer(connectionString));

    // config hangfire para sql server
    builder.Services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(connectionString));
}
else
{
    var targetSchema = string.IsNullOrWhiteSpace(databaseSchema) ? "public" : databaseSchema;
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION")
                           ?? builder.Configuration.GetConnectionString("DefaultConnection");

    // criar o construtor da fonte de dados do PostgreSQL
    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

    // dar permissão para JSON dinâmico
    dataSourceBuilder.EnableDynamicJson();

    //Força o search path a usar o targetschema em todas as conexxões
    dataSourceBuilder.ConnectionStringBuilder["options"] = $"-c search_path=\"{targetSchema}\",public";

    // criar a fonte de dados
    var dataSource = dataSourceBuilder.Build();

    // adicionar o contexto do PostgreSQL
    builder.Services.AddDbContext<AppDbContext, PostgresDbContext>((sp, options) =>
    {
        options.UseNpgsql(dataSource,
                npgsqlOptions => { npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", targetSchema); })
            .AddInterceptors(
                new DatabaseExceptionInterceptor(),
                new AuditInterceptor(sp.GetRequiredService<IUserContext>()));
    });

    // config hangfire 
    builder.Services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(c =>
                c.UseNpgsqlConnection(connectionString),
            new PostgreSqlStorageOptions
            {
                SchemaName = targetSchema
            }));
}

builder.Services.AddHangfireServer();

builder.Services.AddTransient<app_pm_back.Jobs.ManutencaoGeneratorJob>();

builder.Services.AddScoped<ConvertService>();
builder.Services.AddScoped<SpreadsheetService>();
builder.Services.AddScoped<ImportSheetService>();
builder.Services.AddScoped<GenericSearchService>();
builder.Services.AddScoped<NotaService>();
builder.Services.AddScoped<OrdemService>();
builder.Services.AddScoped<ItemNotaService>();
builder.Services.AddScoped<VariavelNotaService>();
builder.Services.AddScoped<PlanoManutencaoService>();
builder.Services.AddScoped<EstrategiaService>();
builder.Services.AddScoped<TipoEstrategiaService>();
builder.Services.AddScoped<TipoManutencaoService>();
builder.Services.AddScoped<TipoPlanoService>();
builder.Services.AddScoped<TipoManutencaoCepService>();
builder.Services.AddScoped<DadosRenovacaoService>();
builder.Services.AddScoped<OperacaoService>();
builder.Services.AddScoped<FerramentaService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<UserContextService>();
builder.Services.AddScoped<CpmService>();
builder.Services.AddScoped<CtmService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<SynchronizeAPMService>();
builder.Services.AddScoped<GpmService>();
builder.Services.AddScoped<ImobilizadoService>();
builder.Services.AddScoped<AlmoxarifadoService>();
builder.Services.AddScoped<ClassificationService<NotaModel>>();
builder.Services.AddScoped<ClassificationService<OrdemModel>>();
builder.Services.AddScoped<AnnexService>();
builder.Services.AddScoped<ConfiguracaoGlobalService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<LinhaTemporalService>();
builder.Services.AddScoped<ReliabilityService>();
builder.Services.AddScoped<AuditoriaService>();
builder.Services.AddScoped<RegraClassificacaoService>();
builder.Services.AddScoped<DiretorioService>();

// configuração de clientes http
builder.Services.AddHttpClient<ExternIntegrationService>(client =>
    {
        // busca a URL do APM 
        var urlBase = app_pm_back.Config.RouteConfig.GetApmBaseUrl(builder.Configuration);

        if (string.IsNullOrEmpty(urlBase))
        {
            throw new InvalidOperationException(
                "A URL do APM não está configurada. Verifique as configurações de 'APIs:APM'.");
        }

        if (!urlBase.EndsWith("/"))
        {
            urlBase += "/";
        }

        client.BaseAddress = new Uri(urlBase);

        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.Timeout = TimeSpan.FromSeconds(30); // aborta se demorar mais de 30seg
    }
);

builder.Services.AddHttpClient<ReliabilityService>(client =>
{
    // busca a URL do reliability
    var urlBase = app_pm_back.Config.RouteConfig.GetReliabilityBaseUrl(builder.Configuration);

    if (string.IsNullOrEmpty(urlBase))
        throw new InvalidOperationException(
            "A URL do Reliability não está configurada. Verifique as configurações de 'APIs:Reliability'.");

    if (!urlBase.EndsWith("/"))
    {
        urlBase += "/";
    }

    client.BaseAddress = new Uri(urlBase);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

var modelPath = Path.Combine(AppContext.BaseDirectory, "CatBoost", "catboost_model.cbm");
var metaPath = Path.Combine(AppContext.BaseDirectory, "CatBoost", "model_meta.json");

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
builder.Services.AddHttpContextAccessor();

// sempre que uma requisição chegar na API, trará um Bearer token
builder.Services.AddScoped<IUserContext, UserContextService>();

var app = builder.Build();
var schema = Environment.GetEnvironmentVariable("SCHEMA") ?? "(not set)";

app.Logger.LogInformation("SCHEMA={Schema}", schema);

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        var dbSchema = app_pm_back.Config.RouteConfig.GetDatabaseSchema();

        if (dbSchema == "cg")
        {
            Console.WriteLine("Ambiente 'cg' detectado. A tentar criar o banco de dados e as tabelas...");
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("Banco de dados e tabelas criados/verificados com sucesso!");
        }
        else
        {
            Console.WriteLine("PostgreSQL detectado. A tentar aplicar as migrations no banco de dados...");
            // REMOVER DEPOIS: APAGA TODAS AS TABELAS
            // schema = "public"; // força o schema para public, para não apagar o schema correto
            // dbContext.Database.ExecuteSqlRaw($"DROP SCHEMA {schema} CASCADE; CREATE SCHEMA {schema};");
             dbContext.Database.ExecuteSqlRaw($"DROP SCHEMA comgas CASCADE; CREATE SCHEMA comgas;");

            // Garante que a conexão é aberta com o search_path configurado antes do Migrate()
            var conn = dbContext.Database.GetDbConnection();
            await conn.OpenAsync();

            await dbContext.Database.MigrateAsync();

            await conn.CloseAsync();
            Console.WriteLine("Migrations aplicadas com sucesso!");
        }
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

// dashboard do hangfire
app.UseHangfireDashboard();

// rotina de verificação nos planos de manutenção
RecurringJob.AddOrUpdate<app_pm_back.Jobs.ManutencaoGeneratorJob>(
    "gerador-fichas",
    service => service.RunDailyRoutine(),
    Cron.Daily(2, 0));

// captura erros
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
    {
        // Intercepta o cabeçalho enviado pelo seu Ingress do Kubernetes
        var basePath = httpReq.Headers["X-Forwarded-Prefix"].FirstOrDefault();

        if (!string.IsNullOrEmpty(basePath))
        {
            // Ajusta a URL base do servidor dinamicamente
            swaggerDoc.Servers = new List<OpenApiServer>
            {
                new OpenApiServer { Url = basePath }
            };
        }
    });
});
app.UseSwaggerUI();

// permite que o angular realize 
app.UseCors("AllowAll");

// app.UseHttpsRedirection();

app.MapControllers();


app.Run();