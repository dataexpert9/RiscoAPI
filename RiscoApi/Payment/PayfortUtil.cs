//using BasketApi;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web;
//using WebApplication1.ViewModels;

//namespace WebApplication1.Payment
//{
//    public class PayfortUtil
//    {
//        /// <summary>
//        /// Generates SDK Token
//        /// </summary>
//        /// <returns></returns>
//        public SDKTokenResponseModel SdkToken(string deviceId)
//        {
//            try
//            {
//                SDKTokenModel requestSDK = new SDKTokenModel { device_id = deviceId };

//                PayFortConfiguration configuration = new PayFortConfiguration();

//                configuration.GetPayFortConfigurationForMobileSDK(requestSDK);

//                Uri payfortUrl;

//                if (BasketSettings.Settings.IsLive)
//                    payfortUrl = new Uri(BasketSettings.Settings.Payfort_liveUrl);
//                else
//                    payfortUrl = new Uri(BasketSettings.Settings.Payfort_TestUrl);

//                requestSDK.service_command = "SDK_TOKEN";

//                requestSDK.signature.CreateSignature(configuration, null, null, requestSDK);

//                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

//                using (var httpClient = new HttpClient())
//                {
//                    string contents = JsonConvert.SerializeObject(requestSDK);

//                    var response = httpClient.PostAsync(payfortUrl, new StringContent(contents, Encoding.UTF8, "application/json"));

//                    response.Wait();

//                    if (response != null)
//                    {
//                        var responseJson = JObject.Parse(response.Result.Content.ReadAsStringAsync().Result);

//                        return responseJson.ToObject<SDKTokenResponseModel>();
//                        //return responseJson.GetValue("status").ToObject<int>() == 04 ? "Success" : "Failed";

//                    }
//                    return new SDKTokenResponseModel();
//                    //return "Failed";
//                }
//            }
//            catch (Exception ex)
//            {
//                Utility.LogError(ex);
//                return new SDKTokenResponseModel();
//            }
//        }
//        public string Capture(CaptureBindingModel captureModel)
//        {
//            try
//            {
//                PayFortConfiguration configuration = new PayFortConfiguration();

//                configuration.GetPayFortConfiguration(captureModel);

//                Uri payfortUrl;

//                if (BasketSettings.Settings.IsLive)
//                    payfortUrl = new Uri(BasketSettings.Settings.Payfort_liveUrl);
//                else
//                    payfortUrl = new Uri(BasketSettings.Settings.Payfort_TestUrl);

//                captureModel.command = "CAPTURE";

//                captureModel.signature.CreateSignature(configuration, captureModel, null, null);

//                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

//                using (var httpClient = new HttpClient())
//                {
//                    string contents = JsonConvert.SerializeObject(captureModel);

//                    var response = httpClient.PostAsync(payfortUrl, new StringContent(contents, Encoding.UTF8, "application/json"));

//                    response.Wait();

//                    if (response != null)
//                    {
//                        var responseJson = JObject.Parse(response.Result.Content.ReadAsStringAsync().Result);

//                        return responseJson.GetValue("status").ToObject<int>() == 04 ? "Success" : "Failed";

//                    }
//                    return "Failed";
//                }
//            }
//            catch (Exception ex)
//            {
//                Utility.LogError(ex);
//                return "Failed";
//            }
//        }

//        public string Void(CaptureBindingModel captureModel)
//        {
//            try
//            {
//                PayFortConfiguration configuration = new PayFortConfiguration();

//                var uri = new Uri("https://sbpaymentservices.payfort.com/FortAPI/paymentApi");

//                configuration.GetPayFortConfiguration(captureModel);

//                VoidAuthorizationBindingModel VoidModel = new VoidAuthorizationBindingModel();

//                VoidModel.access_code = captureModel.access_code;
//                VoidModel.command = "VOID_AUTHORIZATION";
//                VoidModel.fort_id = captureModel.fort_id;
//                VoidModel.language = captureModel.language;
//                VoidModel.merchant_identifier = captureModel.merchant_identifier;
//                VoidModel.merchant_reference = captureModel.merchant_reference;

//                VoidModel.signature.CreateSignature(configuration, null, VoidModel, null);

//                string VoidModelcontents = JsonConvert.SerializeObject(VoidModel);
//                using (var httpClient = new HttpClient())
//                {
//                    var response = httpClient.PostAsync(uri, new StringContent(VoidModelcontents, Encoding.UTF8, "application/json"));

//                    response.Wait();

//                    if (response != null)
//                    {
//                        var responseJson = JObject.Parse(response.Result.Content.ReadAsStringAsync().Result);

//                        return responseJson.GetValue("status").ToObject<int>() == 04 ? "Success" : "Failed";

//                    }
//                    return "Failed";
//                }
//            }
//            catch (Exception ex)
//            {
//                Utility.LogError(ex);
//                return "Failed";
//            }
//        }
//    }
//}