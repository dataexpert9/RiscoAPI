//using BasketApi.BindingModels;
//using BasketApi.ViewModels;
//using DAL;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;
//using System.Web.Http;
//using static BasketApi.Global;
//using System.Data.Entity;
//using System.Globalization;
//using BasketApi.AdminViewModel;
//using static BasketApi.Utility;
////using WebApplication1.Payment;
//using System.Web;
//using System.IO;
//using System.Web.Hosting;
//using WebApplication1.PDFConverter;
//using System.Net.Mail;

//namespace BasketApi.Controllers
//{

//	[RoutePrefix("api/Subscriptions")]
//	public class SubscriptionsController : ApiController
//	{
//		[BasketApi.Authorize("User")]
//		[HttpPost]
//		[Route("SubscribeVideo")]
//		public async Task<IHttpActionResult> SubscribeVideo(SubscribeVideoBindingModel model)
//		{
//			try
//			{
//				if (!ModelState.IsValid)
//				{
//					return BadRequest(ModelState);
//				}
//				else if (model.PaymentMethodType != (int)PaymentMethods.CashOnDelivery && (model.PaymentInfo.fort_id == "" || model.PaymentInfo.merchant_reference == ""))
//				{
//					return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "Please provide fort_id and merchant_reference for card payments" } });
//				}

//				var userId = Convert.ToInt32(User.GetClaimValue("userid"));

//				using (RiscoContext ctx = new RiscoContext())
//				{
//					var box = ctx.Boxes.FirstOrDefault(x => x.Id == model.Box_Id && x.IsDeleted == false);

//					#region Set Box Expiry Date
//					if (box != null)
//					{
//						switch (model.Type)
//						{
//							case (int)BoxCategoryOptions.Junior:
//								model.ExpiryDate = model.Month.AddMonths(1);
//								break;
//							case (int)BoxCategoryOptions.Monthly:
//								model.ExpiryDate = model.Month.AddMonths(1);
//								break;
//							case (int)BoxCategoryOptions.ProBox:
//								model.ExpiryDate = model.Month.AddMonths(1);
//								break;
//							case (int)BoxCategoryOptions.HallOfFame:
//								model.ExpiryDate = model.Month.AddMonths(1);
//								break;
//						}
//						#endregion

//						TimeZoneInfo UAETimeZone1 = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");
//						DateTime utc1 = DateTime.UtcNow;
//						DateTime UAE1 = TimeZoneInfo.ConvertTimeFromUtc(utc1, UAETimeZone1);
//						UserSubscriptions subscription = new UserSubscriptions { CreatedDate = UAE1, Type = box.BoxCategory_Id, Box_Id = model.Box_Id, User_Id = model.User_Id, SubscriptionDate = model.Month, ExpiryDate = model.ExpiryDate };

//						await subscription.GetRandomActivationCode();

//						ctx.UserSubscriptions.Add(subscription);
//						ctx.SaveChanges();

//						#region Save Order
//						//PayfortUtil payment = new PayfortUtil();

//						Order order = new Order();
//						order.DeliveryTime_From = DateTime.UtcNow.AddDays(7);
//						order.DeliveryTime_To = DateTime.UtcNow.AddDays(7);
//						TimeZoneInfo UAETimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time"); DateTime utc = DateTime.UtcNow;
//						DateTime UAE = TimeZoneInfo.ConvertTimeFromUtc(utc, UAETimeZone);
//						order.OrderDateTime = UAE;
//						order.User_ID = userId;
//						StoreOrder storeOrder = new StoreOrder();
//						storeOrder.Store_Id = ctx.Stores.FirstOrDefault(x => x.IsDeleted == false).Id;
//						storeOrder.OrderNo = Guid.NewGuid().ToString("N").ToUpper();
//						Order_Items orderItem = new Order_Items();
//						orderItem.Box_Id = model.Box_Id;
//						orderItem.Name = box.Name;
//						orderItem.Price = box.Price;
//						orderItem.Description = box.Description;
//						orderItem.Qty = 1;
//						storeOrder.Order_Items.Add(orderItem);
//						order.StoreOrders.Add(storeOrder);
//						order.OrderNo = Guid.NewGuid().ToString("N").ToUpper();
//						order.Status = (int)OrderStatuses.Initiated;
//						order.PaymentMethod = model.PaymentMethodType;
//						order.DeliveryAddress = model.DeliveryAddress;
//						order.AdditionalNote = model.AdditionalNote;
//						//order.Tax = BasketSettings.Tax;
//						order.Subtotal = orderItem.Price;
//						order.DeliveryFee = order.Subtotal >= BasketSettings.Settings.FreeDeliveryThreshold ? 0 : BasketSettings.Settings.DeliveryFee;
//						//order.Tax = (order.Subtotal / 100) * BasketSettings.Tax; //VAT in %
//						order.Total = order.Subtotal + order.Tax + order.DeliveryFee;


