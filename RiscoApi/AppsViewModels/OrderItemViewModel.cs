using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.AppsViewModels
{
    public class OrderItemViewModel
    {
        public int Id { get; set; }

        public int ItemId { get; set; }

        public int ItemType { get; set; }

        public int Qty { get; set; }

        public int StoreOrder_Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }

        public string Weight { get; set; }

        public double WeightInGrams { get; set; }

        public double WeightInKiloGrams { get; set; }

        public string ImageUrl { get; set; }
        public bool IsFavourite { get; internal set; }

        public Product Product { get; set; }
    }
}