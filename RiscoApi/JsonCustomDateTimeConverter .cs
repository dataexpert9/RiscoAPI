using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi
{
    public class JsonCustomDateTimeConverter : IsoDateTimeConverter
    {
        public JsonCustomDateTimeConverter()
        {
            DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        }
    }
}