//						try
//						{
//							ctx.Orders.Add(order);
//							ctx.SaveChanges();

//							if (model.PaymentMethodType != (int)PaymentMethods.CashOnDelivery)
//							{
//								model.PaymentInfo.amount = Convert.ToInt32(order.Total) * 100;

//								if (payment.Capture(model.PaymentInfo) == "Success")
//									order.PaymentStatus = (int)PaymentStatuses.Completed;
//								else
//									order.PaymentStatus = (int)PaymentStatuses.Pending;

//								order.PaymentTransactionId = model.PaymentInfo.fort_id;
//								ctx.SaveChanges();
//							}
//						}
//						catch (Exception ex)
//						{
//							Utility.LogError(ex);

//							if (model.PaymentMethodType != (int)PaymentMethods.CashOnDelivery)
//								payment.Void(model.PaymentInfo);
//						}
//						#endregion

//						var orderModel = GetOrderById(order.Id);

//						orderModel.PaymentMethodName = Utility.GetPaymentMethodName(orderModel.PaymentMethod);

//						var contextBase = new HttpContextWrapper(HttpContext.Current);
//						var routeData = new System.Web.Routing.RouteData();
//						routeData.Values.Add("controller", "Home");
//						var controllerContext = new System.Web.Mvc.ControllerContext(contextBase,
//																	  routeData,
//																	  new EmptyController());
//						var razorViewEngine = new System.Web.Mvc.RazorViewEngine();
//						var razorViewResult = razorViewEngine.FindPartialView(controllerContext,
//																	   "~/Views/Home/GenerateInvoiceReport.cshtml",
//																	   false);

//						var writer = new StringWriter();
//						var viewContext = new System.Web.Mvc.ViewContext(controllerContext,
//														  razorViewResult.View,
//														  new System.Web.Mvc.ViewDataDictionary(orderModel),
//														  new System.Web.Mvc.TempDataDictionary(),
//														  writer);

//						razorViewResult.View.Render(viewContext, writer);
//						var invoiceHtml = writer.ToString();

//						string path = @"~/Content/bootstrap.min.css";
//						path = HttpContext.Current.Server.MapPath(path);

//						var pdfDirToSave = HttpContext.Current.Server.MapPath("~\\api\\ImageDirectory") + "\\" + "PDFReports" + "\\";

//						HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
//						{
//							sendInvoiceEmail(invoiceHtml, orderModel.User.Email, path, pdfDirToSave);
//						});

//						return Ok(new CustomResponse<OrderSummaryViewModel>
//						{
//							Message = Global.ResponseMessages.Success,
//							StatusCode = (int)HttpStatusCode.OK,
//							Result = new OrderSummaryViewModel(order)
//						});
//					}
//					else
//						return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "Invalid Box_Id" } });

//				}
//			}
//			catch (Exception ex)
//			{
//				return StatusCode(Utility.LogError(ex));
//			}
//		}

