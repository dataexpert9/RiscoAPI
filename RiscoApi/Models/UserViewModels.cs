using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasketApi.Models
{
    public class PhoneVerificationViewModel
    {
        [Required]
        [Display(Name = "Request Id")]
        public string request_id { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
    }

    public class MessageViewModel
    {
        public string StatusCode { get; set; }
        public string Details { get; set; }
    }

    public class ImagePathViewModel
    {
        [Required]
        public string Path { get; set; }
    }
}