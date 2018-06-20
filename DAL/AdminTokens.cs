using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AdminTokens
    {
        public int Id { get; set; }

        public string Token { get; set; }
        
        public int Admin_Id { get; set; }

        public Admin Admin { get; set; }
        public bool IsActive { get; set; }
    }
}
