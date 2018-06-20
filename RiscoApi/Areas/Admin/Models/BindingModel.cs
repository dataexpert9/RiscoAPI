using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace BasketApi.Areas.SubAdmin.Models
{
    public class CategoryBindingModel
    {
        [Required]
        public string Name { get; set; }

        //[Required]
        public string Description { get; set; }

        [Required]
        public short StoreId { get; set; }

        public short Status { get; set; }
        
        public int ParentCategoryId { get; set; }
    }

    public class SubCategoryBindingModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public short CatId { get; set; }

        [Required]
        public short StoreId { get; set; }

        public short Status { get; set; }
    }

    public class StoreBindingModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public string Description { get; set; }

        [Required]
        public string Address { get; set; }

        public string ImageUrl { get; set; }

        public TimeSpan Open_From { get; set; }

        public TimeSpan Open_To { get; set; }
        //public DbGeography Location { get; set; }

        public StoreDeliveryHours StoreDeliveryHours { get; set; }
        
        public bool ImageDeletedOnEdit { get; set; }
    }

    public class StoreImageBindingModel
    {
        [Required]
        public int storeId { get; set; }
        
    }

    public class AddAdminBindingModel
    {
        [Required]
        public string StoreName { get; set; }

        public decimal Long { get; set; }

        public decimal Lat { get; set; }

        public short Description { get; set; }
    }

    public class SearchProductModel
    {
        public string ProductName { get; set; }
        public string ProductPrice { get; set; }
        public string CategoryName { get; set; }
    }
}