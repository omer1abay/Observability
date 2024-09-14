using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace Order.API.OrderServices
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderCode { get; set; } = null!; //efcore bu entity'e göre bir tablo oluşturunca bu null değil ifadesinin bir değeri var eski sürümlerde efcore string değerleri auto nullable yapardı.
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItem> Items { get; set; } = null!;
    }

    public enum OrderStatus : byte
    {
        Success = 1,
        Failed = 0
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        [Precision(18,2)]
        public decimal Price { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }

}
