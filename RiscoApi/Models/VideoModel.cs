using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class VideoModel
    {
        [Url]
        public string VideoUrl { get; set; }
    }
}