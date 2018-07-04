namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Comment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Comment()
        {
            Likes = new HashSet<Like>();
            ChildComments = new List<Comment>();
            //TrendLogs = new List<TrendLog>();
        }

        public int Id { get; set; }

        public string Text { get; set; }

        public int? Post_Id { get; set; }

        public virtual Post Post { get; set; }

        public int ParentComment_Id { get; set; }

        public int User_Id { get; set; }

        public virtual User User { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Like> Likes { get; set; }

        //[JsonIgnore]
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<TrendLog> TrendLogs { get; set; }

        [NotMapped]
        public List<Comment> ChildComments { get; set; }

        [NotMapped]
        public bool IsLiked { get; set; }
    }
}
