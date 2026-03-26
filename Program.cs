using System.Reflection;
using api.Communications.Service.Impl.Data;
using app_pm_back.Data;
using app_pm_back.Routes;
using app_pm_back.Services;
using app_pm_back.Services.CongasServices;
using app_pm_back.Services.PetroServices;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

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


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Registra os seus Serviços
builder.Services.AddScoped<ConvertService>();
builder.Services.AddScoped<SpreadsheetService>();
builder.Services.AddScoped<NotesPetroService>();
builder.Services.AddScoped<OrderPetroService>();
builder.Services.AddScoped<NoteCongasService>();
builder.Services.AddScoped<OrderCongasService>();
builder.Services.AddScoped<AnnexService>();

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