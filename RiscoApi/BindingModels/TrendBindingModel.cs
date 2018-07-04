using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.BindingModels
{
    public class TrendBindingModel
    {
        public string Text { get; set; }

        public int Count { get; set; }
    }

    public class TrendListViewModel
    {
        public TrendListViewModel()
        {
            Trends = new List<TrendBindingModel>();
        }
        public List<TrendBindingModel> Trends { get; set; }
    }
}