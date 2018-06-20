using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.ViewModels
{
    public class SearchProductViewModel
    {
        public SearchProductViewModel()
        {
            ProductImages = new List<Product_Images>();
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }

        public string Weight { get; set; }

        public double WeightInGrams { get; set; }

        public double WeightInKiloGrams { get; set; }

        public string ImageUrl { get; set; }

        public string VideoUrl { get; set; }

        public short Status { get; set; }

        public int Category_Id { get; set; }

        public int Store_Id { get; set; }

        public string Size { get; set; }

        public string StoreName { get; set; }

        public string CategoryName { get; set; }

        public int Qty { get; set; } = 1;

        public bool IsChecked { get; set; }

        public int PackageProductId { get; set; }

        public int Package_Id { get; set; }

        public int Product_Id { get; set; }

        public int OfferProductId { get; set; }
        
        public double AverageRating { get; set; }

        public int WeightUnit { get; set; }

        public List<Product_Images> ProductImages { get; set; }

        public bool IsFavourite { get; set; }
    }
}