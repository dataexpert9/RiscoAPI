using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.ViewModels
{
    public class SearchAdminViewModel
    {
        public int Id { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string Email { get; set; }

        public string Phone { get; set; }
        
        public short Role { get; set; }
        
        public string AccountNo { get; set; }

        public int? Store_Id { get; set; }

        public string StoreName { get; set; }

        public string ImageUrl { get; set; }
    }

    public class SearchAdminListViewModel
    {
        public IEnumerable<SearchAdminViewModel> Admins { get; set; }
    }
}