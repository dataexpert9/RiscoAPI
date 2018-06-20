namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Application
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Application()
        {
            OrderPayments = new HashSet<OrderPayment>();
        }

        public int Id { get; set; }

        [Required]
        public string OwnerName { get; set; }

        [Required]
        public string OwnerPassword { get; set; }

        [Required]
        public string AccountNo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderPayment> OrderPayments { get; set; }
    }
}
