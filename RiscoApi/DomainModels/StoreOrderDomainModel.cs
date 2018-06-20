using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.DomainModels
{
    public class StoreOrderDomainModel
    {
        public StoreOrderDomainModel(StoreOrder model)
        {
            Id = model.Id;
            OrderNo = model.OrderNo;
            Status = model.Status;
            Store_Id = model.Store_Id;
            Subtotal = model.Subtotal;
            Total = model.Total;
            IsDeleted = model.IsDeleted;
            Order_Id = model.Order_Id;
        }
        public int Id { get; set; }

        public string OrderNo { get; set; }

        public int Status { get; set; }
        
        public int Store_Id { get; set; }

        public double Subtotal { get; set; }

        public double ServiceFee { get; set; }

        public double DeliveryFee { get; set; }

        public double Total { get; set; }

        public bool IsDeleted { get; set; }

        public int Order_Id { get; set; }
    }
}