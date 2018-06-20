using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasketApi.Models
{
    public class PhoneVerificationBindingModel
    {
        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}