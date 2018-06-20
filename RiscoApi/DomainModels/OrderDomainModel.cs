using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.DomainModels
{
    public class OrderDomainModel
    {

        public OrderDomainModel(Order model)
        {
            Id = model.Id;
            OrderNo = model.OrderNo;
            Status = model.Status;
            OrderDateTime = model.OrderDateTime;
            DeliveryTime_From = model.DeliveryTime_From;
            DeliveryTime_To = model.DeliveryTime_To;
            AdditionalNote = model.AdditionalNote;
            PaymentMethod = model.PaymentMethod;
            Subtotal = model.Subtotal;
            ServiceFee = model.ServiceFee;
            DeliveryFee = model.DeliveryFee;
            Total = model.Total;
            User_ID = model.User_ID;
            IsDeleted = model.IsDeleted;
            StoreOrders = model.StoreOrders;
        }

        public int Id { get; set; }

        public string OrderNo { get; set; }

        public int Status { get; set; }

        public DateTime OrderDateTime { get; set; }

        public DateTime DeliveryTime_From { get; set; }

        public DateTime DeliveryTime_To { get; set; }

        public string AdditionalNote { get; set; }

        public int PaymentMethod { get; set; }

        public double Subtotal { get; set; }

        public double ServiceFee { get; set; }

        public double DeliveryFee { get; set; }

        public double Total { get; set; }

        public int User_ID { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<StoreOrder> StoreOrders { get; set; }

        public IEnumerable<CartItemDomainModel> CartItems { get; set; }
    }
}