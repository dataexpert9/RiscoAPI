using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.BindingModels
{
    public class Product_ImagesModel
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public string ThumbnailUrl { get; set; }

        public bool IsVideo { get; set; }

        public int? Product_Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
    }
}