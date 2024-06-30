using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.ConsoleApp
{
    public static class OpenTelemetryConstants
    {
        public const string ServiceName = "ConsoleApp"; //"CompanyName.AppName.ComponentName";
        public const string ServiceVersion = "1.0.0";
        public const string ActivitySourceName = "ActivitySource.ConsoleApp";
        public const string ActivitySourceNameFile = "ActivitySource.ConsoleApp.File";
        public const string ServiceNameFile = "ConsoleAppFile";

    }
}
