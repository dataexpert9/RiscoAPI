namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Category
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public short Status { get; set; }

        public int Store_Id { get; set; }

        public string ImageUrl { get; set; }

        public int ParentCategoryId { get; set; }
        
        public bool IsDeleted { get; set; }

        [NotMapped]
        public int ProductCount { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }

        [JsonIgnore]
        public virtual Store Store { get; set; }

        [NotMapped]
        public bool ImageDeletedOnEdit { get; set; }
    }
}
