using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;

namespace BasketApi.ViewModels
{
    public class OrdersHistoryViewModel
    {
        public int Count { get; set; }
        public List<Order> orders { get; set; }
    }
}