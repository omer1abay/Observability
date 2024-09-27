using Metric.API.OpenTelemetry;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Metric.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MetricsController : ControllerBase
    {

        [HttpGet]
        public IActionResult CounterMetric()
        {
            OpenTelemetry.OpenTelemetryMetric.OrderCreatedEventCounter.Add(1, 
                new KeyValuePair<string, object?>("event","add"),
                new KeyValuePair<string, object?>("queueName", "order-created-queue")); //metric toplama işlemi keyvaluepair parametresi opsiyoneldir. 1 sayısı increment miktarını belirtir.

            return Ok();
        }


        [HttpGet]
        public IActionResult CounterObservableMetric()
        {
            Counter.OrderCancelledCounter += new Random().Next(1,100);

            return Ok();
        }

        [HttpGet]
        public IActionResult UpDownCounterMetric()
        {
            OpenTelemetry.OpenTelemetryMetric.CurrentStockCounter.Add(new Random().Next(-100,100));

            return Ok();
        }


        [HttpGet]
        public IActionResult UpDownCounterObservableMetric()
        {
            Counter.CurrentStockCounter += new Random().Next(-100, 100);

            return Ok();
        }

        [HttpGet]
        public IActionResult GaugeObservableMetric()
        {
            Counter.KitchenTemp = new Random().Next(-30, 60);

            return Ok();
        }

        [HttpGet]
        public IActionResult Histogram()
        {
            OpenTelemetry.OpenTelemetryMetric.MethodDuration.Record(new Random().Next(500, 50000));

            return Ok();
        }

    }
}
