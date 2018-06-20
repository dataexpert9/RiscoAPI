using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Offer_Packages
    {
        public int Id { get; set; }

        public int Offer_Id { get; set; }
        
        public int Package_Id { get; set; }

        //public DateTime ValidUpto { get; set; }

        public string Description { get; set; }

        [JsonProperty("Price")]
        public double DiscountedPrice { get; set; }

        public int DiscountPercentage { get; set; }

        public double SlashPrice { get; set; }

        public string ImageUrl { get; set; }
        
        public virtual Offer Offer { get; set; }

        public virtual Package Package { get; set; }

        public bool IsDeleted { get; set; }

        [NotMapped]
        public int OfferPackageId { get; set; }

    }
}
