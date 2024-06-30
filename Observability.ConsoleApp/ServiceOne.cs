using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.ConsoleApp
{
    internal class ServiceOne
    {
        static HttpClient httpClient = new HttpClient(); //static veya singleton olmalı tek instance üzerinden çalışmalıyız yoksa socket yetersiz hatası alırız ve kesinlikle new'leme DI container'dan al
        public async Task<int> MakeRequestToGoogle()
        {
            using var activity = ActivitySourceProvider.Source.StartActivity(kind: System.Diagnostics.ActivityKind.Server, name: "MakeRequestToGoogle");
            try
            {
                
                var tags = new ActivityTagsCollection();
                tags.Add("isim", "ömer");
                activity.AddEvent(new("google'a istek başladı", tags: tags)); //event ekleme
                activity.AddTag("request.scheme","https"); //tag ekleme
                activity.AddTag("request.method","get");
                var result = await httpClient.GetAsync("https://www.google.com");
                var responseContent = await result.Content.ReadAsStringAsync();
                activity.AddTag("response.length", responseContent.Length);
                tags.Add("length", responseContent.Length);
                activity.AddEvent(new("google'a istek tamamlandı", tags: tags));
                
                var serviceTwo = new ServiceTwo();
                var fileLength = serviceTwo.WriteToFile("Hello world");

                return responseContent.Length;
            }
            catch (Exception e)
            {
                activity.SetStatus(ActivityStatusCode.Error, e.Message); //activity'i error olarak set ettik.
                return -1;
            }
        }
    }
}
