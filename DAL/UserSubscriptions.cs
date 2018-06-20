using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class UserSubscriptions
    {
        public int Id { get; set; }

        public int User_Id { get; set; }

        public int Box_Id { get; set; }

        public int Type { get; set; }

        public string ActivationCode { get; set; }

        public int Status { get; set; }

        public DateTime SubscriptionDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public bool IsDeleted { get; set; }
        
        public Box Box { get; set; }

        public User User { get; set; }
    }
}
