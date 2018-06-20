using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BasketApi.ViewModels;

namespace BasketApi.ViewModels
{
    public class PreviousOrdersViewModel
    {
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}