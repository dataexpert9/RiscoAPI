using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.Models
{
    public class ContactUsBindingModel
    {
        public int? UserId { get; set; }

        public string Description { get; set; }

        //public int Month { get; set; }

        //public int Year { get; set; }
    }
}