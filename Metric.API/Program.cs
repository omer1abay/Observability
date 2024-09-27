using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddOpenTelemetry().WithMetrics(options =>
{
    options.AddMeter("metric.meter.api");
    options.ConfigureResource(resource =>
    {
        resource.AddService("Metric.API", serviceVersion: "1.0.0");
    });
    options.AddPrometheusExporter();
}); //Asp.net configuration


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseSwagger();
app.UseSwaggerUI();
app.UseOpenTelemetryPrometheusScrapingEndpoint(); //endpointimizi ekledik baseURL/meters adýnda bir endpoint oluþturacak bir 
//app.UseHttpsRedirection(); //image'da ssl olmadýðý için kapatýyoruz

app.UseAuthorization();

app.MapControllers();

app.Run();
