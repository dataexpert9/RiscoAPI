namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            PaymentCards = new HashSet<PaymentCard>();
            Favourites = new HashSet<Favourite>();
            ForgotPasswordTokens = new HashSet<ForgotPasswordToken>();
            Notifications = new HashSet<Notification>();
            Orders = new HashSet<Order>();
            ProductRatings = new HashSet<ProductRating>();
            UserRatings = new HashSet<UserRatings>();
            DeliveryManRatings = new HashSet<DeliveryManRatings>();
            AppRatings = new HashSet<AppRatings>();
            UserAddresses = new HashSet<UserAddress>();
            UserDevices = new HashSet<UserDevice>();
            StoreRatings = new HashSet<StoreRatings>();
            UserSubscriptions = new HashSet<UserSubscriptions>();
            Feedback = new HashSet<ContactUs>();
            VerifyNumberCodes = new HashSet<VerifyNumberCodes>();
            Posts = new HashSet<Post>();
        }

        public int Id { get; set; }

        //[StringLength(100)]
        //public string FirstName { get; set; }

        //[StringLength(100)]
        //public string LastName { get; set; }

        [StringLength(200)]
        public string FullName { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        //public string AccountType { get; set; }

        //public string ZipCode { get; set; }

        //public string DateofBirth { get; set; }

        public int? SignInType { get; set; }

        public string UserName { get; set; }

        public short? Status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaymentCard> PaymentCards { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Favourite> Favourites { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserRatings> UserRatings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DeliveryManRatings> DeliveryManRatings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<ForgotPasswordToken> ForgotPasswordTokens { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notification> Notifications { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AppRatings> AppRatings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductRating> ProductRatings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserAddress> UserAddresses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserDevice> UserDevices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreRatings> StoreRatings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserSubscriptions> UserSubscriptions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContactUs> Feedback { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VerifyNumberCodes> VerifyNumberCodes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Post> Posts { get; set; }

        public bool IsNotificationsOn { get; set; }

        [NotMapped]
        public Token Token { get; set; }

        [NotMapped]
        public Settings BasketSettings { get; set; }

        public int Gender { get; set; }

        public string Language { get; set; }

        /// <summary>
        /// Two Way Authentication
        /// </summary>
        public bool IsLoginVerification { get; set; }

        public string CountryCode { get; set; }

        public string AboutMe { get; set; }

        public bool IsVideoAutoPlay { get; set; }

        public string Interests { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool PhoneConfirmed { get; set; }

        #region Notifications Settings

        //public bool IsPeopleIDontFollow { get; set; }

        //public bool IsPeopleWhoDontFollowMe { get; set; }

        //public bool IsPeopleWithNewAccount { get; set; }

        //public bool IsPeopleWithDefaultProfilePhoto { get; set; }

        /// <summary>
        /// Mute Notification From People With Unverified Email
        /// </summary>
        public bool MuteUnverifiedEmail { get; set; }

        /// <summary>
        /// Mute Notification From People With Unverified Phone
        /// </summary>
        public bool MuteUnverifiedPhone { get; set; }

        #endregion

        #region Privacy Settings

        #region Post Settings
        public bool IsPostLocation { get; set; }

        #endregion

        #region Post Tagging

        /// <summary>
        /// Is Allow Anyone To Tag You In Posts
        /// </summary>
        public bool TagAnyone { get; set; }

        /// <summary>
        /// Is Allow Only People You Follow To Tag You In Posts
        /// </summary>

        public bool TagYouFollow { get; set; }

        /// <summary>
        /// Is Allow People Who Follow Me To Tag You In Posts
        /// </summary>

        public bool TagFollowMe { get; set; }

        /// <summary>
        /// Is Dont Allow Anyone To Tag You In Posts
        /// </summary>
        public bool TagNotAllowed { get; set; }

        #endregion

        #region Discoverability

        public bool FindByEmail { get; set; }

        public bool FindByPhone { get; set; }

        #endregion

        #region Direct Messages

        /// <summary>
        /// Is Allow AnyOne To Send Direct Message
        /// </summary>
        public bool MessageAnyone { get; set; }

        /// <summary>
        /// Is Allow Only People You Follow To Send Direct Message
        /// </summary>
        public bool MessageYouFollow { get; set; }

        /// <summary>
        /// Is Allow People Who Follow Me To Send Direct Message
        /// </summary>
        public bool MessageFollowMe { get; set; }

        /// <summary>
        /// Is Dont Allow AnyOne To Send Direct Message
        /// </summary>
        public bool MessageNotAllowed { get; set; }

        #endregion

        #endregion

        public bool IsDeleted { get; set; }

        [NotMapped]
        public int PostCount { get; set; }

        [NotMapped]
        public int FollowingCount { get; set; }

        [NotMapped]
        public int FollowersCount { get; set; }

    }
}
