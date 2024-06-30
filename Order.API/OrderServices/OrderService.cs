using Common.Shared.DTOs;
using Common.Shared.Events;
using MassTransit;
using OpenTelemetry.Shared;
using Order.API.Models;
using Order.API.RedisServices;
using Order.API.StockServices;
using System.Diagnostics;
using System.Net;

namespace Order.API.OrderServices
{
    public class OrderService
    {
        private readonly AppDbContext _context;
        private readonly StockService _stockService;
        private readonly RedisService _redisService;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderService(AppDbContext context, StockService stockService, RedisService redisService, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _stockService = stockService;
            _redisService = redisService;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<CustomResponseDto<OrderCreateResponseDto>> CreateAsync(OrderCreateRequestDto requestDto)
        {

            //using (var redisActivity = ActivitySourceProvider.Source.StartActivity("Redis Request"))
            //{ Redis ile ilgili bir instrumentation olduğu için using almaya gerek yok ama instrumentation paketi olmayan db'leri using bloğuna alıp activity başlatmamız gerekir.
            //Redis'e giden datayı da kayıt etmek istersek şayet yine using ifadesi ile bir acitivity başlatıp Tag ile kayıt etmemiz gerekir

            using (var redis = ActivitySourceProvider.Source.StartActivity("RedisSetGet")) {
                await _redisService.GetDb(0).StringSetAsync("user_id", requestDto.UserId);

                redis.SetTag("user_id",requestDto.UserId); //veriyi kayıt etmek için..

                var redisUserId = await _redisService.GetDb(0).StringGetAsync("user_id");
            }


            //Genel activity'e tag ekleme işlemi

            Activity.Current?.SetTag("aspnetcore instrumentation tag1", "instrumentation tag1 value");

            //-----

            //custom metot için yazdığımız activity'e tag event ekleme
            using var activity = ActivitySourceProvider.Source.StartActivity();

            activity?.AddEvent(new(name: "Sipariş süreci başladı"));

            activity?.SetBaggage("userId",requestDto.UserId.ToString()); //tracestate'e data set etme işlemi..

            var newOrder = new Order()
            {
                CreatedDate = DateTime.Now,
                OrderCode = Guid.NewGuid().ToString(),
                Status = OrderStatus.Success,
                UserId = requestDto.UserId,
                Items = requestDto.Items.Select(x => new OrderItem()
                {
                    Count = x.Count,
                    ProductId = x.ProductId,
                    Price = x.Price
                }).ToList()
            };


            await _context.AddAsync(newOrder);
            await _context.SaveChangesAsync();

            

            StockCheckAndPaymentProcesRequest stockCheck = new();
            stockCheck.OrderCode = newOrder.OrderCode;
            stockCheck.OrderItems = requestDto.Items;


            var (isSuccess, failMessage) = await _stockService.CheckStockAndStartPayment(stockCheck);

            if (!isSuccess)
            {
                return CustomResponseDto<OrderCreateResponseDto>.Fail(HttpStatusCode.BadRequest.GetHashCode(), failMessage!);
            }

            activity?.SetTag("order user id", requestDto.UserId);

            activity?.AddEvent(new(name: "Sipariş süreci tamamlandı"));
            //-----


            return CustomResponseDto<OrderCreateResponseDto>.Success(HttpStatusCode.OK.GetHashCode(), new OrderCreateResponseDto() { Id = newOrder.Id });
        }
    }
}
