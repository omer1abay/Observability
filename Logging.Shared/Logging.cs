using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Elasticsearch;

namespace Logging.Shared
{
    public class Logging
    {
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
