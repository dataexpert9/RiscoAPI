namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Post
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Post()
        {
            Medias = new HashSet<Media>();
            Likes = new HashSet<Like>();
            Comments = new HashSet<Comment>();
            Shares = new HashSet<Share>();
        }

        public int Id { get; set; }

        public string Text { get; set; }

        public int Visibility { get; set; }

        public int RiskLevel { get; set; }

        public string Location { get; set; }

        public int User_Id { get; set; }

        public virtual User User { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public int? UserGroup_Id { get; set; }

        public virtual UserGroup UserGroup { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Media> Medias { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Like> Likes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Share> Shares { get; set; }

        [NotMapped]
        public bool IsLiked { get; set; }

        [NotMapped]
        public int LikesCount { get; set; }

        [NotMapped]
        public int CommentsCount { get; set; }

        [NotMapped]
        public int ShareCount { get; set; }
        [NotMapped]
        public bool IsUserFollow { get; set; }
    }
}
