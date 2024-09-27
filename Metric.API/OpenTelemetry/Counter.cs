namespace Metric.API.OpenTelemetry
{
    public class Counter
    {
        public static int OrderCancelledCounter { get; set; } //bu değeri biz istediğimiz yerde arttıracağız ve prometheus her geldiğinde bu datayı okuyacak

        public static int CurrentStockCounter { get; set; } = 1000;

        public static int KitchenTemp { get; set; } = 0;
    }
}
