namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class StoreOrder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StoreOrder()
        {
            Order_Items = new HashSet<Order_Items>();
        }

        public int Id { get; set; }

        [Required]
        public string OrderNo { get; set; }

        [Required]
        public int Status { get; set; }
        
        public int Store_Id { get; set; }
        
        public double Subtotal { get; set; }

        //public double ServiceFee { get; set; }

        //public double DeliveryFee { get; set; }

        public double Total { get; set; }

        public bool IsDeleted { get; set; }

        public int Order_Id { get; set; }

        public virtual Order Order{ get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_Items> Order_Items { get; set; }

        public virtual Store Store { get; set; }

        public bool RemoveFromDelivererHistory { get; set; }

        public bool RemoveFromUserHistory { get; set; }
    }
}
