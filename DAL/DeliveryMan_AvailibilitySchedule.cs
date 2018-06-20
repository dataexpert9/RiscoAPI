using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DeliveryMan_AvailibilitySchedule
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsDeleted { get; set; }

        public int DeliveryMan_Id { get; set; }

        public DeliveryMan DeliveryMan { get; set; }
    }
}
