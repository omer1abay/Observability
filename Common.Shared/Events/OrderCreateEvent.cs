﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Shared.Events
{
    public record OrderCreateEvent
    {
        public string OrderCode { get; set; } = null!;
    }
}
