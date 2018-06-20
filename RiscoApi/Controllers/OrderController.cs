using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using BasketApi.ViewModels;
using BasketApi.DomainModels;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using static BasketApi.Global;
using Newtonsoft.Json;
using BasketApi.CustomAuthorization;
using System.Net.Mail;
using Microsoft.Reporting.WinForms;
using System.IO;
using System.Web;
using System.Configuration;
using System.Web.Hosting;
using WebApplication1.PDFConverter;
using WebApplication1.ViewModels;
using System.Text;
using Newtonsoft.Json.Linq;
//using WebApplication1.Payment;
using static BasketApi.Utility;

namespace BasketApi.Controllers
{

    [RoutePrefix("api/Order")]
    public class OrderController : ApiController
    {
        [BasketApi.Authorize("User", "Deliverer")]
        /// <summary>
        /// Delete user order
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("DeleteOrderFromHistory")]
        public async Task<IHttpActionResult> DeleteOrderFromHistory(int UserId, int OrderId, int StoreOrderId, int SignInType, bool StoreOrder)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    if (SignInType == (int)RoleTypes.User)
                    {
                        if (ctx.Users.Any(x => x.Id == UserId))
                        {
                            if (StoreOrder)
                            {
                                var order = ctx.Orders.Include(x => x.StoreOrders).FirstOrDefault(x => x.Id == OrderId);

                                order.StoreOrders.FirstOrDefault(x => x.Id == StoreOrderId).RemoveFromUserHistory = true;

                                order.RemoveFromUserHistory = !(order.StoreOrders.Any(x => x.RemoveFromUserHistory == false));
                            }
                            else
                            {
                                var order = ctx.Orders.FirstOrDefault(x => x.Id == OrderId);
                                order.RemoveFromUserHistory = true;
                            }

                            ctx.SaveChanges();
                            return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                        }
                        else
                            return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "Invalid UserId" } });
                    }
                    else
                    {
                        if (ctx.DeliveryMen.Any(x => x.Id == UserId))
                        {
                            if (StoreOrder)
                            {
                                var order = ctx.Orders.Include(x => x.StoreOrders).FirstOrDefault(x => x.Id == OrderId);

                                order.StoreOrders.FirstOrDefault(x => x.Id == StoreOrderId).RemoveFromUserHistory = true;

                                order.RemoveFromUserHistory = !(order.StoreOrders.Any(x => x.RemoveFromUserHistory == false));
                            }
                            else
                            {
                                var order = ctx.Orders.FirstOrDefault(x => x.Id == OrderId);
                                order.RemoveFromDelivererHistory = true;
                            }

                            ctx.SaveChanges();

                            return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                        }
                        else
                            return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "Invalid UserId" } });
                    }

                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("User", "Deliverer")]
        /// <summary>
        /// Get User's previous orders
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageNo"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetOrdersHistory")]
        public async Task<IHttpActionResult> GetOrdersHistory(int UserId, int SignInType, bool IsCurrentOrder, int PageSize, int PageNo)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    BasketApi.AppsViewModels.OrdersHistoryViewModel orderHistory = new AppsViewModels.OrdersHistoryViewModel();

                    if (SignInType == (int)RoleTypes.User)
                    {
                        if (IsCurrentOrder)
                            orderHistory.Count = ctx.Orders.Count(x => x.User_ID == UserId && x.IsDeleted == false && x.Status != (int)OrderStatuses.Completed && x.RemoveFromUserHistory == false);
                        else
                            orderHistory.Count = ctx.Orders.Count(x => x.User_ID == UserId && x.IsDeleted == false && x.Status == (int)OrderStatuses.Completed && x.RemoveFromUserHistory == false);
                    }
                    else
                    {
                        if (IsCurrentOrder)
                            orderHistory.Count = ctx.Orders.Count(x => x.DeliveryMan_Id == UserId && x.IsDeleted == false && x.Status != (int)OrderStatuses.Completed && x.RemoveFromDelivererHistory == false);
                        else
                            orderHistory.Count = ctx.Orders.Count(x => x.DeliveryMan_Id == UserId && x.IsDeleted == false && x.Status == (int)OrderStatuses.Completed && x.RemoveFromDelivererHistory == false);
                    }

                    if (orderHistory.Count == 0)
                    {
                        return Ok(new CustomResponse<AppsViewModels.OrdersHistoryViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = orderHistory });
                    }

                    #region OrderQuery
                    string orderQuery = String.Empty;
                    if (SignInType == (int)RoleTypes.User)
                    {
                        orderQuery = @"
                        SELECT *, Users.FullName as UserFullName FROM Orders 
                        join Users on Users.ID = Orders.User_ID
                        where Orders.User_Id = " + UserId + @" and Orders.IsDeleted = 0 and Users.IsDeleted = 0 and Orders.RemoveFromUserHistory = 0 and " + (IsCurrentOrder ? " Orders.Status <> " + (int)OrderStatuses.Completed : " Orders.Status = " + (int)OrderStatuses.Completed) +
                        @"ORDER BY Orders.id desc OFFSET " + PageNo * PageSize + " ROWS FETCH NEXT " + PageSize + " ROWS ONLY;";
                    }
                    else
                    {
                        orderQuery = @"
                        SELECT *, Users.FullName as UserFullName FROM Orders 
                        join Users on Users.ID = Orders.User_ID
                        join DeliveryMen on DeliveryMen.ID = Orders.DeliveryMan_Id
                        where Orders.DeliveryMan_Id = " + UserId + @" and Orders.IsDeleted = 0 and Orders.RemoveFromDelivererHistory = 0 and " + (IsCurrentOrder ? " Orders.Status <> " + (int)OrderStatuses.Completed : " Orders.Status = " + (int)OrderStatuses.Completed) +
                        @"ORDER BY Orders.id desc OFFSET " + PageNo * PageSize + " ROWS FETCH NEXT " + PageSize + " ROWS ONLY;";
                    }

                    #endregion

                    orderHistory.orders = ctx.Database.SqlQuery<BasketApi.AppsViewModels.OrderViewModel>(orderQuery).ToList();
                    if (orderHistory.orders.Count == 0)
                    {
                        return Ok(new CustomResponse<AppsViewModels.OrdersHistoryViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = orderHistory });
                    }
                    var orderIds = string.Join(",", orderHistory.orders.Select(x => x.Id.ToString()));

                    #region StoreOrderQuery
                    string storeOrderQuery = String.Empty;
                    if (SignInType == (int)RoleTypes.User)
                    {
                        storeOrderQuery = @"
select
StoreOrders.*,
Stores.Name as StoreName,
Stores.ImageUrl from StoreOrders 
join Stores on Stores.Id = StoreOrders.Store_Id
where 
Order_Id in (" + orderIds + @")
and StoreOrders.RemoveFromUserHistory = 0
";

                    }
                    else
                    {
                        storeOrderQuery = @"
select
StoreOrders.*,
Stores.Name as StoreName,
Stores.ImageUrl from StoreOrders 
join Stores on Stores.Id = StoreOrders.Store_Id
where 
Order_Id in (" + orderIds + @")
and StoreOrders.RemoveFromDelivererHistory = 0
";
                    }

                    #endregion

                    var storeOrders = ctx.Database.SqlQuery<BasketApi.AppsViewModels.StoreOrderViewModel>(storeOrderQuery).ToList();

                    if (storeOrders.Count == 0)
                    {
                        orderHistory.orders.Clear();
                        return Ok(new CustomResponse<AppsViewModels.OrdersHistoryViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = orderHistory });
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
    WHEN ISNULL(Order_Items.Box_Id, 0) <> 0 THEN Boxes.Id
  END AS ItemId,
