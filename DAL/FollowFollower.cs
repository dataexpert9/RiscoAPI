namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FollowFollower
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FollowFollower()
        {
        }

        public int Id { get; set; }

        public int FirstUser_Id { get; set; }

        public virtual User FirstUser { get; set; }

        public int SecondUser_Id { get; set; }

        public virtual User SecondUser { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }

        [NotMapped]
        public bool IsFollowing { get; set; }
    }
}
