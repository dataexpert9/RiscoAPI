using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.BindingModels
{
    public class SettingsProductBindingModel
    {
        public int Product_Id { get; set; }

        public int BannerImage_Id { get; set; }
    }

    public class PrivacyViewModel
    {
        public string PrivacyPolicy { get; set; }
    }

    public class AboutUsViewModel
    {
        public string AboutUs { get; set; }
    }

    public class TermsConditionsViewModel
    {
        public string TermsConditions { get; set; }
    }

    public class FAQsViewModel
    {
        public string FAQs { get; set; }
    }

    public class RefundExchangeViewModel
    {
        public string RefundExchange { get; set; }
    }

    public class SetUrlsViewModel
    {
        public string GoogleUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string PintrestUrl { get; set; }
    }
}