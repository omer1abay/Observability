using Common.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Payment.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        [HttpPost]
        public IActionResult Create(PaymentCreateRequestDto requestDto)
        {

            if (HttpContext.Request.Headers.TryGetValue("traceparent", out StringValues value))
            {
                Console.WriteLine($"traceParent:{value.First()}");
            }

            const decimal balance = 1000;

            if (requestDto.TotalPrice > balance)
            {
                return BadRequest(CustomResponseDto<PaymentCreateResponseDto>.Fail(400,"yetersiz bakiye"));
            }

            return Ok(CustomResponseDto<PaymentCreateResponseDto>.Success(200,new PaymentCreateResponseDto() { Description = "ödeme başarılı" }));

        }

    }
}
