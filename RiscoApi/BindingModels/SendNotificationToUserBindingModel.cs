using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.BindingModels
{
    public class SendNotificationToUserBindingModel
    {
        public int User_Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
    public class FCMTokenModel
    {
        public string Token { get; set; }
    }

}