using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.BindingModels
{
    public class PostBindingModel
    {
        [Required]
        public string Text { get; set; }

        [Required]
        public int Visibility { get; set; }

        [Required]
        public int RiskLevel { get; set; }

        public string Location { get; set; }
    }
}