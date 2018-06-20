using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels
{
    public class OrdersSalesGraph
    {
        public double Total { get; set; }
        public DateTime OrderDateTime { get; set; }
    }

    public class ListOrderSalesGraph
    {
        public ListOrderSalesGraph()
        {
            Orders = new List<OrdersSalesGraph>();
        }
        public List<OrdersSalesGraph> Orders { get; set; }
    }
}