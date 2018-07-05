namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ReportPost
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ReportPost()
        {
        }

        public int Id { get; set; }

        public int User_Id { get; set; }

        public virtual User User { get; set; }

        public int Post_Id { get; set; }

        public virtual Post Post { get; set; }

        public string Text { get; set; }

        public int ReportType { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
