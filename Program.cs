using Microsoft.EntityFrameworkCore;

using NotesMinimalAPI.app;
using NotesMinimalAPI.ctl;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON logging to the console.
//builder.Logging.AddJsonConsole();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<appContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "[object Object]");

app.MapGet("/usluga", appController.GetUslugi);
app.MapPost("/usluga", appController.AddUsluga);
app.MapGet("/usluga/{usl:int}", appController.GetUsluga);
app.MapPut("/usluga/{usl:int}", appController.UpdUsluga);
app.MapDelete("/usluga/{usl:int}", appController.DelUsluga);

app.MapGet("/tariff", appController.GetTariffs);
app.MapGet("/tariff/{id:int}", appController.GetTariff);
app.MapPut("/tariff/{id:int}", appController.UpdTariff);
app.MapDelete("/tariff/{id:int}", appController.DelTariff);
app.MapGet("/usluga/null/tariff", appController.GetTariffs);
app.MapGet("/usluga/undefined/tariff", appController.GetTariffs);
app.MapGet("/usluga/{usl:int}/tariff", appController.GetTariffsByUsluga);
app.MapPost("/usluga/{usl:int}/tariff", appController.AddTariff);

//app.MapGet("/usluga/{usl:int}/tariff/{date}", appController.GetTariffByDate);

app.MapGet("/counter", appController.GetCounters);
app.MapGet("/counter/{id:int}", appController.GetCounter);
app.MapPut("/counter/{id:int}", appController.UpdCounter);
app.MapDelete("/counter/{id:int}", appController.DelCounter);
app.MapGet("/usluga/null/counter", appController.GetCounters);
app.MapGet("/usluga/undefined/counter", appController.GetCounter);
app.MapGet("/usluga/{usl:int}/counter", appController.GetCountersByUsluga);
app.MapPost("/usluga/{usl:int}/counter", appController.AddCounter);

app.MapGet("/measure", appController.GetMeasures);
app.MapGet("/measure/{id:int}", appController.GetMeasure);
app.MapPut("/measure/{id:int}", appController.UpdMeasure);
app.MapDelete("/measure/{id:int}", appController.DelMeasure);
app.MapGet("/counter/null/measure", appController.GetMeasures);
app.MapGet("/counter/undefined/measure", appController.GetMeasure);
app.MapGet("/counter/{cnt:int}/measure", appController.GetMeasuresByCounter);
app.MapPost("/counter/{cnt:int}/measure", appController.AddMeasure);

await app.RunAsync();
