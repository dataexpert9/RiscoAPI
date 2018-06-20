using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.ViewModels
{
    public class CartViewModel
    {
        public CartViewModel()
        {
            CartItems = new List<CartItemViewModel>();
        }
        public List<CartItemViewModel> CartItems { get; set; }
    }
}