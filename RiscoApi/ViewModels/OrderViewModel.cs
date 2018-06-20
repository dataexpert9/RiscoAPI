using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication1.ViewModels;

namespace BasketApi.ViewModels
{
    public class OrderViewModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime DeliveryDateTime_From  { get; set; }

        [Required]
        public DateTime DeliveryDateTime_To { get; set; }

        public string AdditionalNote { get; set; }

        public UInt16 PaymentMethodType { get; set; }

        public string DeliveryAddress { get; set; }

        public OrderViewModel()
        {
            Cart = new CartViewModel();

            PaymentCard = new PaymentCardViewModel();

            PaymentInfo = new CaptureBindingModel();
        }
        public CartViewModel Cart { get; set; }

        public PaymentCardViewModel PaymentCard { get; set; }

        public CaptureBindingModel PaymentInfo { get; set; }

    }
}