using Common.Shared.DTOs;

namespace Order.API.OrderServices
{
    //record olarak yaptık çünkü imuttable yani değiştirilemeyen nesne olarak tutmak istiyoruz dto'ları
    public record OrderCreateRequestDto
    {
        public int UserId { get; set; }
        public List<OrderItemDto> Items { get; set; } = null!;
    }
}