//		[BasketApi.Authorize("User")]
//		[HttpGet]
//		[Route("MySkriblBox")]
//		public async Task<IHttpActionResult> MySkriblBox(int UserId)
//		{
//			try
//			{
//				using (RiscoContext ctx = new RiscoContext())
//				{
//					return Ok(new CustomResponse<MySkriblBoxViewModel>
//					{
//						Message = Global.ResponseMessages.Success,
//						StatusCode = (int)HttpStatusCode.OK,
//						Result = new MySkriblBoxViewModel { Subscriptions = ctx.UserSubscriptions.Include(x => x.Box.BoxVideos).Where(x => x.User_Id == UserId && x.IsDeleted == false).ToList() }
//					});
//				}
//			}
//			catch (Exception ex)
//			{
//				return StatusCode(Utility.LogError(ex));
//			}
//		}

//		[BasketApi.Authorize("User")]
//		[HttpGet]
//		[Route("GetBox")]
//		public async Task<IHttpActionResult> GetBox(int Type, string month, int User_Id)
//		{
//			try
//			{
//				DateTime monthDateTime;
//				DateTime.TryParse(month, out monthDateTime);

//				using (RiscoContext ctx = new RiscoContext())
//				{
//					var existingUserSubscription = ctx.UserSubscriptions.Include(x => x.Box).FirstOrDefault(x => x.User_Id == User_Id && x.Type == Type && x.SubscriptionDate.Month == monthDateTime.Month && x.SubscriptionDate.Year == monthDateTime.Year && x.IsDeleted == false);
//					if (existingUserSubscription != null)
//					{
//						existingUserSubscription.Box.AlreadySubscribed = true;
//						return Ok(new CustomResponse<Box>
//						{
//							Message = Global.ResponseMessages.Success,
//							StatusCode = (int)HttpStatusCode.OK,
//							Result = existingUserSubscription.Box
//						});
//					}
//					else
//					{
//						return Ok(new CustomResponse<Box>
//						{
//							Message = Global.ResponseMessages.Success,
//							StatusCode = (int)HttpStatusCode.OK,
//							Result = ctx.Boxes.FirstOrDefault(x => x.BoxCategory_Id == Type && x.ReleaseDate.Month == monthDateTime.Month && x.ReleaseDate.Year == monthDateTime.Year && x.IsDeleted == false)
//						});
//					}
//				}
//			}
//			catch (Exception ex)
//			{
//				return StatusCode(Utility.LogError(ex));
//			}
//		}

//		[BasketApi.Authorize("User")]
//		[HttpGet]
//		[Route("MySubsciptions")]
//		public async Task<IHttpActionResult> MySubsciptions(int UserId)
//		{
//			try
//			{
//				using (RiscoContext ctx = new RiscoContext())
//				{
//					return Ok(new CustomResponse<MySkriblBoxViewModel>
//					{
//						Message = Global.ResponseMessages.Success,
//						StatusCode = (int)HttpStatusCode.OK,
//						Result = new MySkriblBoxViewModel
//						{
//							Subscriptions = ctx.UserSubscriptions.Include(x => x.Box).Where(x => x.User_Id == UserId && x.IsDeleted == false).ToList()
//						}
//					});
//				}
//			}
//			catch (Exception ex)
//			{
//				return StatusCode(Utility.LogError(ex));
//			}
//		}

//		[BasketApi.Authorize("User", "SubAdmin", "SuperAdmin", "ApplicationAdmin")]
//		[HttpGet]
//		[Route("SearchSubscriptions")]
//		public async Task<IHttpActionResult> SearchSubscriptions(int? SubscriptionId, int? BoxId, int? UserId)
//		{
//			try
//			{
//				using (RiscoContext ctx = new RiscoContext())
//				{
//					SubscriptionListViewModel returnModel = new SubscriptionListViewModel();
//					var query = @"select 
//								UserSubscriptions.Id,
//								UserSubscriptions.User_Id,
//								UserSubscriptions.SubscriptionDate,
//								UserSubscriptions.ExpiryDate,
//								UserSubscriptions.Box_Id,
//								UserSubscriptions.Type,
//								UserSubscriptions.Status,
//								UserSubscriptions.ActivationCode,
//								Boxes.Name,
//								Boxes.BoxCategory_Id,
//								Boxes.Price,
//								Users.FullName,
//								Users.ProfilePictureUrl,
//								Users.Email,
//								Users.Phone
//								from UserSubscriptions
//								join Boxes on Boxes.Id = UserSubscriptions.Box_Id
//								join Users on Users.ID = UserSubscriptions.User_Id  ";

