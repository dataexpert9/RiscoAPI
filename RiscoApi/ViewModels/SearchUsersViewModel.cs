using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.ViewModels
{
    public class SearchUsersViewModel
    {
        public SearchUsersViewModel()
        {
            Users = new List<User>();
        }
        public List<User> Users { get; set; }
    }
}