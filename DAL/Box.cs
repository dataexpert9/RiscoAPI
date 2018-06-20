using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Box
    {
        public Box()
        {
            BoxVideos = new HashSet<BoxVideos>();
            UserSubscriptions = new HashSet<UserSubscriptions>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string IntroUrl { get; set; }

        public string IntroUrlThumbnail { get; set; }

        public bool IsDeleted { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ReleaseDate { get; set; }

        public int BoxCategory_Id { get; set; }

        public double Price { get; set; }

        public short Status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BoxVideos> BoxVideos { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserSubscriptions> UserSubscriptions { get; set; }

        [NotMapped]
        public bool AlreadySubscribed { get; set; }
    }
}
