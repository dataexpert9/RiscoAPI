using BasketApi.DomainModels;
using DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Spatial;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using WebApplication1.ViewModels;
using static BasketApi.Global;

namespace BasketApi
{
    public static class Utility
    {
        private static HttpClient client = new HttpClient();

        public static string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];

        public static IEnumerable<T> Page<T>(this IEnumerable<T> en, int pageSize, int page)
        {
            return en.Skip(page * pageSize).Take(pageSize);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> en, int pageSize, int page)
        {
            return en.Skip(page * pageSize).Take(pageSize);
        }

        public static async Task GenerateToken(this User user, HttpRequestMessage request)
        {
            try
            {
                var parameters = new Dictionary<string, string>{
                            { "username", user.Email },
                            { "password", user.Password },
                            { "grant_type", "password" },
                            { "signintype", Convert.ToString(user.SignInType)},
                            { "userid", user.Id.ToString()}
                        };

                var content = new FormUrlEncodedContent(parameters);
                var baseUrl = request.RequestUri.AbsoluteUri.Substring(0, request.RequestUri.AbsoluteUri.IndexOf("api"));
                var response = await client.PostAsync(baseUrl + "token", content);

                user.Token = await response.Content.ReadAsAsync<Token>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task GenerateToken(this DeliveryMan user, HttpRequestMessage request)
        {
            try
            {
                var parameters = new Dictionary<string, string>{
                            { "username", user.Email },
                            { "password", user.Password },
                            { "grant_type", "password" },
                            { "signintype", "1" },
                            { "userid", user.Id.ToString()}
                        };

                var content = new FormUrlEncodedContent(parameters);
                var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/";
                var response = await client.PostAsync(baseUrl + "token", content);

                user.Token = await response.Content.ReadAsAsync<Token>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task GenerateToken(this Admin user, HttpRequestMessage request)
        {
            try
            {
                var parameters = new Dictionary<string, string>{
                            { "username", user.Email },
                            { "password", user.Password },
                            { "grant_type", "password" },
                            { "signintype", user.Role.ToString() },
                            { "userid", user.Id.ToString()}
                        };

                var content = new FormUrlEncodedContent(parameters);
                //var baseUrl = request.RequestUri.AbsoluteUri.Substring(0, request.RequestUri.AbsoluteUri.IndexOf("api"));
                var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/";
                var response = await client.PostAsync(baseUrl + "token", content);

                user.Token = await response.Content.ReadAsAsync<Token>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static HttpStatusCode LogError(Exception ex)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + "/ErrorLog.txt"))
                {
                    sw.WriteLine("DateTime : " + DateTime.Now + Environment.NewLine);
                    if (ex.Message != null)
                    {
                        sw.WriteLine(Environment.NewLine + "Message" + ex.Message);
                        sw.WriteLine(Environment.NewLine + "StackTrace" + ex.StackTrace);
                    }
                    again: if (ex.InnerException != null)
                    {
                        sw.WriteLine(Environment.NewLine + "Inner Exception : " + ex.InnerException.Message);
                        //if (ex.InnerException.InnerException != null)
                        //{
                        //    sw.WriteLine(Environment.NewLine + "Inner Exception : " + ex.InnerException.Message);
                        //}
                    }
                    if (ex.InnerException.InnerException != null)
                    {
                        ex = ex.InnerException;
                        goto again;
                    }

                    sw.WriteLine("------******------");
                }
                return HttpStatusCode.InternalServerError;
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public static DbGeography CreatePoint(double lat, double lon, int srid = 4326)
        {
            string wkt = String.Format("POINT({0} {1})", lon, lat);

            return DbGeography.PointFromText(wkt, srid);
        }

        public static String sha256_hash(String value)
        {
            using (SHA256 hash = SHA256Managed.Create())
            {
                return String.Concat(hash
                  .ComputeHash(Encoding.UTF8.GetBytes(value))
                  .Select(item => item.ToString("x2")));
            }
        }



        public enum SubscriptionStatus
        {
            InActive,
            Active
        }

        public enum PaymentStatuses
        {
            Pending = 1,
            Completed = 2
        }
        public enum BasketEntityTypes
        {
            Product,
            Category,
            Store,
            Package,
            Admin,
            Offer,
            Box,
            Post
        }
        public enum PayfortCommands
        {
            AUTHORIZE,
            CAPTURE,
            VOID,
            PURCHASE
        }
        public enum SocialLoginType
        {
            Google = 6,
            Facebook = 7,
            Instagram = 8,
            Twitter = 9
        }

        public enum PaymentStatus
        {
            Pending,
            Paid
        }
        public static string GetOrderStatusName(int orderStatus)
        {
            try
            {
                switch (orderStatus)
                {
                    case (int)OrderStatuses.AssignedToDeliverer:
                        return "Assigned To Deliverer";
                    case (int)OrderStatuses.DelivererInProgress:
                        return "Deliverer In Progress";
                    case (int)OrderStatuses.InProgress:
                        return "In Progress";
                    case (int)OrderStatuses.ReadyForDelivery:
                        return "Ready For Delivery";
                    default:
                        return ((OrderStatuses)orderStatus).ToString();
                }

            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
                return null;
            }
        }


        public static string GetBoxCategoryName(int CategoryId)
        {
            try
            {
                switch (CategoryId)
                {
                    case (int)BoxCategoryOptions.Junior:
                        return "Junior";
                    case (int)BoxCategoryOptions.Monthly:
                        return "Monthly";
                    case (int)BoxCategoryOptions.ProBox:
                        return "TriMonthly";
                    case (int)BoxCategoryOptions.HallOfFame:
                        return "Hall Of Fame";
                    default:
                        return "Invalid";
                }

            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
                return null;
            }
        }


        public static void DeleteFileIfExists(string path)
        {
            try
            {
                var filePath = HttpContext.Current.Server.MapPath("~/" + path);

                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        public static void SendPushNotifications(List<UserDevice> usersToPushAndroid, List<UserDevice> usersToPushIOS, Notification Notification, int PushType)
        {
            try
            {

                HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                {
                    Global.objPushNotifications.SendAndroidPushNotification
                    (
                        usersToPushAndroid,
                        OtherNotification: Notification,
                        Type: PushType);

                    Global.objPushNotifications.SendIOSPushNotification
                    (
                        usersToPushIOS,
                        OtherNotification: Notification,
                        Type: PushType);
                });
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        public static string GetClaimValue(this IPrincipal currentPrincipal, string key)
        {
            var identity = currentPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
                return null;

            var claim = identity.Claims.FirstOrDefault(c => c.Type == key);
            return claim?.Value;
        }

        public static string GetPaymentMethodName(int PaymentIndex)
        {
            switch (PaymentIndex)
            {
                case (int)PaymentMethods.CashOnDelivery:
                    return "Cash On Delivery";

                case (int)PaymentMethods.CreditCard:
                    return "Credit Card";

                case (int)PaymentMethods.DebitCard:
                    return "Debit Card";
                default:
                    return "No Method Selected";

            }
        }
    }

    internal static class Youtube
    {
        private const string YoutubeLinkRegex = "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+";


        internal static string GetThumbnailUrl(string input)
        {
            var videoId = GetVideoId(input);
            return "https://img.youtube.com/vi/" + videoId + "/0.jpg";
        }
        internal static string GetVideoId(string input)
        {
            var regex = new Regex(YoutubeLinkRegex, RegexOptions.Compiled);
            foreach (Match match in regex.Matches(input))
            {
                //Console.WriteLine(match);
                foreach (var groupdata in match.Groups.Cast<Group>().Where(groupdata => !groupdata.ToString().StartsWith("http://") && !groupdata.ToString().StartsWith("https://") && !groupdata.ToString().StartsWith("youtu") && !groupdata.ToString().StartsWith("www.")))
                {
                    return groupdata.ToString();
                }
            }
            return string.Empty;
        }
    }

    public class EmailUtil
    {
        public static string FromEmail = ConfigurationManager.AppSettings["FromMailAddress"];
        public static string FromName = ConfigurationManager.AppSettings["FromMailName"];
        public static string FromPassword = ConfigurationManager.AppSettings["FromPassword"];
        public static MailAddress FromMailAddress = new MailAddress(FromEmail, FromName);
    }


    public static class MvcControllerCustom
    {
        public static string RenderViewToString(string controllerName,
                                         string viewName,
                                         object viewData, HttpContextWrapper contextBase)
        {

            var routeData = new System.Web.Routing.RouteData();
            routeData.Values.Add("controller", controllerName);
            var controllerContext = new System.Web.Mvc.ControllerContext(contextBase,
                                                          routeData,
                                                          new EmptyController());
            var razorViewEngine = new System.Web.Mvc.RazorViewEngine();
            var razorViewResult = razorViewEngine.FindPartialView(controllerContext,
                                                           viewName,
                                                           false);

            var writer = new StringWriter();
            var viewContext = new System.Web.Mvc.ViewContext(controllerContext,
                                              razorViewResult.View,
                                              new System.Web.Mvc.ViewDataDictionary(viewData),
                                              new System.Web.Mvc.TempDataDictionary(),
                                              writer);
            razorViewResult.View.Render(viewContext, writer);
            return writer.ToString();
        }
    }

    class EmptyController : System.Web.Mvc.ControllerBase
    {
        protected override void ExecuteCore() { }
    }
}