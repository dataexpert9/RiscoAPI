using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.BindingModels
{
    public class AddBoxBindingModel
    {
        public AddBoxBindingModel()
        {
            BoxVideos = new List<Video>();
        }

        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string IntroUrl { get; set; }

        public string IntroUrlThumbnail { get; set; }

        public string Description { get; set; }

        public DateTime ReleaseDate { get; set; }

        public double Price { get; set; }

        public int BoxCategory_Id { get; set; }
        
        public List<Video> BoxVideos { get; set; }
    }

    public class Video
    {
        public string VideoUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}