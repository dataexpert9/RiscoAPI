using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasketApi.ViewModels
{
    public class SearchOrdersViewModel
    {
        public SearchOrdersViewModel()
        {
            DeliveryMen = new List<DelivererOptionsViewModel>();
        }
        public int Id { get; set; }

        public string CustomerName { get; set; }

        public int StoreId { get; set; }

        [Required]
        [Range(1, Int32.MaxValue)]
        public int? DeliveryManId { get; set; }

        public string StoreName { get; set; }

        public int OrderStatus { get; set; }

        public string OrderStatusName { get; set; }
        
        public DateTime CreatedOn { get; set; }

        public string PaymentStatus { get; set; }

        public double OrderTotal { get; set; }

        public int StoreOrder_Id { get; set; }

        public DbGeography StoreLocation { get; set; }
        public string OrderItemsComaSep { get; set; }

        public List<DelivererOptionsViewModel> DeliveryMen { get; set; }

        public class Comparer : IEqualityComparer<SearchOrdersViewModel>
        {
            public bool Equals(SearchOrdersViewModel x, SearchOrdersViewModel y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(SearchOrdersViewModel obj)
            {
                unchecked  // overflow is fine
                {
                    int hash = 17;
                    hash = hash * 23 + (obj.Id).GetHashCode();
                    return hash;
                }
            }
        }
    }

    public class DelivererOptionsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public class Comparer : IEqualityComparer<DelivererOptionsViewModel>
        {
            public bool Equals(DelivererOptionsViewModel x, DelivererOptionsViewModel y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(DelivererOptionsViewModel obj)
            {
                unchecked  // overflow is fine
                {
                    int hash = 17;
                    hash = hash * 23 + (obj.Id).GetHashCode();
                    return hash;
                }
            }
        }
    }

    public class SearchOrdersListViewModel
    {
        public SearchOrdersListViewModel()
        {
            Orders = new List<SearchOrdersViewModel>();
        }

        public List<SearchOrdersViewModel> Orders { get; set; }
    }
}