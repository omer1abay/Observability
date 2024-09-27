using System.Diagnostics.Metrics;

namespace Metric.API.OpenTelemetry;

public static class OpenTelemetryMetric
{
    public static readonly Meter meter = new Meter("metric.meter.api");

    //counter
    public static Counter<int> OrderCreatedEventCounter = meter.CreateCounter<int>("order.created.event.count"); //bu isim prometheus'ta görünecek isimdir

    public static ObservableCounter<int> OrderCancelledCounter = meter.CreateObservableCounter("order.cancelled.count",
            () => new Measurement<int>(Counter.OrderCancelledCounter)
        );


    public static UpDownCounter<int> CurrentStockCounter = meter.CreateUpDownCounter<int>("current.stock.count");

    public static ObservableUpDownCounter<int> CurrentStockObservableCounter = meter.CreateObservableUpDownCounter("current.stock.observable.count",
                () => new Measurement<int>(Counter.CurrentStockCounter)
            );


    public static ObservableGauge<int> RowKitchenTemp = meter.CreateObservableGauge("room.kitchen.temp", () => new Measurement<int>(Counter.KitchenTemp));

    public static Histogram<int> MethodDuration = meter.CreateHistogram<int>("method.duration", unit: "milliseconds");

}
