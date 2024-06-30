﻿using Common.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;

namespace Stock.API.Services
{
    public class StockService
    {
        private readonly PaymentService _paymentService;

        public StockService(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        private Dictionary<int, int> GetProductStockList()
        {
            Dictionary<int, int> productStockList = new();
            productStockList.Add(1, 10);
            productStockList.Add(2, 20);
            productStockList.Add(3, 30);


            return productStockList;
        }


        public async Task<CustomResponseDto<StockCheckAndPaymentProcesResponse>> CheckAndPaymentProcess(StockCheckAndPaymentProcesRequest request)
        {
            //traceState üzerinden OrderServisinden gönderdiğimiz data'yı alma işlemi
            var userId = Activity.Current?.GetBaggageItem("userId");


            var productStockList = GetProductStockList();

            var stockStatus = new List<(int productId, bool hasStockExist)>(); //gizli tuple

            foreach (var requestItem in request.OrderItems)
            {
                var hasExistStock = productStockList.Any(x => x.Key == requestItem.ProductId
                                    && x.Value >= requestItem.Count);


                stockStatus.Add((requestItem.ProductId, hasExistStock));
            }

            if (stockStatus.Any(x => x.hasStockExist == false))
            {
                //stokta yok

                return CustomResponseDto<StockCheckAndPaymentProcesResponse>.Fail(HttpStatusCode.BadRequest.GetHashCode(), "stok yok");
            }

            //stokta var, payment süre. başlar
            var (isSuccess, failMessage) = await _paymentService.CreatePaymentProcess(new PaymentCreateRequestDto() { OrderCode = request.OrderCode, TotalPrice = request.OrderItems.Sum(x => x.Price) });

            if (!isSuccess)
            {
                return CustomResponseDto<StockCheckAndPaymentProcesResponse>.Fail(HttpStatusCode.BadRequest.GetHashCode(), failMessage!);
            }

            return CustomResponseDto<StockCheckAndPaymentProcesResponse>.Success(HttpStatusCode.OK.GetHashCode(), new StockCheckAndPaymentProcesResponse { Description = "ödeme süreci tamamlandı." });


        }


    }
}