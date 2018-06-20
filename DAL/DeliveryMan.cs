namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DeliveryMan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DeliveryMan()
        {
            OrderPayments = new HashSet<OrderPayment>();
            Orders = new HashSet<Order>();
            Notifications = new HashSet<Notification>();
            DeliveryManRatings = new HashSet<DeliveryManRatings>();
            UserRatings = new HashSet<UserRatings>();
            DelivererAddresses = new HashSet<DelivererAddress>();
            AvailibilitySchedules = new HashSet<DeliveryMan_AvailibilitySchedule>();
        }

        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }
        
        public string Address1 { get; set; }
        
        public string Address2 { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [JsonIgnore]
        public string Password { get; set; }
        
        public string ZipCode { get; set; }
        
        public string DateOfBirth { get; set; }

        [Required]
        public string Phone { get; set; }
        
        public string ProfilePictureUrl { get; set; }

        [NotMapped]
        public short SignInType { get; set; }

        //public int Store_Id { get; set; }

        [NotMapped]
        public Token Token { get; set; }
        
        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderPayment> OrderPayments { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeliveryManRatings> DeliveryManRatings { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserRatings> UserRatings { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notification> Notifications { get; set; }

        //public virtual Store Store { get; set; }
        public bool IsOnline { get; set; }
        public short? Status { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneConfirmed { get; set; }

        [NotMapped]
        public Settings BasketSettings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DelivererAddress> DelivererAddresses { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public DbGeography Location { get; set; }

        public short Type { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeliveryMan_AvailibilitySchedule> AvailibilitySchedules { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsNotificationsOn { get; set; }
    }
}
