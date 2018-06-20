using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Settings
    {
        public Settings()
        {
            //BannerImages = new List<Banner_Images>();
            Interests = new List<Interest>();
        }

        [JsonIgnore]
        public int Id { get; set; }
        public double DeliveryFee { get; set; }
        public string Currency { get; set; }
        //public string HowItWorksUrl { get; set; }
        //public string HowItWorksUrlThumbnail { get; set; }
        //public string HowItWorksDescription { get; set; }
        public string AboutUs { get; set; }
        //public string BannerImage { get; set; } 
        public double FreeDeliveryThreshold { get; set; }
        //public string InstagramImage { get; set; }
        //public double Tax { get; set; }
        public string PrivacyPolicy { get; set; }
        //public string RefundExchange { get; set; }
        public string TermsConditions { get; set; }

        public string FAQs { get; set; }
        //public string ContactNo { get; set; }
        //public string  GoogleUrl{ get; set; }
        //public string  FacebookUrl{ get; set; }
        //public string  TwitterUrl{ get; set; }
        //public string  InstagramUrl{ get; set; }
        //public string  PintrestUrl{ get; set; }        
        public string AdminEmailForOrders { get; set; }
        //public List<Banner_Images> BannerImages { get; set; }

        //public string Payfort_AccessCode { get; set; }
        //public string Payfort_MerchantId { get; set; }
        //public string Payfort_RequestPhrase { get; set; }
        //public string Payfort_ResponsePhrase { get; set; }
        //public string SHA_TYPE { get; set; }

        //public bool IsLive { get; set; }

        //public string Payfort_TestUrl { get; set; }
        //public string Payfort_liveUrl { get; set; }

        public List<Interest> Interests { get; set; }
    }
}
