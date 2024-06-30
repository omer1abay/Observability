// See https://aka.ms/new-console-template for more information
using Observability.ConsoleApp;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using System.Diagnostics;

Console.WriteLine("Hello, World!");

//opentelemetry configuration for console app

//activity listener nadir kullanacağımız senaryolarda kullanılır sadece.
ActivitySource.AddActivityListener(new ActivityListener()
{
    ShouldListenTo=source => source.Name==OpenTelemetryConstants.ActivitySourceNameFile, //bu isme sahip activity'leri dinlemesini sağladık.
    ActivityStarted = activity =>
    {
        //activity başladığı anda bu event tetiklenir ve kodlarımız çalışır (StartActivity() metotu çalıştıktan sonra buraya düşer)
        //buraya istediğimiz kodu yazarız
        Console.WriteLine("Activity başladı");
    },
    ActivityStopped = activity => 
    { 
        //tüm bilgilere 'activity (yukarıda belirttiğimiz Func delegesidir)' üzerinden erişebiliriz
        //activity bittiğinde bu event tetiklenir, metotun ne kadar sürdüğünü bu kısımda anlarız ve db'ye bu event içinde ekleriz.
        Console.WriteLine("Activity bitti"); 
    }


});

using var traceProviderFile = Sdk.CreateTracerProviderBuilder()
                        .AddSource(OpenTelemetryConstants.ActivitySourceNameFile)
                        .Build();


//bu kısıma da using ifadesi eklememiz gerekiyor yoksa jaeger'a data gitmez
using var traceProvider = Sdk.CreateTracerProviderBuilder() 
                        .AddSource(OpenTelemetryConstants.ActivitySourceName)
                        .ConfigureResource(configure =>
                        {
                            configure.AddService(OpenTelemetryConstants.ServiceName, serviceVersion: OpenTelemetryConstants.ServiceVersion)
                            .AddAttributes(new List<KeyValuePair<string, object>>() //opsiyonel ekleyebiliriz
                            {
                                new KeyValuePair<string, object>("host.machineName", Environment.MachineName), //opentelemetry'de standart nokta ile ayırmaktır birden fazla kelimeden oluşan isimlendirmeleri
                                new KeyValuePair<string, object>("host.environment", "dev"),
                                
                            });
                        })
                        .AddConsoleExporter() //console için exporter ekledik
                        .AddOtlpExporter() //bu kod jaeger'a göndermek için yeterli.
                        .AddZipkinExporter(
                            zipkinOptions =>
                            {
                                zipkinOptions.Endpoint = new Uri("http://localhost:9411/api/v2/spans"); //portu elle belirtiyoruz çünkü gideceği portu kendi bilmiyor.
                            } 
                        )
                        .Build();

var serviceHelper = new ServiceHelper();
//await serviceHelper.Work1();
await serviceHelper.Work2();

//kuyruk sistemine bir trace eklerken bir source daha ekleyeceğiz hepsi(db, uygulama, kuyruk vb.) için bir activitysource ekleyip isim vereceğiz
//isimlendirmeler aşırı önemli olmamakla birlikte unique olması lazım















