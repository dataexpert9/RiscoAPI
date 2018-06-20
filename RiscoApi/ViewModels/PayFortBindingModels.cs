using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels
{
    public class PayFortBindingModels
    {

    }
    public class PayFortConfiguration
    {

        public string access_code { get; set; }

        public string merchant_identifier { get; set; }

        public string merchant_reference { get; set; }

        public string currency { get; set; }

        public string language { get; set; }

        public string sha_type { get; set; }

        public string sha_request_phrase { get; set; }

        public string sha_response_phrase { get; set; }

    }

    public class SDKTokenModel
    {
        public string service_command { get; set; }
        public string access_code { get; set; }
        public string merchant_identifier { get; set; }
        public string language { get; set; }
        public string device_id { get; set; }
        public string signature { get; set; }
    }

    public class SDKTokenResponseModel
    {
        public string service_command { get; set; }
        public string access_code { get; set; }
        public string merchant_identifier { get; set; }
        public string language { get; set; }
        public string device_id { get; set; }
        public string signature { get; set; }
        public string sdk_token { get; set; }
        public string response_message { get; set; }
        public string response_code { get; set; }
        public string status { get; set; }
    }

    public class CaptureBindingModel
    {
        public string access_code { get; set; }

        public int amount { get; set; }

        public string command { get; set; }

        public string currency { get; set; }

        public string fort_id { get; set; }

        public string language { get; set; }
        
        public string merchant_identifier { get; set; }
        
        public string merchant_reference { get; set; }
        
        public string signature { get; set; }
    }

    public class CaptureResponseModel
    {
        public string command { get; set; }
        public string access_code { get; set; }
        public string merchant_identifier { get; set; }
        public string merchant_reference { get; set; }
        public int amount { get; set; }
        public string currency { get; set; }
        public string language { get; set; }
        public string signature { get; set; }
        public string fort_id { get; set; }
        public string order_description { get; set; }
        public string response_message { get; set; }
        public int response_code { get; set; }
        public int status { get; set; }
    }

    public class VoidAuthorizationBindingModel
    {
        public string access_code { get; set; }

        public string command { get; set; }

        public string fort_id { get; set; }

        public string language { get; set; }

        public string merchant_identifier { get; set; }

        public string merchant_reference { get; set; }

        public string signature { get; set; }
    }
}