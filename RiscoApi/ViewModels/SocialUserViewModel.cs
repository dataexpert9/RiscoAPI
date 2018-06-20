using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.ViewModels
{
    public class SocialUserViewModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string email { get; set; }
        public string picture { get; set; }
    }

    public class GoogleUserModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string email { get; set; }
        public string picture { get; set; }
    }

}