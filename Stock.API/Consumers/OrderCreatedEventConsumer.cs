using Common.Shared.Events;
using MassTransit;
using System.Diagnostics;
using System.Text.Json;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreateEvent>
    {
        public Task Consume(ConsumeContext<OrderCreateEvent> context)
        {

            Thread.Sleep(2000);

            #region MassTransit Instrumentation paketi yüklenmeden önce
            //şu an bu boş olacak çünkü aspnetcore instrumentation paketi stock apisine istek geldiğinde tetikleniyor burada bir istek yok mesaj kuyruk sistemi var
            var a = Activity.Current?.SetTag("messagge.body", JsonSerializer.Serialize(context.Message)); //şu durumda bunlar birbirinden bağımsız aktiviteler aktiviteleri bağlama işlemini instrumentation paketimiz yapacak.

            #endregion




            return Task.CompletedTask;
        }
    }
}
