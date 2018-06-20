using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.BindingModels
{
    public class NotificationSettingsBindingModel
    {
        [Required]
        public bool MuteUnverifiedEmail { get; set; }

        [Required]
        public bool MuteUnverifiedPhone { get; set; }
    }
}