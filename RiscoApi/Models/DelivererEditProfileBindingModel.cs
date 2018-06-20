using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasketApi.Models
{
    public class DelivererEditProfileBindingModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address1 { get; set; }

        [Required]
        public string Address2 { get; set; }
    }
}