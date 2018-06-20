using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasketApi.Controllers
{
    //just to push
    public class AddUserAddressBindingModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int SignInType { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string StreetName { get; set; }

        [Required]
        public string BuildingName { get; set; }

        public string Floor { get; set; }

        public string Apartment { get; set; }

        [Required]
        public string NearestLandmark { get; set; }
        
        [Required]
        public short AddressType  { get; set; }
    }

    public class PaymentCardBindingModel
    {
        public int Id { get; set; }

        [Required]
        public bool IsEdit { get; set; }

        [Required]
        public string CardNumber { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public string CCV { get; set; }

        [Required]
        public string NameOnCard { get; set; }

        [Required]
        public int CardType { get; set; } //1 for Credit, 2 for Debit

        [Required]
        public int UserId { get; set; }
    }
}