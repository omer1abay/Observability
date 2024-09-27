using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Order.API.OrderServices;
using OpenTelemetry.Shared;
using Common.Shared;
using Order.API.Models;
using Microsoft.EntityFrameworkCore;
using Order.API.StockServices;
using Order.API.RedisServices;
using StackExchange.Redis;
using MassTransit;
using Serilog;
using Logging.Shared;
using OpenTelemetry.Logs;

var builder = WebApplication.CreateBuilder(args);
//builder.Host.UseSerilog(Logging.Shared.Logging.ConfigureLogging);
//builder.AddOpenTelemetryLog();

builder.Logging.AddOpenTelemetry(cfg =>
{
    cfg.SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService(builder.Configuration.GetSection("OpenTelemetry")["ServiceName"]!,
                    builder.Configuration.GetSection("OpenTelemetry")["Version"]));

    cfg.AddOtlpExporter((x,y) => { }); //loglarýmýzý artýk new relic'te görebiliyor olacaðýz.
}); //new relic'te rs sürümde bir paket isim çakýþmasý sorunu var bunu çözmek adýna extension metottan çýkarýp program.cs'de tanýmladýk ve boþ bir metotu gösteren delege içine ekledik (5. overload parametre) istediðimiz metotu gösterebilmek için

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<OrderService>(); //repo, dataaccess'ler scope olmak zorunda çünkü dbcontext nesnesi scope'tur. Scoped olunca request'ten response'a dönüþünceye kadar bu servis nesnesi kullanýlýr response oluþunca dispose olur yeni request gelince yeniden oluþur.
builder.Services.AddScoped<StockService>();

//rabbitmq, masstransit 
builder.Services.AddMassTransit(x=>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });
    });
});
builder.Services.AddSingleton(_ =>
{
    //redisi ekledik
    return new RedisService(builder.Configuration);
});
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisService = sp.GetService<RedisService>();

    return redisService.GetConnectionMultiplexer; // bu iþlemler Redis instrumentation için yapýldý..
});
builder.Services.AddHttpClient<StockService>(opt =>
{
    opt.BaseAddress = new Uri(builder.Configuration.GetSection("ApiServices")["StockApi"]!);
});

//otlp için extension metotumuzu ekliyoruz
builder.Services.AddOpenTelemetryExt(builder.Configuration);
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

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
