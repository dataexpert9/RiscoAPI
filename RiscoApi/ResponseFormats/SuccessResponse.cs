using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace BasketApi
{
    public class CustomResponse<T>
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public T Result { get; set; }
    }

    [JsonObject(Title = "Error")]
    class Error
    {
        //public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}