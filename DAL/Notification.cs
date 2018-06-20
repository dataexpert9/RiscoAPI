namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Notification
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public string Title { get; set; }

        [Required]
        public int Status { get; set; }

        public int? User_ID { get; set; }

        public int? DeliveryMan_ID { get; set; }

        public int? AdminNotification_Id { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

        [JsonIgnore]
        public virtual DeliveryMan DeliveryMan { get; set; }

        public virtual AdminNotifications AdminNotification { get; set; }
    }
}
