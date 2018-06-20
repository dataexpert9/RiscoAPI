using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DelivererAddress
    {
        public int Id { get; set; }

        public int DeliveryMan_Id { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string StreetName { get; set; }

        public string Floor { get; set; }

        public string Apartment { get; set; }

        [Required]
        public string NearestLandmark { get; set; }

        [Required]
        public string BuildingName { get; set; }

        [Required]
        public short Type { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public DeliveryMan DeliveryMan { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public DbGeography Location { get; set; }
    }
}
