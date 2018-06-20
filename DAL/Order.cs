using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Order
    {
        public Order()
        {
            StoreOrders = new HashSet<StoreOrder>();
        }
        public int Id { get; set; }

        [Required]
        public string OrderNo { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        [JsonConverter(typeof(JsonCustomDateTimeConverter))]
        public DateTime OrderDateTime { get; set; }

        [JsonConverter(typeof(JsonCustomDateTimeConverter))]
        public DateTime DeliveryTime_From { get; set; }

        [JsonConverter(typeof(JsonCustomDateTimeConverter))]
        public DateTime DeliveryTime_To { get; set; }
        
        public string AdditionalNote { get; set; }

        public int PaymentMethod { get; set; }

        public double Subtotal { get; set; }

        public double ServiceFee { get; set; }

        public double DeliveryFee { get; set; }

        public double Total { get; set; }

        public int User_ID { get; set; }

        public bool IsDeleted { get; set; }

        public int? OrderPayment_Id { get; set; }

        public short PaymentStatus { get; set; }

        public string DeliveryAddress { get; set; }

        public virtual OrderPayment OrderPayment { get; set; }

        public virtual User User { get; set; }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreOrder> StoreOrders { get; set; }

        public virtual DeliveryMan DeliveryMan { get; set; }

        public int? DeliveryMan_Id { get; set; }
        public bool RemoveFromDelivererHistory { get; set; }
        public bool RemoveFromUserHistory { get; set; }
        public double Tax { get; set; }
        /// <summary>
        /// This is Fort_id
        /// </summary>
        public string PaymentTransactionId { get; set; }
    }
}
