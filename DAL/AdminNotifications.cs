using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AdminNotifications
    {
        public AdminNotifications()
        {
            Notifications = new HashSet<Notification>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int TargetAudienceType { get; set; }

        public DateTime CreatedDate { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
