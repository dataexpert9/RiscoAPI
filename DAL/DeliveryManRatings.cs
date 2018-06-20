using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DeliveryManRatings
    {
        public int Id { get; set; }
        public Int16 Rating { get; set; }
        public int User_ID { get; set; }
        public int Deliverer_Id { get; set; }
        public string Description { get; set; }
        public User User { get; set; }
        public DeliveryMan DeliveryMan { get; set; }
    }
}
