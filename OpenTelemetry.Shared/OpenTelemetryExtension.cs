using MassTransit.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace OpenTelemetry.Shared
{
    public static class OpenTelemetryExtension
    {

        //builder.Services için extension method
        public static void AddOpenTelemetryExt(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OpenTelemetryConstants>(configuration.GetSection("OpenTelemetry"));
            var openTelemetryConstants = (configuration.GetSection("OpenTelemetry").Get<OpenTelemetryConstants>())!;

            ActivitySourceProvider.Source = new System.Diagnostics.ActivitySource(openTelemetryConstants.ActivitySourceName);
            
            services.AddOpenTelemetry().WithTracing(options =>
            {
                options.AddSource(openTelemetryConstants.ActivitySourceName)
                .AddSource(DiagnosticHeaders.DefaultListenerName) //mass transit için source
                .ConfigureResource(resource =>
                {
                    resource.AddService(openTelemetryConstants.ServiceName, serviceVersion: openTelemetryConstants.ServiceName);
                });

                options.AddAspNetCoreInstrumentation(opt =>
                {
                    opt.Filter = (context) => context.Request.Path.Value.Contains("api", StringComparison.InvariantCulture); //sadece endpoint'leri trace etmesi için filter ekledik bu filter ile her endpoint'imiz url'i api içermeli yoksa api içermeyeni de trace etmez. (api/[controller]) 
                    opt.RecordException = true; //hatanın tüm detayları ile kayıt edilmesi içindir. yoksa çok küçük bir açıklama yazar. Eğer loglarda tutuyorsak true set etmeye gerek yoktur ekstra maliyetten kurtulmak adına
                });

                options.AddEntityFrameworkCoreInstrumentation(opt =>
                {
                    opt.SetDbStatementForText = true;
                    opt.SetDbStatementForStoredProcedure = true; //sp'leri kayıt edeyim mi?
                    opt.EnrichWithIDbCommand = (activity, dbCommand) => //güncel activity'i ve db'ye giden data'yı temsil eder
                    { 
                           
                    }; //ekstra dataların kayıt edilmesi, datayı her span (actitivity) olarak kaydettiğinde bu metot tetiklenir bu metot içinde istediğimiz gibi manipülasyon veya ek bilgi eklemesi yapabiliriz. aspnetcore içinde de vardır exception'lar için
                });

                options.AddHttpClientInstrumentation(opt =>
                {
                    opt.EnrichWithHttpRequestMessage = async (activity, request) =>
                    {
                        //request'i alacağız
                        var reqContent = string.Empty;

                        if (request.Content != null)
                        {
                            reqContent = await request.Content.ReadAsStringAsync();
                        }

                        activity.SetTag("http.request.body", reqContent); 

                    };

                    opt.EnrichWithHttpResponseMessage = async (activity, response) =>
                    {
                        // response'u alacğaız

                        var resContent = string.Empty;

                        if (response.Content != null) //kontrolün sebebi bazı get işlemlerinde response boş gider.
                        {
                            resContent = await response.Content.ReadAsStringAsync();

                            activity.SetTag("http.response.body", resContent);
                        }

                    };

                });

                options.AddRedisInstrumentation(opt => //redis instrumentation di container üzerinden okuyabileceği bir connection_multiplexer ister. Bu durumda container'a singleton olarak bir IConnectionMultiplexer geçmemiz gerek.
                {
                    opt.SetVerboseDatabaseStatements = true;
                });


                options.AddConsoleExporter(); //console'a yazdıracak
                options.AddOtlpExporter(); // Jaeger'a yazdıracak, neden Jaeger adı geçmiyor diye sorarsak Jaeger default olarak otlp'i native olarak destekler. Daha best practice bir yaklaşımdır jaeger
            });


            
        }

    }
}