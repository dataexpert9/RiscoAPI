using BasketApi.ViewModels;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.Areas.Admin.ViewModels
{
    public class ProductsViewModel
    {
        public int Count { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }

    public class SearchProductListViewModel
    {
        public IEnumerable<SearchProductViewModel> Products { get; set; }
    }

    public class SearchCategoryListViewModel
    {
        public IEnumerable<SearchCategoryViewModel> Categories { get; set; }
    }

    public class SearchPackageListViewModel
    {
        public IEnumerable<SearchPackageViewModel> Packages { get; set; }
    }

    
    public class SearchOfferListViewModel
    {
        public IEnumerable<SearchOfferViewModel> Offers { get; set; }
    }
}