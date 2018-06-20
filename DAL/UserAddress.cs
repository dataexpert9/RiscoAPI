using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class UserAddress
    {
        public int Id { get; set; }

        public int User_ID { get; set; }

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

        public bool IsDeleted { get; set; }

        public bool IsPrimary { get; set; }
        
        [JsonIgnore]
        public User User { get; set; }
    }
}
