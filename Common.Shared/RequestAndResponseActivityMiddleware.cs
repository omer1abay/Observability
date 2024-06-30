using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Shared
{
    public class RequestAndResponseActivityMiddleware
    {

        private readonly RequestDelegate _next;

        private readonly RecyclableMemoryStreamManager _memoryStreamManager;

        public RequestAndResponseActivityMiddleware(RequestDelegate next)
        {
            _next = next;
            _memoryStreamManager = new RecyclableMemoryStreamManager();
        }


        //request ve response'u daha verimli daha hızlı okuyabilmek için bir paket kullanacağız memory'i daha performanslı kullanacağız bu sayede çünkü stream üzerinden okuyoruz
        //Paket Adı : Microsoft.IO.RecyclableMemoryStream -> bu paket üzerinden stream edeceğiz. mem allocation için performanslıdır

        public async Task InvokeAsync(HttpContext context)
        {
            await AddRequestBodyContentToActivityTag(context);
            await AddResponseBodyContentToActivityTag(context);
        }


        private async Task AddResponseBodyContentToActivityTag(HttpContext context)
        {
            //middleware'de next'ten önce request işlenir

            var originalResponse = context.Response.Body;

            await using var responseBodyStream = _memoryStreamManager.GetStream();
            context.Response.Body = responseBodyStream; //boş bir stream verdik hala request durumundayız sayesinde memory'de aynı yeri işaret etmesini sağlıyoruz body dolduğu anda hepsi dolacak

            await _next(context);

            //next'ten sonra response işlenir
            responseBodyStream.Position = 0;
            var responseBodyStreamReader = new StreamReader(responseBodyStream);
            var responseBody = await responseBodyStreamReader.ReadToEndAsync();

            Activity.Current?.SetTag("http.response.body", responseBody);
            responseBodyStream.Position = 0;

            await responseBodyStream.CopyToAsync(originalResponse);

            //body'i dolaylı yoldan okuyoruz çünkü eğer direkt okursak uygulamanın başka bir yerinde okuyamayız biz burada bir stream'e aldık body'i bu şekilde okuduk.


        }

        private async Task AddRequestBodyContentToActivityTag(HttpContext context)
        {
            //request'i birden fazla kez okuyabileceğimiz için buffer'ladık
            context.Request.EnableBuffering();

            var requestBodyStreamReader = new StreamReader(context.Request.Body);
            
            var requestBody = await requestBodyStreamReader.ReadToEndAsync();

            Activity.Current?.SetTag("http.request.body",requestBody);

            context.Request.Body.Position = 0; //yeniden okunabileceği için pozisyonunu 0 yaptık binary data olduğu için yeniden ilk veriye gelmesini sağladık.
        }
    }
}
