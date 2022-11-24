// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Oms
{
    public class OrderStatusFilter
    {
        /// <summary>
        /// Order status string
        /// </summary>
        public List<OrderStatus> OrderStatuses { get; set; }

        public OrderStatusFilter()
        {
            OrderStatuses = new List<OrderStatus>();
        }
    }
}
