namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Status
    {
        public int Id { get; set; }

        [Required]
        public string StatusId { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
