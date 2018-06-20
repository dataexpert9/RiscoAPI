namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PaymentCard
    {
        public int Id { get; set; }

        [Required]
        public string CardNumber { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        public string CCV { get; set; }

        [Required]
        public string NameOnCard { get; set; }

        [Required]
        public int CardType { get; set; } //1 for Credit, 2 for Debit

        public int User_ID { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public virtual User User { get; set; }
    }
}