CASE
    WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN 0
    WHEN ISNULL(Order_Items.Package_Id, 0) <> 0 THEN 1
    WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN 2
    WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN 3
    WHEN ISNULL(Order_Items.Box_Id, 0) <> 0 THEN 4
  END AS ItemType,
  Order_Items.Name AS Name,
  Order_Items.Price AS Price,
  CASE
    WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN Product_Images.Url
    WHEN ISNULL(Order_Items.Package_Id, 0) <> 0 THEN Packages.ImageUrl
    WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN Offer_Products.ImageUrl
    WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN Offer_Packages.ImageUrl
    WHEN ISNULL(Order_Items.Box_Id, 0) <> 0 THEN Boxes.IntroUrlThumbnail
  END AS ImageUrl,
  Order_Items.Id,
  Order_Items.Qty,
  ISNULL(Products.WeightInGrams,0) as WeightInGrams,
  ISNULL(Products.WeightInKiloGrams,0) as WeightInKiloGrams,
  Order_Items.StoreOrder_Id
FROM Order_Items
LEFT JOIN products
  ON products.Id = Order_Items.Product_Id
    left join Product_Images
  On Product_Images.Id = (Select top 1 Id from Product_Images where Product_Id = products.Id)
LEFT Join Boxes on Boxes.Id = Order_Items.Box_Id
LEFT JOIN Packages
  ON Packages.Id = Order_Items.Package_Id
