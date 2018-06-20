using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.ViewModels
{

    public class MySkriblBoxViewModel
    {
        public MySkriblBoxViewModel()
        {
            Subscriptions = new List<UserSubscriptions>();
        }

        public List<UserSubscriptions> Subscriptions { get; set; }
    }
}