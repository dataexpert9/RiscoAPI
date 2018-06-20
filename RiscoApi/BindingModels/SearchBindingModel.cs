using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.BindingModels
{
    public class SearchBindingModel
    {

        public double DeliveryFee { get; set; }
        public string Currency { get; set; }
        public string HowItWorksUrl { get; set; }
        public string HowItWorksDescription { get; set; }
        public string AboutUs { get; set; }
        public string BannerImage { get; set; }
        public double FreeDeliveryThreshold { get; set; }

    }
}