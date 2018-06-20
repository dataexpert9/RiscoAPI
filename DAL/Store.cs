namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Store
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Store()
        {
            Categories = new HashSet<Category>();
            DeliveryMen = new HashSet<DeliveryMan>();
            Offers = new HashSet<Offer>();
            StoreOrders = new HashSet<StoreOrder>();
            Products = new HashSet<Product>();
            Admins = new HashSet<Admin>();
            StoreRatings = new HashSet<StoreRatings>();
            Packages = new HashSet<Package>();
            Feedback = new HashSet<ContactUs>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string ImageUrl { get; set; }
        
        public string BannerUrl { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }

        public TimeSpan Open_From { get; set; }

        public TimeSpan Open_To { get; set; }
        
        [NotMapped]
        public bool ImageDeletedOnEdit { get; set; }
        
        [NotMapped]
        public double AverageRating { get; set; }

        public DbGeography Location { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Category> Categories { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeliveryMan> DeliveryMen { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Offer> Offers { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreOrder> StoreOrders { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Admin> Admins { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreRatings> StoreRatings { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Package> Packages { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual StoreDeliveryHours StoreDeliveryHours { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContactUs> Feedback { get; set; }

        public bool IsDeleted { get; set; }
    }
}
