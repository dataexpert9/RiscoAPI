using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasketApi.Models
{
    public class RateUserBindingModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int Deliverer_Id { get; set; }

        [Required]
        public short SignInType { get; set; }

        [Required]
        public short Rating { get; set; }

        [Required]
        public string Feedback { get; set; }
    }
}