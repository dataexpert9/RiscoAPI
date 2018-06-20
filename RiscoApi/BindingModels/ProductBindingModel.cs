using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasketApi.BindingModels
{
    public class ProductBindingModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        //[Required]
        public string Description { get; set; }

        public double Weight { get; set; }

        public string ImageUrl { get; set; }

        public string VideoUrl { get; set; }

        public short Status { get; set; }

        //public int SubCategory_Id { get; set; }

        public int Category_Id { get; set; }

        public int Store_Id { get; set; }

        public bool IsDeleted { get; set; }

        public string Size { get; set; }
        

    }
}