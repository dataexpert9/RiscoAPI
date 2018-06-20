namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Offer_Products
    {
        public int Id { get; set; }

        public int Offer_Id { get; set; }
        
        public int Product_Id { get; set; }

        [NotMapped]
        public int OfferProductId { get; set; }

        //public DateTime ValidUpto { get; set; }

        public string Description { get; set; }

        [JsonProperty("Price")]
        public double DiscountedPrice { get; set; }

        public int DiscountPercentage { get; set; }

        public double SlashPrice { get; set; }

        public string ImageUrl { get; set; }

        public bool IsDeleted { get; set; }
                                           //public int Qty { get; set; }

        //public int Package_Id { get; set; }

        public virtual Offer Offer { get; set; }

        //public virtual Package Package { get; set; }

        public virtual Product Product { get; set; }
    }
}
