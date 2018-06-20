using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.ViewModels
{
    public class SearchBoxesViewModel
    {
        public SearchBoxesViewModel()
        {
            Boxes = new List<Box>();
        }
        public List<Box> Boxes { get; set; }
    }
}