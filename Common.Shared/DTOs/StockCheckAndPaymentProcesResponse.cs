﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Shared.DTOs
{
    public record StockCheckAndPaymentProcesResponse
    {
        public string Description { get; set; } = null!;
    }
}
