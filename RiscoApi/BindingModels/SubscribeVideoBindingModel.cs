using BasketApi.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication1.ViewModels;

namespace BasketApi.BindingModels
{
    public class SubscribeVideoBindingModel
    {
        public SubscribeVideoBindingModel()
        {
            PaymentInfo = new CaptureBindingModel();
        }

        public int Id { get; set; }

        [Required]
        public int User_Id { get; set; }

        [Required]
        public int Box_Id { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        public DateTime Month { get; set; }

        [Required]
        public UInt16 PaymentMethodType { get; set; }

        public PaymentCardViewModel PaymentCard { get; set; }

        public DateTime SubscriptionDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public string DeliveryAddress { get; set; }

        public string AdditionalNote { get; set; }

        public CaptureBindingModel PaymentInfo { get; set; }
    }
}