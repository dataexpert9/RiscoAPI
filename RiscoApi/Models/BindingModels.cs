using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasketApi.Models
{
    public class LocationBindingModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }
    }

    public class EditUserAddressBindingModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int SignInType { get; set; }

        [Required]
        public int AddressId { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string StreetName { get; set; }

        public string Floor { get; set; }

        public string Apartment { get; set; }

        [Required]
        public string BuildingName { get; set; }

        [Required]
        public string NearestLandmark { get; set; }

        public bool IsPrimary { get; set; }

        [Required]
        public short AddressType { get; set; }

    }
}