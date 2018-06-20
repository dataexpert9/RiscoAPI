using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.ViewModels
{
    public class CartItemViewModel
    {
        public int ItemId { get; set; }
        public int ItemType { get; set; }
        public int Qty { get; set; }
        public int StoreId { get; set; }
    }
}