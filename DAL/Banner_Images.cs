using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Banner_Images
    {
        public int Id { get; set; }

        public string Url { get; set; }
        
        public int? Product_Id { get; set; }

        [ForeignKey("Product_Id")]
        public virtual Product Product { get; set; }
    }
}