//					if (BoxId.HasValue && UserId.HasValue)
//					{
//						query += @"Where UserSubscriptions.User_Id=" + UserId + " AND UserSubscriptions.Box_Id=" + BoxId + "";
//					}
//					else if (BoxId.HasValue)
//					{
//						query += @"Where UserSubscriptions.Box_Id=" + BoxId + "";
//					}
//					else if (UserId.HasValue)
//					{
//						query += @"Where UserSubscriptions.User_Id=" + UserId + "";
//					}
//					else if (SubscriptionId.HasValue)
//					{
//						query += @" Where UserSubscriptions.Id=" + SubscriptionId + "";
//						returnModel.is_detail = true;

//					}
//					else
//					{

//					}
//					query += @" AND  Boxes.IsDeleted=0";
//					returnModel.Subscriptions = ctx.Database.SqlQuery<AdminSubscriptionViewModel>(query).ToList();

//					// dont need to exe for each of no data found and to prevent any other kind of exception
//					if (returnModel.Subscriptions != null)
//					{
//						foreach (var subscription in returnModel.Subscriptions)
//						{
//							subscription.BoxCategoryName = Utility.GetBoxCategoryName(subscription.BoxCategory_Id);
//						}
//					}

//					return Ok(new CustomResponse<SubscriptionListViewModel>
//					{
//						Message = Global.ResponseMessages.Success,
//						StatusCode = (int)HttpStatusCode.OK,
//						Result = returnModel
//					});
//				}
//			}
//			catch (Exception ex)
//			{
//				return StatusCode(Utility.LogError(ex));
//			}
//		}

//		[BasketApi.Authorize("User")]
//		[HttpGet]
//		[Route("ActivateBox")]
//		public async Task<IHttpActionResult> ActivateBox(int UserId, int SubscriptionId, string ActivationCode)
//		{
//			try
//			{
//				using (RiscoContext ctx = new RiscoContext())
//				{
//					var userSubscription = ctx.UserSubscriptions.FirstOrDefault(x => x.Id == SubscriptionId && x.User_Id == UserId && x.ActivationCode == ActivationCode && x.IsDeleted == false);
//					if (userSubscription != null)
//					{
//						userSubscription.Status = (int)SubscriptionStatus.Active;
//						ctx.SaveChanges();
//						return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
//					}
//					else
//					{
//						return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "Invalid Subscription" } });
//					}
//				}
//			}
//			catch (Exception ex)
//			{
//				return StatusCode(Utility.LogError(ex));
//			}
//		}

//		[BasketApi.Authorize("User")]
//		[HttpGet]
//		[Route("GetBoxesByType")]
//		public async Task<IHttpActionResult> GetBoxesByType(int Type, int UserId)
//		{
//			try
//			{
//				using (RiscoContext ctx = new RiscoContext())
//				{
//					List<Box> boxes = new List<Box>();
//					if (Type == (int)BoxCategoryOptions.HallOfFame)
//					{
//						var latestMonthlyBoxId = ctx.Boxes.Where(x => x.BoxCategory_Id == (int)BoxCategoryOptions.Monthly && x.IsDeleted == false).Max(x => x.Id);
//						boxes = ctx.Boxes.Where(x => x.IsDeleted == false && x.Id < latestMonthlyBoxId && x.BoxCategory_Id == (int)BoxCategoryOptions.Monthly).ToList();
//					}
//					else if (Type == (int)BoxCategoryOptions.Monthly)
//					{
//						var monthlyBox = ctx.Boxes.Where(x => x.IsDeleted == false && x.BoxCategory_Id == (int)BoxCategoryOptions.Monthly).OrderByDescending(x => x.Id).FirstOrDefault();
//						if (monthlyBox != null)
//						{
//							boxes.Add(monthlyBox);
//						}
//					}
//					else
//						boxes = ctx.Boxes.Where(x => x.IsDeleted == false && x.BoxCategory_Id == Type).ToList();

