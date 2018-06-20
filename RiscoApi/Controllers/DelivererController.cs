using BasketApi;
using BasketApi.CustomAuthorization;
using BasketApi.ViewModels;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using BasketApi.Models;
using static BasketApi.Global;

namespace BasketApi.Controllers
{
    [BasketApi.Authorize("Deliverer")]
    [RoutePrefix("api/Deliverer")]
    public class DelivererController : ApiController
    {
        /// <summary>
        /// Mark deliverer as online offline
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="IsOnline"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("MarkOnlineOffline")]
        public async Task<IHttpActionResult> MarkOnlineOffline(int userId, bool IsOnline)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var user = ctx.DeliveryMen.FirstOrDefault(x => x.Id == userId);
                    if (user != null)
                    {
                        user.IsOnline = IsOnline;
                        ctx.SaveChanges();
                        return Ok(new CustomResponse<string>
                        {
                            Message = Global.ResponseMessages.Success,
                            StatusCode = (int)HttpStatusCode.OK
                        });
                    }
                    else
                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "Invalid userid" } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        /// <summary>
        /// Update deliverers availibility schedule
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="availibility"> true for available, false for unavailable</param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("MarkAvailibility")]
        public async Task<IHttpActionResult> MarkAvailibilitySchedule(int userId, bool availibility, string StartDate, string EndDate)
        {
            try
            {
                DateTime startDateTime;
                DateTime endDateTime;
                DateTime.TryParse(StartDate, out startDateTime);
                DateTime.TryParse(EndDate, out endDateTime);

                if (startDateTime == DateTime.MinValue || endDateTime == DateTime.MinValue)
                    return Ok(new CustomResponse<Error> { Message = "BadRequest", StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "Invalid startdate or enddate" } });

                using (RiscoContext ctx = new RiscoContext())
                {
                    var deliveryMan = ctx.DeliveryMen.Include(x => x.AvailibilitySchedules).FirstOrDefault(x => x.Id == userId);
                    if (deliveryMan != null)
                    {
                        var existingSchedule = deliveryMan.AvailibilitySchedules.FirstOrDefault(x => x.StartDate.Month == startDateTime.Month && x.StartDate.Year == startDateTime.Year && x.IsAvailable == availibility && x.IsDeleted == false);
                        if (existingSchedule != null)
                        {
                            existingSchedule.StartDate = startDateTime;
                            existingSchedule.EndDate = endDateTime;
                        }
                        else
                            deliveryMan.AvailibilitySchedules.Add(new DeliveryMan_AvailibilitySchedule { IsAvailable = availibility, StartDate = startDateTime, EndDate = endDateTime });

                        ctx.SaveChanges();
                        return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });

                    }
                    else
                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "Invalid userid" } });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("GetAvailibilityScheduleByDateRange")]
        public async Task<IHttpActionResult> GetAvailibilityScheduleByDateRange(int userId, bool availibility, string StartDate)
        {
            try
            {
                DateTime startDateTime;
                DateTime.TryParse(StartDate, out startDateTime);

                using (RiscoContext ctx = new RiscoContext())
                {
                    var schedule = ctx.DeliveryManAvailibilitySchedules.FirstOrDefault(x => x.IsDeleted == false && x.IsAvailable == availibility && x.StartDate.Month == startDateTime.Month && x.StartDate.Year == startDateTime.Year);
                    return Ok(new CustomResponse<DeliveryMan_AvailibilitySchedule> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = schedule });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("DeleteAvailibilityScheduleByDateRange")]
        public async Task<IHttpActionResult> DeleteAvailibilityScheduleByDateRange(int userId, bool availibility, string StartDate)
        {
            try
            {
                DateTime startDateTime;
                DateTime.TryParse(StartDate, out startDateTime);

                using (RiscoContext ctx = new RiscoContext())
                {
                    var schedule = ctx.DeliveryManAvailibilitySchedules.FirstOrDefault(x => x.IsDeleted == false && x.IsAvailable == availibility && x.StartDate.Month == startDateTime.Month && x.StartDate.Year == startDateTime.Year);
                    if (schedule != null)
                    {
                        schedule.IsDeleted = true;
                        ctx.SaveChanges();
                        return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                    }
                    else
                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "No schedule to delete" } });

                }

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        /// <summary>
        /// Edit deliverer profile without image
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("EditProfile")]
        public async Task<IHttpActionResult> EditProfile(DelivererEditProfileBindingModel model)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var deliverer = ctx.DeliveryMen.FirstOrDefault(x => x.Id == model.Id);

                    if (deliverer != null)
                    {
                        deliverer.Address1 = model.Address1;
                        deliverer.Address2 = model.Address2;
                        deliverer.FullName = model.FullName;
                        deliverer.Phone = model.PhoneNumber;
                        ctx.SaveChanges();

                        return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                    }
                    else
                        return Ok(new CustomResponse<Error> { Message = "NotFound", StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "User not found" } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpPost]
        [Route("UpdateLocation")]
        public async Task<IHttpActionResult> UpdateLocation(LocationBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using (RiscoContext ctx = new RiscoContext())
                {
                    var deliverer = ctx.DeliveryMen.FirstOrDefault(x => x.Id == model.Id);
                    if (deliverer != null)
                    {
                        deliverer.Longitude = model.Longitude;
                        deliverer.Latitude = model.Latitude;
                        deliverer.Location = Utility.CreatePoint(model.Latitude, model.Longitude);
                        ctx.SaveChanges();

                        return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                    }
                    else
                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "Invalid deliverer Id" } });

                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }

        }


        [HttpGet]
        [Route("GetDeliveryScheduleByDateRange")]
        public async Task<IHttpActionResult> GetDeliveryScheduleByDateRange(int DelivererId, string StartDate, string EndDate, int PageSize, int PageNo)
        {
            try
            {
                DateTime startDateTime;
                DateTime endDateTime;
                DateTime.TryParse(StartDate, out startDateTime);
                DateTime.TryParse(EndDate, out endDateTime);

                using (RiscoContext ctx = new RiscoContext())
                {
                    BasketApi.AppsViewModels.OrdersHistoryViewModel model = new AppsViewModels.OrdersHistoryViewModel();
                    //BasketApi.AppsViewModels.OrdersScheduleViewModel model = new AppsViewModels.OrdersScheduleViewModel();

                    model.Count = ctx.Orders.Count(x => x.DeliveryMan_Id == DelivererId && DbFunctions.TruncateTime(x.DeliveryTime_From) >= startDateTime.Date && DbFunctions.TruncateTime(x.DeliveryTime_From) <= endDateTime.Date);

                    string orderQuery = String.Empty;

                    #region ordersQuery
                    orderQuery = @"
SELECT *, Users.FullName as UserFullName, Users.profilepictureurl as UserProfilePictureUrl FROM Orders 
join Users on Users.ID = Orders.User_ID
where Orders.DeliveryMan_Id = " + DelivererId +
@" and CAST(DeliveryTime_From AS DATE) >= '" + startDateTime + @"' and CAST(DeliveryTime_From AS DATE) <= '" + endDateTime.Date +
@"'  and Orders.IsDeleted = 0 and Users.IsDeleted = 0
ORDER BY Orders.id desc OFFSET " + PageNo * PageSize + " ROWS FETCH NEXT " + PageSize + " ROWS ONLY;";
                    #endregion

                    model.orders = ctx.Database.SqlQuery<BasketApi.AppsViewModels.OrderViewModel>(orderQuery).ToList();
                    if (model.orders.Count == 0)
                    {
                        return Ok(new CustomResponse<AppsViewModels.OrdersHistoryViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = model });
                    }
                    var orderIds = string.Join(",", model.orders.Select(x => x.Id.ToString()));
                    string storeOrderQuery = String.Empty;
                    #region storeOrdersQuery
                    storeOrderQuery = @"
select
StoreOrders.*,
Stores.Name as StoreName,
Stores.ImageUrl from StoreOrders 
join Stores on Stores.Id = StoreOrders.Store_Id
where 
Order_Id in (" + orderIds + @")
";
                    #endregion

                    var storeOrders = ctx.Database.SqlQuery<BasketApi.AppsViewModels.StoreOrderViewModel>(storeOrderQuery).ToList();

                    if (storeOrders.Count == 0)
                    {
                        model.orders.Clear();
                        return Ok(new CustomResponse<AppsViewModels.OrdersHistoryViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = model });
                    }

                    var storeOrderIds = string.Join(",", storeOrders.Select(x => x.Id.ToString()));

                    #region OrderItemsQuery
                    var orderItemsQuery = @"
SELECT
  CASE
    WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN Products.Id
    WHEN ISNULL(Order_Items.Package_Id, 0) <> 0 THEN Packages.Id
    WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN Offer_Products.Id
    WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN Offer_Packages.Id
  END AS ItemId,
  Order_Items.Name AS Name,
  Order_Items.Price AS Price,
  CASE
    WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN Products.ImageUrl
    WHEN ISNULL(Order_Items.Package_Id, 0) <> 0 THEN Packages.ImageUrl
    WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN Offer_Products.ImageUrl
    WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN Offer_Packages.ImageUrl
  END AS ImageUrl,
  Order_Items.Id,
  Order_Items.Qty,
  ISNULL(Products.WeightInGrams,0) as WeightInGrams,
  ISNULL(Products.WeightInKiloGrams,0) as WeightInKiloGrams,
  Order_Items.StoreOrder_Id
FROM Order_Items
LEFT JOIN products
  ON products.Id = Order_Items.Product_Id
LEFT JOIN Packages
  ON Packages.Id = Order_Items.Package_Id
LEFT JOIN Offer_Products
  ON Offer_Products.Id = Order_Items.Offer_Product_Id
LEFT JOIN Offer_Packages
  ON Offer_Packages.Id = Order_Items.Offer_Package_Id
WHERE StoreOrder_Id IN (" + storeOrderIds + ")";
                    #endregion

                    var orderItems = ctx.Database.SqlQuery<BasketApi.AppsViewModels.OrderItemViewModel>(orderItemsQuery).ToList();

                    foreach (var orderItem in orderItems)
                    {
                        storeOrders.FirstOrDefault(x => x.Id == orderItem.StoreOrder_Id).OrderItems.Add(orderItem);
                    }

                    foreach (var storeOrder in storeOrders)
                    {
                        model.orders.FirstOrDefault(x => x.Id == storeOrder.Order_Id).StoreOrders.Add(storeOrder);
                    }

                    return Ok(new CustomResponse<AppsViewModels.OrdersHistoryViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = model });
                    //var orders = ctx.Orders.Include(x => x.User).Where(x => x.DeliveryMan_Id == DelivererId && DbFunctions.TruncateTime(x.DeliveryTime_From) >= startDateTime.Date && DbFunctions.TruncateTime(x.DeliveryTime_From) <= endDateTime.Date).OrderByDescending(x => x.Id).Page(PageSize, PageNo).ToList();

                    //model.Orders = orders;


                    //return Ok(new CustomResponse<BasketApi.AppsViewModels.OrdersScheduleViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = model });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpPost]
        [Route("ChangeOrderStatus")]
        public async Task<IHttpActionResult> ChangeOrderStatus(int OrderId, int Status)
        {
            try
            {
                if (Status >= (int)OrderStatuses.DelivererInProgress && Status <= (int)OrderStatuses.Completed)
                {
                    using (RiscoContext ctx = new RiscoContext())
                    {
                        ctx.Orders.FirstOrDefault(x => x.Id == OrderId).Status = Status;
                        ctx.SaveChanges();
                        return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                    }
                }
                else
                    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "Invalid status code" } });

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }

        }
    }
}