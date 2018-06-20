using BasketApi.CustomAuthorization;
using BasketApi.Models;
using BasketApi.ViewModels;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using WebApplication1.BindingModels;
using static BasketApi.Global;

namespace BasketApi.Controllers
{
    [BasketApi.Authorize("User", "Guest", "Deliverer")]
    [RoutePrefix("api/User")]
    public class NotificationsController : ApiController
    {
        /// <summary>
        /// Get All Notifications
        /// </summary>
        /// <param name="userId">User unique identifier</param>
        /// <param name="SignInType">0 for User, 1 for deliverer</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetNotifications")]
        public async Task<IHttpActionResult> GetNotifications(int UserId, int SignInType)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    NotificationsViewModel notificationsViewModel = new NotificationsViewModel();

                    if (SignInType == (int)RoleTypes.User)
                        notificationsViewModel.Notifications = ctx.Notifications.Where(x => x.User_ID.HasValue && x.User_ID.Value == UserId && x.Status == 0).OrderByDescending(x => x.Id).ToList();
                    else if (SignInType == (int)RoleTypes.Deliverer)
                        notificationsViewModel.Notifications = ctx.Notifications.Where(x => x.DeliveryMan_ID.HasValue && x.DeliveryMan_ID.Value == UserId && x.Status == 0).OrderByDescending(x => x.Id).ToList();

                    CustomResponse<NotificationsViewModel> response = new CustomResponse<NotificationsViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = notificationsViewModel };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        /// <summary>
        /// Mark notification as read.
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("MarkNotificationAsRead")]
        public async Task<IHttpActionResult> MarkNotificationAsRead(int NotificationId)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var notification = ctx.Notifications.FirstOrDefault(x => x.Id == NotificationId);

                    if (notification != null)
                    {
                        notification.Status = (int)Global.NotificationStatus.Read;
                        ctx.SaveChanges();
                        CustomResponse<string> response = new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK };
                        return Ok(response);
                    }
                    else
                    {
                        CustomResponse<Error> response = new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "Invalid notificationid" } };
                        return Ok(response);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        /// <summary>
        /// Turn user notifications on or off
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="On"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("UserNoticationsOnOff")]
        public async Task<IHttpActionResult> UserNoticationsOnOff(int UserId, int SignInType, bool On)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    if (SignInType == (int)RoleTypes.User)
                    {

                        var user = ctx.Users.FirstOrDefault(x => x.Id == UserId);
                        if (user != null)
                        {
                            user.IsNotificationsOn = On;
                            ctx.SaveChanges();
                        }
                    }
                    else
                    {
                        var deliveryMan = ctx.DeliveryMen.FirstOrDefault(x => x.Id == UserId);
                        if (deliveryMan != null)
                        {
                            deliveryMan.IsNotificationsOn = On;
                            ctx.SaveChanges();
                        }
                    }

                }
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

    }
}
