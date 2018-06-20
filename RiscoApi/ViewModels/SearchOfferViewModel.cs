using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.ViewModels
{
    public class SearchOfferViewModel
    {
        public int Id { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidUpto { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Status { get; set; }

        public string ImageUrl { get; set; }

        public int Store_Id { get; set; }

        public string StoreName { get; set; }
    }
}