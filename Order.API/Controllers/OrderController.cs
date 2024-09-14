using Common.Shared.Events;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.OrderServices;

namespace Order.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly IPublishEndpoint _publishEndpoint; //Ipublish endpoint direkt exhange'e gönderir kuyruğa göndermez
        

        public OrderController(OrderService orderService, IPublishEndpoint publishEndpoint)
        {
            _orderService = orderService;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateRequestDto dto)
        {


            var request = await _orderService.CreateAsync(dto);

            #region Third party api istek
            var httpCliemt = new HttpClient(); //normal şartlarda new'lenmez httpclient nesnesi DI container'dan verilir yoksa socket yetersiz hatası alırız.

            var result = await httpCliemt.GetAsync("https://jsonplaceholder.typicode.com/todos/1");

            var response = await result.Content.ReadAsStringAsync(); 
            #endregion


            #region Exception Example
            //var a = 10;
            //var b = 0;
            //var c = a / b; 
            #endregion

            return new ObjectResult(request) {  StatusCode = request.StatusCode };
        }


        [HttpGet]
        public async Task<IActionResult> SendOrderCreatedEvent()
        {
            //kuyruğa mesaj gönder -> masstransit her iki consumer ve producer servislerinde aynı event'e bakmasını ister. Bu yüzden Shared yapmamız lazım.
            await _publishEndpoint.Publish(new OrderCreateEvent() { OrderCode = Guid.NewGuid().ToString() });

            return Ok();
        }

    }
}
