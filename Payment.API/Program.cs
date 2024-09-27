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

    cfg.AddOtlpExporter((x, y) => { }); //loglarýmýzý artýk new relic'te görebiliyor olacaðýz.
}); //new relic'te rs sürümde bir paket isim çakýþmasý sorunu var bunu çözmek adýna extension metottan çýkarýp program.cs'de tanýmladýk ve boþ bir metotu gösteren delege içine ekledik (5. overload parametre) istediðimiz metotu gösterebilmek için


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
