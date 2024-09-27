using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Elasticsearch;
using System.Runtime.CompilerServices;

namespace Logging.Shared
{
    public static class Logging
    {

        public static void AddOpenTelemetryLog(this WebApplicationBuilder builder)
        {
            builder.Logging.AddOpenTelemetry(cfg =>
            {
                cfg.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(builder.Configuration.GetSection("OpenTelemetry")["ServiceName"]!, 
                                builder.Configuration.GetSection("OpenTelemetry")["Version"]));

                cfg.AddOtlpExporter(); //loglarımızı artık new relic'te görebiliyor olacağız.
            });
        }

        public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogging => (builderContext,
            loggerConfiguration) =>
            {
                var environment = builderContext.HostingEnvironment;

                loggerConfiguration
                .ReadFrom.Configuration(builderContext.Configuration) //direkt olarak serilog'u bulacak
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("Env", environment.EnvironmentName)
                .Enrich.WithProperty("AppName", environment.ApplicationName);

                //log'a traceId'yi merkezi bir yerden vermemiz gerekir her seferinde eklemememiz gerek


                var elasticSearchBaseUrl = builderContext.Configuration.GetSection("Elasticsearch")["BaseUrl"];
                var elasticSearchUsername = builderContext.Configuration.GetSection("Elasticsearch")["UserName"];
                var elasticSearchPassword = builderContext.Configuration.GetSection("Elasticsearch")["Password"];
                var elasticSearchIndexName = builderContext.Configuration.GetSection("Elasticsearch")["IndexName"];

                loggerConfiguration.WriteTo.Elasticsearch(new(new Uri(elasticSearchBaseUrl))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = Serilog.Sinks.Elasticsearch.AutoRegisterTemplateVersion.ESv8,
                    IndexFormat = $"{elasticSearchIndexName}-{environment.EnvironmentName}-logs-" + "{0:yyy.MM.dd}",
                    ModifyConnectionSettings = x => x.BasicAuthentication(elasticSearchUsername,elasticSearchPassword),
                    CustomFormatter = new ElasticsearchJsonFormatter()
                });

            };
    }
}
