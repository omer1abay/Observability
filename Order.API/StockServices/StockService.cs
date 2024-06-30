using Common.Shared.DTOs;

namespace Order.API.StockServices
{
    public class StockService
    {
        private readonly HttpClient _httpClient;

        public StockService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool isSuccess, string? failMessage)> CheckStockAndStartPayment(StockCheckAndPaymentProcesRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync<StockCheckAndPaymentProcesRequest>("api/Stock/CheckStockAndPaymentStart",request);
            
            var responseContent = await response.Content.ReadFromJsonAsync<CustomResponseDto<StockCheckAndPaymentProcesResponse>>();

            return response.IsSuccessStatusCode ? (true,null) : (false,responseContent?.Errors?.First());

        }


    }
}
