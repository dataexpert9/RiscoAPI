using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.BindingModels
{
    public class PrivacySettingsBindingModel
    {
        [Required]
        public bool IsPostLocation { get; set; }

        [Required]
        public bool TagAnyone { get; set; }

        [Required]
        public bool TagYouFollow { get; set; }

        [Required]
        public bool TagFollowMe { get; set; }

        [Required]
        public bool TagNotAllowed { get; set; }

        [Required]
        public bool FindByEmail { get; set; }

        [Required]
        public bool FindByPhone { get; set; }

        [Required]
        public bool MessageAnyone { get; set; }

        [Required]
        public bool MessageYouFollow { get; set; }

        [Required]
        public bool MessageFollowMe { get; set; }

        [Required]
        public bool MessageNotAllowed { get; set; }

    }
}