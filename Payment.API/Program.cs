using Common.Shared;
using Logging.Shared;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Shared;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
//builder.Host.UseSerilog(Logging.Shared.Logging.ConfigureLogging);
//builder.AddOpenTelemetryLog();

builder.Logging.AddOpenTelemetry(cfg =>
{
    cfg.SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService(builder.Configuration.GetSection("OpenTelemetry")["ServiceName"]!,
                    builder.Configuration.GetSection("OpenTelemetry")["Version"]));

    cfg.AddOtlpExporter((x, y) => { }); //loglar�m�z� art�k new relic'te g�rebiliyor olaca��z.
}); //new relic'te rs s�r�mde bir paket isim �ak��mas� sorunu var bunu ��zmek ad�na extension metottan ��kar�p program.cs'de tan�mlad�k ve bo� bir metotu g�steren delege i�ine ekledik (5. overload parametre) istedi�imiz metotu g�sterebilmek i�in


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenTelemetryExt(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<OpenTelemetryTraceIdMiddleware>();
app.UseMiddleware<RequestAndResponseActivityMiddleware>();
app.UseExceptionMiddleware();
app.UseAuthorization();

app.MapControllers();

app.Run();
