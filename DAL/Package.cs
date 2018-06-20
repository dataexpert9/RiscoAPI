namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Package
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Package()
        {
            Offer_Packages = new HashSet<Offer_Packages>();
            //Order_Items = new HashSet<Order_Items>();
            Package_Products = new HashSet<Package_Products>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public short Status { get; set; }

        [Required]
        public double Price { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public int Store_Id { get; set; }

        [NotMapped]
        public bool ImageDeletedOnEdit { get; set; }
        //[JsonIgnore]
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Offer_Packages> Offer_Packages { get; set; }

        //[JsonIgnore]
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Order_Items> Order_Items { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Package_Products> Package_Products { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Offer_Packages> Offer_Packages { get; set; }

        public bool IsDeleted { get; set; }

        public virtual Store Store { get; set; }

    }
}
