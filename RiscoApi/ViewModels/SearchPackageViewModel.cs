using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.ViewModels
{
    public class SearchPackageViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public short Status { get; set; }

        public int Store_Id { get; set; }

        public string ImageUrl { get; set; }

        public bool IsDeleted { get; set; }

        public string StoreName { get; set; }

        public double Price { get; set; }

        public bool IsChecked { get; set; }

        public int OfferPackageId { get; set; }

        public int Package_Id { get; set; }

    }
}