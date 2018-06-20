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
        public int TaggingPrivacy { get; set; }

        [Required]
        public bool FindByEmail { get; set; }

        [Required]
        public bool FindByPhone { get; set; }

        [Required]
        public int MessagePrivacy { get; set; }
    }
}