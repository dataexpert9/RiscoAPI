using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace DAL
{
    public class Media
    {
        public int Id { get; set; }

        public int Type { get; set; }

        public string Url { get; set; }

        public int Post_Id { get; set; }

        public virtual Post Post { get; set; }

        //public int User_Id { get; set; }

        //public virtual User User { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
