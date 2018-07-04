using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Areas.Admin.ViewModels
{
    public class PostListViewModel
    {
        public PostListViewModel()
        {
            Posts = new List<Post>();
        }
        public int PostCount { get; set; }
        public List<Post> Posts { get; set; }
    }
}