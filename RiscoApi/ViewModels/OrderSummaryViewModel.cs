using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.ViewModels
{
    public class OrderSummaryViewModel
    {
        public OrderSummaryViewModel(Order order)
        {
            OrderId = order.Id;
            OrderDateTime = order.OrderDateTime;
            DeliveryDateTime_From = order.DeliveryTime_From;
            DeliveryDateTime_To = order.DeliveryTime_To;
            AdditionalNote = order.AdditionalNote;
            PaymentMethodType = order.PaymentMethod;
            DeliveryAddress = order.DeliveryAddress;
            SubTotal = order.Subtotal;
            ServiceFee = order.ServiceFee;
            DeliveryFee = order.DeliveryFee;
            Total = order.Total;
            Tax = order.Tax;
        }

        public int OrderId { get; set; }

        public DateTime OrderDateTime { get; set; }

        public DateTime DeliveryDateTime_From { get; set; }

        public DateTime DeliveryDateTime_To { get; set; }

        public string AdditionalNote { get; set; }

        public int PaymentMethodType { get; set; }

        public string DeliveryAddress { get; set; }

        public double SubTotal { get; set; }

        public double ServiceFee { get; set; }

        public double DeliveryFee { get; set; }

        public double Total { get; set; }

        public double Tax { get; set; }
    }
}

