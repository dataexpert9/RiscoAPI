using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.Models
{
    public class SearchOrderModel
    {
        public string Id { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? StoreId { get; set; }
        public int? OrderStatusId { get; set; }
        public int? PaymentStatusId { get; set; }
        public int? PaymentMethodId { get; set; }
    }
}