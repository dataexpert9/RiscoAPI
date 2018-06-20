using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.ViewModels
{
    public class NotificationsViewModel
    {
        public IEnumerable<Notification> Notifications { get; set; }
    }
}