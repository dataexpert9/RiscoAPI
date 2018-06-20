namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OrderPayment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderPayment()
        {
        }

        //[Key, ForeignKey("Order")]
        public int Id { get; set; }

        [Required]
        public string Amount { get; set; }

        [Required]
        public string PaymentType { get; set; }

        [Required]
        public string CashCollected { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string Order_Id { get; set; }

        [Required]
        public string AccountNo { get; set; }

        public int? DeliveryMan_Id { get; set; }

        public int Application_Id { get; set; }

        public virtual Application Application { get; set; }

        public virtual DeliveryMan DeliveryMan { get; set; }
        
        public virtual Order Order { get; set; }
    }
}
