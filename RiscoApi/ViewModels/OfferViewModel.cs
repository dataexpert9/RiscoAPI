using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.ViewModels
{
    public class OfferViewModel
    {
        public IEnumerable<SearchPackageViewModel> Packages { get; set; }
        public IEnumerable<SearchProductViewModel> Products { get; set; }
    }

    public class OfferMobileViewModel
    {
        public IEnumerable<OfferPackagesMobileViewModel> Offer_Packages { get; set; }
        public IEnumerable<OfferProductsMobileViewModel> Offer_Products { get; set; }
    }

    public class OfferPackagesMobileViewModel
    {
        public int Offer_Id { get; set; }
        public string Offer_Title { get; set; }
        public string Offer_ImageUrl { get; set; }
        public DateTime ValidUpto { get; set; }
        public int Store_Id { get; set; }
        public int OfferPackage_Id { get; set; }
        public string OfferPackage_Name { get; set; }
        public double DiscountedPrice { get; set; }
        public double SlashPrice { get; set; }
        public string OfferPackage_ImageUrl { get; set; }
        public string PackageName { get; set; }
        public double PackagePrice { get; set; }
        public string ProductName { get; set; }
        public int ProductQty { get; set; }
        public string OfferPackage_Description { get; set; }
    }

    public class OfferProductsMobileViewModel
    {
        public int Offer_Id { get; set; }
        public string Offer_Title { get; set; }
        public string Offer_ImageUrl { get; set; }
        public DateTime ValidUpto { get; set; }
        public int Store_Id { get; set; }
        public int OfferProduct_Id { get; set; }
        public string OfferProduct_Name { get; set; }
        public string OfferProduct_ImageUrl { get; set; }
        public double DiscountedPrice { get; set; }
        public double SlashPrice { get; set; }
        public int ProductId { get; set; }
        public string Product_Name { get; set; }
        public string Product_ImageUrl { get; set; }
        public string OfferProduct_Description { get; set; }
    }
}