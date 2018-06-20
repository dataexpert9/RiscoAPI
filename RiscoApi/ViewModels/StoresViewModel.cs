using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.ViewModels
{
    public class StoresViewModel
    {
        public int Count { get; set; }
        public IEnumerable<Store> Stores { get; set; }
    }
}