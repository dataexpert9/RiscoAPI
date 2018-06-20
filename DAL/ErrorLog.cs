namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ErrorLog
    {
        public int Id { get; set; }

        [Required]
        public string ErrorMessage { get; set; }

        [Required]
        public string StackTrace { get; set; }

        [Required]
        public string Source { get; set; }
    }
}
