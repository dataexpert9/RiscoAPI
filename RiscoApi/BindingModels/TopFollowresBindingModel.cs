using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.BindingModels
{
    public class TopFollowersBindingModel
    {
        public int FirstUser_Id { get; set; }

        public int Count { get; set; }
    }

    public class TopFollowersListViewModel
    {
        public TopFollowersListViewModel()
        {
            TopFollowers = new List<TopFollowersBindingModel>();
        }
        public List<TopFollowersBindingModel> TopFollowers { get; set; }
    }
}