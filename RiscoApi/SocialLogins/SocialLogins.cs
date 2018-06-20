using BasketApi.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace BasketApi
{
    public class SocialLogins
    {
        public async Task<SocialUserViewModel> GetSocialUserData(string access_token, SocialLoginType socialLoginType)
        {
            try
            {
                HttpClient client = new HttpClient();
                string urlProfile = "";

                if (socialLoginType == SocialLoginType.Google)
                    urlProfile = "https://www.googleapis.com/oauth2/v3/tokeninfo?id_token=" + access_token;
                else if (socialLoginType == SocialLoginType.Facebook)
                    urlProfile = "https://graph.facebook.com/v2.4/me?fields=id,name,email,first_name,last_name&access_token=" + access_token;
                
                client.CancelPendingRequests();
                HttpResponseMessage output = await client.GetAsync(urlProfile);

                if (output.IsSuccessStatusCode)
                {
                    string outputData = await output.Content.ReadAsStringAsync();
                    SocialUserViewModel socialUser = JsonConvert.DeserializeObject<SocialUserViewModel>(outputData);
                    if (socialLoginType == SocialLoginType.Facebook)
                        socialUser.picture = "http://graph.facebook.com/" + socialUser.id + "/picture?type=large";
                    return socialUser;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public enum SocialLoginType
        {
            Google,
            Facebook,
            Instagram,
            Twitter
        }
    }
}