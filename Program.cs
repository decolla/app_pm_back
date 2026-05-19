using System.Reflection;
using System.Text.Json;
using api.Communications.Service.Impl.Data;
using app_pm_back.Data;
using app_pm_back.Models;
using app_pm_back.Models.CongasModel;
using app_pm_back.Models.Domain;
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
builder.Services.AddScoped<GenericSearchService>();
builder.Services.AddScoped<NotesPetroService>();
builder.Services.AddScoped<OrderPetroService>();
builder.Services.AddScoped<NotaManutencaoService>();
builder.Services.AddScoped<OrderCongasService>();
builder.Services.AddScoped<ItemNoteCongasService>();
builder.Services.AddScoped<EquipmentService>();
// builder.Services.AddScoped<AcaoNotaComgasService>();
builder.Services.AddScoped<PlanoManutencaoService>();
builder.Services.AddScoped<EstrategiaComgasService>();
builder.Services.AddScoped<TipoEstrategiaService>();
builder.Services.AddScoped<TipoManutencaoService>();
builder.Services.AddScoped<TipoManutencaoCepService>();
builder.Services.AddScoped<TarefaService>();
builder.Services.AddScoped<FerramentaTipoService>();
builder.Services.AddScoped<UsuarioSapService>();
builder.Services.AddScoped<FabricanteService>();
builder.Services.AddScoped<EnderecoService>();
builder.Services.AddScoped<LocalizacaoService>();
builder.Services.AddScoped<CentroPlanejamentoManutencaoService>();
builder.Services.AddScoped<CentroTrabalhoService>();
builder.Services.AddScoped<GrupoPlanejamentoManutencaoService>();
builder.Services.AddScoped<ImobilizadoService>();
builder.Services.AddScoped<LocalInstalacaoService>();
builder.Services.AddScoped<ComponenteEquipamentoService>();
builder.Services.AddScoped<CicloManutencaoService>();
builder.Services.AddScoped<ItemPlanoManutencaoService>();
builder.Services.AddScoped<MedidaNotaService>();
builder.Services.AddScoped<AtividadeNotaService>();
builder.Services.AddScoped<AnaliseRiscoService>();
builder.Services.AddScoped<AnaliseRiscoOperacionalService>();
builder.Services.AddScoped<PermissaoTrabalhoService>();
builder.Services.AddScoped<MatrizLibraService>();
builder.Services.AddScoped<OperacaoOrdemService>();
builder.Services.AddScoped<FuncionarioService>();
builder.Services.AddScoped<SetorVendasService>();
builder.Services.AddScoped<AlmoxarifadoService>();
builder.Services.AddScoped<ClassificationService<NotaManutencaoModel>>();
builder.Services.AddScoped<ClassificationService<NotaPetroModel>>();
builder.Services.AddScoped<ClassificationService<OrdemManutencaoModel>>();
builder.Services.AddScoped<ClassificationService<OrdemPetroModel>>();
builder.Services.AddScoped<AnnexService>();
builder.Services.AddScoped<IReportService, ReportService>();

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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        Console.WriteLine("A tentar aplicar as migrations no banco de dados...");
        // REMOVER DEPOIS: APAGA TODAS AS TABELAS
        dbContext.Database.ExecuteSqlRaw("DROP SCHEMA public CASCADE; CREATE SCHEMA public;");
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
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        string mensagem = "Ocorreu um erro inesperado.";
        string detalhe = error?.Message ?? "";

        // Se o erro for do Banco de Dados
        if (error is DbUpdateException dbEx && dbEx.InnerException is PostgresException pgEx)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest; // Muda para 400 Bad Request

            if (pgEx.SqlState == "23503")
            {
                mensagem = "Erro de Relacionamento: O registo principal (pai) informado não existe.";
            }
            else if (pgEx.SqlState == "23505")
            {
                mensagem = "Erro de Duplicidade: Este registo já existe no sistema.";
            }
            else
            {
                mensagem = "Erro de integridade no banco de dados.";
            }

            detalhe = pgEx.MessageText;
        }

        await context.Response.WriteAsJsonAsync(new
        {
            message = mensagem,
            detail = detalhe
        });
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