using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ContactUs
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public int? Store_Id { get; set; }
        public Store Store { get; set; }
    }
}
