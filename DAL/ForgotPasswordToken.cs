namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ForgotPasswordToken
    {
        public int Id { get; set; }
        
        [Required]
        public string Code { get; set; }
        
        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public int User_ID { get; set; }

        public virtual User User { get; set; }
        
        
        
    }
}
