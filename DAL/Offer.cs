namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Offer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Offer()
        {
            Offer_Products = new HashSet<Offer_Products>();
            Offer_Packages = new HashSet<Offer_Packages>();
        }

        public int Id { get; set; }

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidUpto { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public int Status { get; set; }

        public string ImageUrl { get; set; }

        public int Store_Id { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Offer_Products> Offer_Products { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage","CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Offer_Packages> Offer_Packages { get; set; }

        public virtual Store Store { get; set; }

        public bool IsDeleted { get; set; }

        [NotMapped]
        public bool ImageDeletedOnEdit { get; set; }
    }
}
