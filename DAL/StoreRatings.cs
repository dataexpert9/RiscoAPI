using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class StoreRatings
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int User_Id { get; set; }
        [Required]
        public int Store_Id { get; set; }
        [Required]
        public int Rating { get; set; }

        public User User { get; set; }
        public Store Store { get; set; }
    }
}
