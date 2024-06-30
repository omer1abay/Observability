using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.ConsoleApp
{
    internal class ActivitySourceProvider
    {
        public static ActivitySource Source = new ActivitySource(OpenTelemetryConstants.ActivitySourceName);
        public static ActivitySource SourceFile = new ActivitySource(OpenTelemetryConstants.ActivitySourceNameFile);
        //bir activity yani trace data üretmek istediğimizde burada constant olarak verdiğimiz ismi kullanacak bu yüzden bu isim önemlidir.
    }
}
