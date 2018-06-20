using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AppRatings
    {
        public int Id { get; set; }
        public int User_ID { get; set; }
        public short Rating { get; set; }
        public string Description { get; set; }
        public User User { get; set; }
    }
}
