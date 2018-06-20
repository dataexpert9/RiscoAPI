using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.ViewModels
{
    public class FavouritesViewModel
    {
        public IEnumerable<Favourite> Favourites { get; set; }
    }
}