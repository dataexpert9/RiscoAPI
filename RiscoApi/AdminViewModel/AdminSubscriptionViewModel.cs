using BasketApi.ViewModels;
using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BasketApi.AdminViewModel
{

    public class SubscriptionListViewModel
    {
        public SubscriptionListViewModel()
        {
            Subscriptions = new List<AdminSubscriptionViewModel>();
        }
        public List<AdminSubscriptionViewModel> Subscriptions { get; set; }
        public Boolean is_detail { get; set; } = false;

    }
    public class AdminSubscriptionViewModel
    {
        public int Id { get; set; }

       public int UserId { get; set; }

        public DateTime SubscriptionDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public int Box_Id { get; set; }

        public int Type { get; set; }

        public string ActivationCode { get; set; }

        public int Status { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string BoxCategoryName { get; set; } = "";

        public int BoxCategory_Id { get; set; }

        public double Price { get; set; }

        public string FullName { get; set; }

        public string ProfilePictureUrl { get; set; }




    }
    #region Not Used Yet 



    //public class AdminBoxViewModel
    //{
    //    public int Id { get; set; }

    //    public string Name { get; set; }

    //    public string IntroUrl { get; set; }

    //    public bool IsDeleted { get; set; }

    //    public DateTime CreatedDate { get; set; }

    //    public DateTime ReleaseDate { get; set; }

    //    public int BoxCategory_Id { get; set; }

    //    public double Price { get; set; }

    //    public bool AlreadySubscribed { get; set; }
    //} 
    #endregion



}