using Common.Shared.DTOs;

namespace Stock.API.Services
{
    public class PaymentService
    {
        private readonly HttpClient _httpClient;

        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool isSuccess, string? failMessage)> CreatePaymentProcess(PaymentCreateRequestDto request)
        {
            var response = await _httpClient.PostAsJsonAsync<PaymentCreateRequestDto>("api/Payment/Create", request);

            var responseContent = await response.Content.ReadFromJsonAsync<CustomResponseDto<PaymentCreateResponseDto>>();

            return response.IsSuccessStatusCode ? (true, null) : (false, responseContent?.Errors?.First());

        }

    }
}
