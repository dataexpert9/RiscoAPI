namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class HideAllPost
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HideAllPost()
        {
        }

        public int Id { get; set; }

        public int FirstUserAll_Id { get; set; }

        public virtual User FirstUserAll { get; set; }

        public int SecondUserAll_Id { get; set; }

        public virtual User SecondUserAll { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }

    }
}
