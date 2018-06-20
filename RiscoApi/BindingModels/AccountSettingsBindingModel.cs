using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.BindingModels
{
    public class AccountSettingsBindingModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Language { get; set; }

        [Required]
        public bool IsLoginVerification { get; set; }

        [Required]
        public string CountryCode { get; set; }

        [Required]
        public string AboutMe { get; set; }

        [Required]
        public bool IsVideoAutoPlay { get; set; }
        
        public string Interests { get; set; }
    }
}