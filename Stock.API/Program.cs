using Stock.API.Services;
using OpenTelemetry.Shared;
using Common.Shared;
using MassTransit;
using Stock.API.Consumers;
using Logging.Shared;
using Serilog;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

//builder.Host.UseSerilog(Logging.Shared.Logging.ConfigureLogging);
//builder.AddOpenTelemetryLog(); //new relic impelementasyonu

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

builder.Services.AddScoped<StockService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddOpenTelemetryExt(builder.Configuration);

builder.Services.AddHttpClient<PaymentService>(opt =>
{
    opt.BaseAddress = new Uri(builder.Configuration.GetSection("ApiServices")["PaymentApi"]!);
});

//rabbitmq, masstransit 
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });

        cfg.ReceiveEndpoint("stock.order-created-event.queue", e =>
        {
            e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
        });
    });
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
