namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Franchisor
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string CommissionFormula { get; set; }

        public string AdminName { get; set; }

        public string Password { get; set; }

        public string AccountNo { get; set; }
    }
}
