using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.ViewModels
{
    public class SearchAdminNotificationsViewModel
    {
        public SearchAdminNotificationsViewModel()
        {
            Notifications = new List<AdminNotifications>();
        }
        public List<AdminNotifications> Notifications { get; set; }
    }
}