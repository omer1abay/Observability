using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.ConsoleApp
{
    internal class ServiceHelper
    {
        internal async Task Work1()
        {
            //eskiden kullanım olarak stopwatch vardı.
            //activity oluşturuyoruz ve ürettiği data'yı export etmemiz lazım bunun için bir paket daha lazım console için OpenTelemetry.Exporter.Console yükleyeceğiz.
            using var activity = ActivitySourceProvider.Source.StartActivity(); //bu şekilde yapılan kullanımda using scope olarak metotun tamamını alır.
            var serviceOne = new ServiceOne();
            Console.WriteLine($"google response length : {await serviceOne.MakeRequestToGoogle()}");
            Console.WriteLine("Ömer Abay");
        }


        internal async Task Work2()
        {
            //eskiden kullanım olarak stopwatch vardı.
            //activity oluşturuyoruz ve ürettiği data'yı export etmemiz lazım bunun için bir paket daha lazım console için OpenTelemetry.Exporter.Console yükleyeceğiz.
            using var activity = ActivitySourceProvider.SourceFile.StartActivity(); //burada SourceFile üzerinde üretilen activity'leri biz ele alıyoruz

            activity.SetTag("work 2 tag","value");
            activity.AddEvent(new ActivityEvent("work 2 event"));

        }

    }
}
