using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTelemetry.Shared
{
    public class ActivitySourceProvider
    {
        public static ActivitySource Source = null!;
        //bir activity yani trace data üretmek istediğimizde burada constant olarak verdiğimiz ismi kullanacak bu yüzden bu isim önemlidir.
    }
}
