using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.AppsViewModels
{
    public class OrdersHistoryViewModel
    {
        public int Count { get; set; }
        public List<OrderViewModel> orders { get; set; }
    }

    public class OrdersScheduleViewModel
    {
        public int Count { get; set; }
        public List<DAL.Order> Orders { get; set; }
    }
}