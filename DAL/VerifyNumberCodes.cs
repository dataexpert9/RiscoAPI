using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class VerifyNumberCodes
    {
        public int Id { get; set; }

        public int Code { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public int User_Id { get; set; }

        public virtual User User { get; set; }
        public string Phone { get; set; }
    }
}