//					var userSubscriptions = ctx.UserSubscriptions.Where(x => x.User_Id == UserId && x.IsDeleted == false).ToList();

//					if (userSubscriptions != null && userSubscriptions.Count > 0)
//					{
//						foreach (var box in boxes)
//						{
//							box.AlreadySubscribed = userSubscriptions.Any(x => x.Box_Id == box.Id);
//						}
//					}

//					return Ok(new CustomResponse<SearchBoxesViewModel>
//					{
//						Message = Global.ResponseMessages.Success,
//						StatusCode = (int)HttpStatusCode.OK,
//						Result = new SearchBoxesViewModel { Boxes = boxes }
//					});
//				}
//			}
//			catch (Exception ex)
//			{
//				return StatusCode(Utility.LogError(ex));
//			}
//		}

//		private BasketApi.AppsViewModels.OrderViewModel GetOrderById(int OrderId)
//		{
//			using (RiscoContext ctx = new RiscoContext())
//			{
//				#region OrderQuery
//				var orderQuery = @"
//SELECT *, Users.FullName as UserFullName FROM Orders 
//join Users on Users.ID = Orders.User_ID
//where Orders.Id = " + OrderId + @" and Orders.IsDeleted = 0 ";
//				#endregion

//				var order = ctx.Database.SqlQuery<BasketApi.AppsViewModels.OrderViewModel>(orderQuery).First();

//				#region storeOrderQuery
//				var storeOrderQuery = @"
//select
//StoreOrders.*,
//Stores.Name as StoreName,
//Stores.ImageUrl from StoreOrders 
//join Stores on Stores.Id = StoreOrders.Store_Id
//where 
//Order_Id = " + order.Id + @"
//";
//				#endregion
//				var UserQuery = @"
//select Users.Id , 
//Users.FirstName,
//Users.LastName,
//Users.Email,
//Users.Phone,
//Users.ProfilePictureUrl
//from 
//Users Where Users.Id=" + order.User_ID + "";
//				var user = ctx.Database.SqlQuery<BasketApi.ViewModels.UserViewModel>(UserQuery).FirstOrDefault();


//				var storeOrders = ctx.Database.SqlQuery<BasketApi.AppsViewModels.StoreOrderViewModel>(storeOrderQuery).ToList();

//				var storeOrderIds = string.Join(",", storeOrders.Select(x => x.Id.ToString()));

//				#region OrderItemsQuery
//				var orderItemsQuery = @"
//SELECT
//  CASE
//	WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN Products.Id
//	WHEN ISNULL(Order_Items.Package_Id, 0) <> 0 THEN Packages.Id
//	WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN Offer_Products.Id
//	WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN Offer_Packages.Id
//	WHEN ISNULL(Order_Items.Box_Id, 0) <> 0 THEN Boxes.Id
//  END AS ItemId,
//CASE
//	WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN 0
//	WHEN ISNULL(Order_Items.Package_Id, 0) <> 0 THEN 1
//	WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN 2
//	WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN 3
//	WHEN ISNULL(Order_Items.Box_Id, 0) <> 0 THEN 4
//  END AS ItemType,
//  Order_Items.Name AS Name,
//  Order_Items.Price AS Price,
//  CASE
//	WHEN ISNULL(Order_Items.Product_Id, 0) <> 0 THEN Products.ImageUrl
//	WHEN ISNULL(Order_Items.Package_Id, 0) <> 0 THEN Packages.ImageUrl
//	WHEN ISNULL(Order_Items.Offer_Product_Id, 0) <> 0 THEN Offer_Products.ImageUrl
//	WHEN ISNULL(Order_Items.Offer_Package_Id, 0) <> 0 THEN Offer_Packages.ImageUrl
//	WHEN ISNULL(Order_Items.Box_Id, 0) <> 0 THEN Boxes.IntroUrlThumbnail
//  END AS ImageUrl,
//  Order_Items.Id,
//  Order_Items.Qty,
//  ISNULL(Products.WeightInGrams,0) as WeightInGrams,
//  ISNULL(Products.WeightInKiloGrams,0) as WeightInKiloGrams,
//  Order_Items.StoreOrder_Id
//FROM Order_Items
//LEFT JOIN products
//  ON products.Id = Order_Items.Product_Id
//LEFT JOIN Packages
//  ON Packages.Id = Order_Items.Package_Id
//LEFT JOIN Offer_Products
//  ON Offer_Products.Id = Order_Items.Offer_Product_Id
//LEFT JOIN Offer_Packages
//  ON Offer_Packages.Id = Order_Items.Offer_Package_Id
//LEFT JOIN Boxes
//  ON Boxes.Id = Order_Items.Box_Id
//WHERE StoreOrder_Id IN (" + storeOrderIds + ")";
//				#endregion

