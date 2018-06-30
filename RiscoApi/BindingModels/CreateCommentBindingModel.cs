using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.BindingModels
{
    public class CreateCommentBindingModel
    {
        [Required]
        public string Text { get; set; }

        [Required]
        public int Post_Id { get; set; }

        public int ParentComment_Id { get; set; }
    }
}