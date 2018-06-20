using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.DomainModels
{
    public class CartItemDomainModel
    {
        /// <summary>
        /// ProductId
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ProductName
        /// </summary>
        public string Name { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string Weight { get; set; }
        public string ImageUrl { get; set; }
        public string VideoUrl { get; set; }
        public Int16 Status { get; set; }
        public int Category_Id { get; set; }
        public string Size { get; set; }
        public int StoreId { get; set; }
    }
}