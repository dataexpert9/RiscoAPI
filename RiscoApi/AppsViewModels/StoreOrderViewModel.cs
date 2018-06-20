using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.AppsViewModels
{
    public class StoreOrderViewModel
    {
        public StoreOrderViewModel()
        {
            OrderItems = new List<OrderItemViewModel>();
        }
        public int Id { get; set; }
        
        public string OrderNo { get; set; }

        public int Status { get; set; }

        public int Store_Id { get; set; }

        public double Subtotal { get; set; }

        public double Total { get; set; }

        public bool IsDeleted { get; set; }

        public int Order_Id { get; set; }
        
        public string StoreName { get; set; }

        public string ImageUrl { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; }
    }
}