using PushSharp.Apple;
using PushSharp.Google;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class UserDevice
    {

        public int Id { get; set; }
        public string DeviceName { get; set; }
        public string UDID { get; set; }
        public bool Platform { get; set; }
        public int User_Id { get; set; }
        public string AuthToken { get; set; }
        public bool IsActive { get; set; }
        public User User { get; set; }

        [NotMapped]
        public GcmConfiguration AndroidPushConfiguration;
        [NotMapped]
        public ApnsConfiguration iOSPushConfiguration;
        
        public enum ApnsEnvironmentTypes
        {
            Sandbox,
            Production
        }

        public enum ApplicationTypes
        {
            PlayStore,
            Enterprise
        }
        /// <summary>
        /// Will be initialized against IOS devices(if enabled on client side)
        /// </summary>
        
        public ApnsEnvironmentTypes EnvironmentType { get; set; }
        /// <summary>
        /// Will be initialized against IOS devices(if enabled on client side)
        /// </summary>
        
        public ApplicationTypes ApplicationType { get; set; }
    }
}
