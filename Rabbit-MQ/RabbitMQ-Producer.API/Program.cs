using Prometheus;
using RabbitMQ_Producer.API;
using Commons;
var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddLogs();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<Producer>();
builder.Services.AddSingleton<Listen>();
builder.Services.AddHostedService<Job>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

// Configurar o middleware para expor métricas no endpoint "/metrics"
app.UseMetricServer();

// Incluir métricas de requisição HTTP
app.UseHttpMetrics();

app.MapControllers();

var listen = app.Services.GetRequiredService<Listen>();
listen.StartConsuming();

app.Run();