LEFT JOIN Offer_Products
  ON Offer_Products.Id = Order_Items.Offer_Product_Id
LEFT JOIN Offer_Packages
  ON Offer_Packages.Id = Order_Items.Offer_Package_Id
  WHERE StoreOrder_Id IN (" + storeOrderIds + ")";
                    #endregion

                    var orderItems = ctx.Database.SqlQuery<BasketApi.AppsViewModels.OrderItemViewModel>(orderItemsQuery).ToList();

                    var userFavourites = ctx.Favourites.Where(x => x.User_ID == UserId && x.IsDeleted == false).ToList();

                    foreach (var orderItem in orderItems)
                    {
                        orderItem.Weight = Convert.ToString(orderItem.WeightInGrams) + " gm";

                        if (userFavourites.Any(x => x.Product_Id == orderItem.Id))
                            orderItem.IsFavourite = true;
                        else
                            orderItem.IsFavourite = false;

                        //Add product Images
                        //var productImage = ctx.ProductImages.FirstOrDefault(x => x.Product_Id == orderItem.ItemId);
                        //orderItem.ImageUrl = productImage == null ? "" : productImage.Url;
                    }

                    foreach (var orderItem in orderItems)
                    {
                        storeOrders.FirstOrDefault(x => x.Id == orderItem.StoreOrder_Id).OrderItems.Add(orderItem);
                    }

                    foreach (var storeOrder in storeOrders)
                    {
                        orderHistory.orders.FirstOrDefault(x => x.Id == storeOrder.Order_Id).StoreOrders.Add(storeOrder);
                    }

                    return Ok(new CustomResponse<AppsViewModels.OrdersHistoryViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = orderHistory });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("User")]
        [HttpGet]
        [Route("EditOrderScheduling")]
        public async Task<IHttpActionResult> EditOrderScheduling(int OrderId, string From, string To, string AdditionalNote)
        {
            try
            {
                DateTime FromDateTime;
                DateTime ToDateTime;
                DateTime.TryParse(From, out FromDateTime);
                DateTime.TryParse(To, out ToDateTime);

                if (FromDateTime == DateTime.MinValue || ToDateTime == DateTime.MinValue)
                    return Ok(new CustomResponse<Error> { Message = "BadRequest", StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "Invalid startdate or enddate" } });

                using (RiscoContext ctx = new RiscoContext())
                {
                    var order = ctx.Orders.FirstOrDefault(x => x.Id == OrderId);
                    if (order != null)
                    {
                        order.DeliveryTime_From = FromDateTime;
                        order.DeliveryTime_To = ToDateTime;
                        order.AdditionalNote = AdditionalNote;
                        ctx.SaveChanges();
                        return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                    }
                    else
                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("OrderId") } });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        //[BasketApi.Authorize("User")]
        //[HttpPost]
        //[Route("InsertOrder")]
        //public async Task<IHttpActionResult> InsertOrder(OrderViewModel model)
        //{
        //    try
        //    {
        //        Order order;

        //        if (System.Web.HttpContext.Current.Request.Params["PaymentCard"] != null)
        //            model.PaymentCard = JsonConvert.DeserializeObject<PaymentCardViewModel>(System.Web.HttpContext.Current.Request.Params["PaymentCard"]);

        //        if (System.Web.HttpContext.Current.Request.Params["Cart"] != null)
        //            model.Cart = JsonConvert.DeserializeObject<CartViewModel>(System.Web.HttpContext.Current.Request.Params["Cart"]);

        //        Validate(model);

        //        if (!ModelState.IsValid)
        //            return BadRequest(ModelState);
        //        else if (model.PaymentMethodType != (int)PaymentMethods.CashOnDelivery && (model.PaymentInfo.fort_id == "" || model.PaymentInfo.merchant_reference == ""))
        //            return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "Please provide fort_id and merchant_reference for card payments" } });

        //        if (model.Cart.CartItems.Count() > 0)
        //        {
        //            order = new Order();
        //            order.MakeOrder(model);

        //            using (RiscoContext ctx = new RiscoContext())
        //            {
        //                PayfortUtil payment = new PayfortUtil();

        //                try
        //                {
        //                    ctx.Orders.Add(order);
        //                    ctx.SaveChanges();

        //                    if (model.PaymentMethodType != (int)PaymentMethods.CashOnDelivery)
        //                    {
        //                        model.PaymentInfo.amount = Convert.ToInt32(order.Total) * 100;

        //                        if (payment.Capture(model.PaymentInfo) == "Success")
        //                            order.PaymentStatus = (int)PaymentStatuses.Completed;
        //                        else
        //                            order.PaymentStatus = (int)PaymentStatuses.Pending;

        //                        order.PaymentTransactionId = model.PaymentInfo.fort_id;
        //                        ctx.SaveChanges();
        //                    }

        //                    var productNames = String.Join(",", order.StoreOrders.First().Order_Items.Select(x => x.Name));
        //                    var adminsToPush = ctx.AdminTokens.ToList();
        //                    var orderUrl = ConfigurationManager.AppSettings["WebsiteBaseUrl"] + "Dashboard/Orders?OrderId=" + order.Id;

        //                    HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
        //                    {
        //                        Global.objPushNotifications.SendWebGCMPushNotification(adminsToPush, "New Order Received! #" + order.Id, productNames, orderUrl);
        //                    });
        //                }
        //                catch (Exception ex)
        //                {
        //                    Utility.LogError(ex);

        //                    if (model.PaymentMethodType != (int)PaymentMethods.CashOnDelivery)
        //                        payment.Void(model.PaymentInfo);
        //                }


        //                var orderModel = GetOrderById(order.Id);

        //                orderModel.PaymentMethodName = Utility.GetPaymentMethodName(orderModel.PaymentMethod);

        //                var contextBase = new HttpContextWrapper(HttpContext.Current);
        //                var routeData = new System.Web.Routing.RouteData();
        //                routeData.Values.Add("controller", "Home");
        //                var controllerContext = new System.Web.Mvc.ControllerContext(contextBase,
        //                                                              routeData,
        //                                                              new EmptyController());
        //                var razorViewEngine = new System.Web.Mvc.RazorViewEngine();
        //                var razorViewResult = razorViewEngine.FindPartialView(controllerContext,
        //                                                               "~/Views/Home/GenerateInvoiceReport.cshtml",
        //                                                               false);

        //                var writer = new StringWriter();
        //                var viewContext = new System.Web.Mvc.ViewContext(controllerContext,
        //                                                  razorViewResult.View,
        //                                                  new System.Web.Mvc.ViewDataDictionary(orderModel),
        //                                                  new System.Web.Mvc.TempDataDictionary(),
        //                                                  writer);

        //                razorViewResult.View.Render(viewContext, writer);
        //                var invoiceHtml = writer.ToString();

        //                string path = @"~/Content/bootstrap.min.css";
        //                path = HttpContext.Current.Server.MapPath(path);

        //                var pdfDirToSave = HttpContext.Current.Server.MapPath("~\\api\\ImageDirectory") + "\\" + "PDFReports" + "\\";

        //                HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
        //                {
        //                    sendInvoiceEmail(invoiceHtml, orderModel.User.Email, path, pdfDirToSave);
        //                });

        //            }
        //            return Ok(new CustomResponse<OrderSummaryViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new OrderSummaryViewModel(order) });
        //        }
        //        else
        //            return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "No items in the cart." } });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        //[BasketApi.Authorize("User", "Deliverer")]
        [HttpGet]
        [Route("GetOrderByOrderId")]
        public async Task<IHttpActionResult> GetOrderByOrderId(int OrderId)
        {
            try
            {
                var order = GetOrderById(OrderId);
                return Ok(new CustomResponse<AppsViewModels.OrderViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = order });
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        private BasketApi.AppsViewModels.OrderViewModel GetOrderById(int OrderId)
        {
            using (RiscoContext ctx = new RiscoContext())
            {
                #region OrderQuery
                var orderQuery = @"
SELECT *, Users.FullName as UserFullName FROM Orders 
join Users on Users.ID = Orders.User_ID
where Orders.Id = " + OrderId + @" and Orders.IsDeleted = 0 ";
                #endregion

                var order = ctx.Database.SqlQuery<BasketApi.AppsViewModels.OrderViewModel>(orderQuery).First();

                #region storeOrderQuery
                var storeOrderQuery = @"
select
StoreOrders.*,
Stores.Name as StoreName,
Stores.ImageUrl from StoreOrders 
join Stores on Stores.Id = StoreOrders.Store_Id
where 
Order_Id = " + order.Id + @"
";
                #endregion
                var UserQuery = @"
select Users.Id , 
Users.FirstName,
Users.LastName,
Users.Email,
Users.Phone,
Users.ProfilePictureUrl
from 
Users Where Users.Id=" + order.User_ID + "";
                var user = ctx.Database.SqlQuery<BasketApi.ViewModels.UserViewModel>(UserQuery).FirstOrDefault();


                var storeOrders = ctx.Database.SqlQuery<BasketApi.AppsViewModels.StoreOrderViewModel>(storeOrderQuery).ToList();

                var storeOrderIds = string.Join(",", storeOrders.Select(x => x.Id.ToString()));

                #region OrderItemsQuery
                var orderItemsQuery = @"
SELECT
  CASE
    WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN Products.Id
    WHEN ISNULL(Order_Items.Package_Id, 0) <> 0 THEN Packages.Id
    WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN Offer_Products.Id
    WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN Offer_Packages.Id
    WHEN ISNULL(Order_Items.Box_Id, 0) <> 0 THEN Boxes.Id
  END AS ItemId,
CASE
    WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN 0
    WHEN ISNULL(Order_Items.Package_Id, 0) <> 0 THEN 1
    WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN 2
    WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN 3
    WHEN ISNULL(Order_Items.Box_Id, 0) <> 0 THEN 4
  END AS ItemType,
  Order_Items.Name AS Name,
  Order_Items.Price AS Price,
  CASE
    WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN Product_Images.Url
    WHEN ISNULL(Order_Items.Package_Id, 0) <> 0 THEN Packages.ImageUrl
    WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN Offer_Products.ImageUrl
    WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN Offer_Packages.ImageUrl
    WHEN ISNULL(Order_Items.Box_Id, 0) <> 0 THEN Boxes.IntroUrlThumbnail
  END AS ImageUrl,
  Order_Items.Id,
  Order_Items.Qty,
  ISNULL(Products.WeightInGrams,0) as WeightInGrams,
  ISNULL(Products.WeightInKiloGrams,0) as WeightInKiloGrams,
  Order_Items.StoreOrder_Id
FROM Order_Items
LEFT JOIN products
  ON products.Id = Order_Items.Product_Id
    left join Product_Images
  On Product_Images.Id = (Select top 1 Id from Product_Images where Product_Id = products.Id)
LEFT Join Boxes on Boxes.Id = Order_Items.Box_Id
LEFT JOIN Packages
  ON Packages.Id = Order_Items.Package_Id
LEFT JOIN Offer_Products
  ON Offer_Products.Id = Order_Items.Offer_Product_Id
LEFT JOIN Offer_Packages
  ON Offer_Packages.Id = Order_Items.Offer_Package_Id
  WHERE StoreOrder_Id IN (" + storeOrderIds + ")";
                #endregion

                var orderItems = ctx.Database.SqlQuery<BasketApi.AppsViewModels.OrderItemViewModel>(orderItemsQuery).ToList();

                //var userFavourites = ctx.Favourites.Where(x => x.User_ID == UserId && x.IsDeleted == false).ToList();

                //foreach (var orderItem in orderItems)
                //{
                //    orderItem.Weight = Convert.ToString(orderItem.WeightInGrams) + " gm";

                //    if (userFavourites.Any(x => x.Product_Id == orderItem.Id))
                //        orderItem.IsFavourite = true;
                //    else
                //        orderItem.IsFavourite = false;
                //}

                foreach (var orderItem in orderItems)
                {
                    storeOrders.FirstOrDefault(x => x.Id == orderItem.StoreOrder_Id).OrderItems.Add(orderItem);
                }

                foreach (var storeOrder in storeOrders)
                {
                    order.StoreOrders.Add(storeOrder);
                }
                order.User = user;
                return order;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("EmailInvoice")]
        public async Task<IHttpActionResult> EmailInvoice()
        {
            try
            {
                string mimeType;
                string encoding;
                string filenameExtension;
                string[] streamids;
                Warning[] warnings;

                var reportPath = HttpContext.Current.Server.MapPath("~/" + "Reports/" + "Invoice.rdlc");

                Microsoft.Reporting.WinForms.ReportViewer invoice = new Microsoft.Reporting.WinForms.ReportViewer();
                invoice.LocalReport.ReportPath = reportPath;

                List<Order> Orders = new List<Order>();
                List<StoreOrder> StoreOrders = new List<StoreOrder>();
                List<Order_Items> OrderItems = new List<Order_Items>();
                List<User> Users = new List<User>();

                using (RiscoContext ctx = new RiscoContext())
                {
                    var order = ctx.Orders.Include(x => x.User).Include(x => x.StoreOrders.Select(x1 => x1.Order_Items)).FirstOrDefault(x => x.Id == 2275);
                    Orders.Add(order);
                    StoreOrders.Add(order.StoreOrders.First());
                    OrderItems.AddRange(order.StoreOrders.First().Order_Items);
                    Users.Add(order.User);
                }

                invoice.LocalReport.DataSources.Add(new ReportDataSource("Orders", Orders));
                invoice.LocalReport.DataSources.Add(new ReportDataSource("StoreOrders", StoreOrders));
                invoice.LocalReport.DataSources.Add(new ReportDataSource("OrderItems", OrderItems));
                invoice.LocalReport.DataSources.Add(new ReportDataSource("Users", Users));

                byte[] bytes = invoice.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

                var path = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["InvoicesImageFolderPath"] + "invoice.pdf");
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                string subject = "Reset your password - " + EmailUtil.FromName;
                const string body = "Use this code to reset your password";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(EmailUtil.FromMailAddress.Address, EmailUtil.FromPassword)
                };

                var message = new MailMessage(EmailUtil.FromMailAddress, new MailAddress("engr.mmohsin@gmail.com"))
                {
                    Subject = subject,
                    Body = body + " Invoice Attached"
                };

                smtp.Send(message);

                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        //[AllowAnonymous]
        //[HttpGet]
        //[Route("sendInvoiceEmail")]
        [NonAction]
        public async Task sendInvoiceEmail(string invoiceHtml, string userEmail, string path, string pdfPathToSave)
        {
            try
            {
                //var order = GetOrderById(orderId);

                //order.PaymentMethodName = Utility.GetPaymentMethodName(order.PaymentMethod);

                //string invoiceHtml = MvcControllerCustom.RenderViewToString("Home", "~/Views/Home/GenerateInvoiceReport.cshtml", order, contextBase);

                //string path = @"~/Content/bootstrap.min.css";
                //path = HttpContext.Current.Server.MapPath(path);
                if (File.Exists(path))
                {
                    string readText = File.ReadAllText(path);
                    if (!string.IsNullOrWhiteSpace(readText))
                    {
                        invoiceHtml = invoiceHtml.Replace(".testClass {}", readText);
                    }
                }
                NReco.PdfGenerator.HtmlToPdfConverter pdfGenerator = new NReco.PdfGenerator.HtmlToPdfConverter();
                byte[] pdfDocument = pdfGenerator.GeneratePdf(invoiceHtml);

                var url = SaveFile.SaveFileFromBytes(pdfDocument, "PDFReports", pdfPathToSave);

                string subject = "New Order Has been placed - " + EmailUtil.FromName;
                const string body = "Please find attached Invoice file";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(EmailUtil.FromMailAddress.Address, EmailUtil.FromPassword)
                };

                var message = new MailMessage(EmailUtil.FromMailAddress, new MailAddress(userEmail))
                {
                    Subject = subject,
                    Body = body + " Invoice Attached"
                };
                var AdminEmail = BasketSettings.GetAdminEmailForOrders();
                if (AdminEmail != null)
                {
                    message.To.Add(new MailAddress(AdminEmail));
                }
                Attachment attachment = new Attachment(url);
                message.Attachments.Add(attachment);
                smtp.Send(message);


            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        //[BasketApi.Authorize("User")]
        //[HttpGet]
        //[Route("PayfortSDKToken")]
        //public async Task<IHttpActionResult> PayfortSDKToken(string device_id)
        //{
        //    PayfortUtil payfort = new PayfortUtil();
        //    return Ok(new CustomResponse<SDKTokenResponseModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = payfort.SdkToken(device_id) });
        //}

        [HttpPost]
        [Route("PayfortCapture")]
        public async Task<IHttpActionResult> PayfortCapture(CaptureResponseModel model)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + "/PaymentErrorLog.txt"))
                {
                    sw.WriteLine("DateTime : " + DateTime.Now + Environment.NewLine);
                    sw.WriteLine(Environment.NewLine + "Payfort Capture called by payfort");
                    sw.WriteLine(Environment.NewLine + "command: " + model.command);
                    sw.WriteLine(Environment.NewLine + "access_code: " + model.access_code);
                    sw.WriteLine(Environment.NewLine + "amount: " + model.amount);
                    sw.WriteLine(Environment.NewLine + "currency: " + model.currency);
                    sw.WriteLine(Environment.NewLine + "fort_id: " + model.fort_id);
                    sw.WriteLine(Environment.NewLine + "language: " + model.language);
                    sw.WriteLine(Environment.NewLine + "merchant_identifier: " + model.merchant_identifier);
                    sw.WriteLine(Environment.NewLine + "merchant_reference: " + model.merchant_reference);
                    sw.WriteLine(Environment.NewLine + "order_description: " + model.order_description);
                    sw.WriteLine(Environment.NewLine + "response_code: " + model.response_code);
                    sw.WriteLine(Environment.NewLine + "response_message: " + model.response_message);
                    sw.WriteLine(Environment.NewLine + "signature: " + model.signature);
                    sw.WriteLine(Environment.NewLine + "status: " + model.status);
                    sw.WriteLine(Environment.NewLine + "*****End Of Transaction*****");
                }

                using (RiscoContext ctx = new RiscoContext())
                {
                    var order = ctx.Orders.FirstOrDefault(x => x.PaymentTransactionId == x.PaymentTransactionId);
                    if (order != null)
                    {
                        if (model.status == 04)
                            order.PaymentStatus = (int)PaymentStatuses.Completed;
                        else
                            order.PaymentStatus = (int)PaymentStatuses.Pending;

                        order.PaymentTransactionId = model.fort_id;

                        ctx.SaveChanges();
                    }
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

    }
}
