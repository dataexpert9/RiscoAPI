using BasketApi.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BasketApi.AppsViewModels
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {
            StoreOrders = new HashSet<StoreOrderViewModel>();
        }
        public int Id { get; set; }
        
        public string OrderNo { get; set; }

        public int Status { get; set; }

        [JsonConverter(typeof(JsonCustomDateTimeConverter))]
        public DateTime OrderDateTime { get; set; }

        [JsonConverter(typeof(JsonCustomDateTimeConverter))]
        public DateTime DeliveryTime_From { get; set; }

        [JsonConverter(typeof(JsonCustomDateTimeConverter))]
        public DateTime DeliveryTime_To { get; set; }

        public string AdditionalNote { get; set; }

        public int PaymentMethod { get; set; }
        public string PaymentMethodName { get; set; }

        public double Subtotal { get; set; }

        public double ServiceFee { get; set; }

        public double DeliveryFee { get; set; }

        public double Total { get; set; }

        public int User_ID { get; set; }

        public bool IsDeleted { get; set; }

        public int? OrderPayment_Id { get; set; }

        public string DeliveryAddress { get; set; }

        public int? DeliveryMan_Id { get; set; }

        public string UserFullName { get; set; }

        public string UserProfilePictureUrl { get; set; }

        public virtual ICollection<StoreOrderViewModel> StoreOrders { get; set; }

        public double Tax { get; set; }

        //public virtual OrderPaymentViewModel OrderPayment { get; set; }

            
        public virtual UserViewModel User { get; set; }
    }

    //public class OrderPaymentViewModel
    //{


    //}
}