//				var orderItems = ctx.Database.SqlQuery<BasketApi.AppsViewModels.OrderItemViewModel>(orderItemsQuery).ToList();

//				//var userFavourites = ctx.Favourites.Where(x => x.User_ID == UserId && x.IsDeleted == false).ToList();

//				//foreach (var orderItem in orderItems)
//				//{
//				//    orderItem.Weight = Convert.ToString(orderItem.WeightInGrams) + " gm";

//				//    if (userFavourites.Any(x => x.Product_Id == orderItem.Id))
//				//        orderItem.IsFavourite = true;
//				//    else
//				//        orderItem.IsFavourite = false;
//				//}

//				foreach (var orderItem in orderItems)
//				{
//					storeOrders.FirstOrDefault(x => x.Id == orderItem.StoreOrder_Id).OrderItems.Add(orderItem);
//				}

//				foreach (var storeOrder in storeOrders)
//				{
//					order.StoreOrders.Add(storeOrder);
//				}
//				order.User = user;
//				return order;
//			}
//		}

//		[NonAction]
//		public async Task sendInvoiceEmail(string invoiceHtml, string userEmail, string path, string pdfPathToSave)
//		{
//			try
//			{
//				if (File.Exists(path))
//				{
//					string readText = File.ReadAllText(path);
//					if (!string.IsNullOrWhiteSpace(readText))
//					{
//						invoiceHtml = invoiceHtml.Replace(".testClass {}", readText);
//					}
//				}
//				NReco.PdfGenerator.HtmlToPdfConverter pdfGenerator = new NReco.PdfGenerator.HtmlToPdfConverter();
//				byte[] pdfDocument = pdfGenerator.GeneratePdf(invoiceHtml);

//				var url = SaveFile.SaveFileFromBytes(pdfDocument, "PDFReports", pdfPathToSave);

//				string subject = "New Order Has been placed - " + EmailUtil.FromName;
//				const string body = "Please find attached Invoice file";

//				var smtp = new SmtpClient
//				{
//					Host = "smtp.gmail.com",
//					Port = 587,
//					EnableSsl = true,
//					DeliveryMethod = SmtpDeliveryMethod.Network,
//					UseDefaultCredentials = false,
//					Credentials = new NetworkCredential(EmailUtil.FromMailAddress.Address, EmailUtil.FromPassword)
//				};

//				var message = new MailMessage(EmailUtil.FromMailAddress, new MailAddress(userEmail))
//				{
//					Subject = subject,
//					Body = body + " Invoice Attached"
//				};
//				var AdminEmail = BasketSettings.GetAdminEmailForOrders();
//				if (AdminEmail != null)
//				{
//					message.To.Add(new MailAddress(AdminEmail));
//				}
//				Attachment attachment = new Attachment(url);
//				message.Attachments.Add(attachment);
//				smtp.Send(message);
//			}
//			catch (Exception ex)
//			{
//				Utility.LogError(ex);
//			}
//		}

//	}
//}
