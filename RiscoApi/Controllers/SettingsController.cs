using BasketApi;
using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebApplication1.BindingModels;
using System.Data.Entity;
using System.Web.Http.Cors;

namespace WebApplication1.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/Settings")]
    public class SettingsController : ApiController
    {

        [AllowAnonymous]
        [HttpGet]
        [Route("GetSettings")]
        public async Task<IHttpActionResult> GetSettings()
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var settings = ctx.Settings.FirstOrDefault();
                    if (settings == null)
                    {
                        return Ok(new CustomResponse<Settings>
                        {
                            Message = Global.ResponseMessages.Success,
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = new Settings
                            {
                                DeliveryFee = 0,
                                AboutUs = "",
                                Currency = "",
                                FreeDeliveryThreshold = 0,
                                AdminEmailForOrders = "",
                                PrivacyPolicy = "",
                                TermsConditions = "",
                                FAQs = "",
                                //HowItWorksDescription = "",
                                //HowItWorksUrl = "",
                                //HowItWorksUrlThumbnail = "",
                                //BannerImages = new List<Banner_Images>()
                            }
                        });
                    }
                    else
                    {
                        //settings.BannerImages = ctx.BannerImages.Include(x => x.Product).ToList();

                        return Ok(new CustomResponse<Settings>
                        {
                            Message = Global.ResponseMessages.Success,
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = settings
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("SetSettingsProduct")]
        public async Task<IHttpActionResult> SetSettingsProduct(SettingsProductBindingModel model)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var BannerImage = ctx.BannerImages.FirstOrDefault(x => x.Id == model.BannerImage_Id);
                    if (BannerImage == null)
                    {
                        return Ok(new CustomResponse<Settings>
                        {
                            Message = Global.ResponseMessages.Success,
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = new Settings
                            {
                                DeliveryFee = 0,
                                AboutUs = "",
                                Currency = "",
                                FreeDeliveryThreshold = 0,
                                //HowItWorksDescription = "",
                                //HowItWorksUrl = "",
                                //HowItWorksUrlThumbnail = "",
                                //BannerImages = new List<Banner_Images>()
                            }
                        });
                    }
                    else
                    {
                        if (model.Product_Id > 0)
                            BannerImage.Product_Id = model.Product_Id;
                        else
                            BannerImage.Product_Id = null;
                        ctx.SaveChanges();
                        return Ok(new CustomResponse<Banner_Images>
                        {
                            Message = Global.ResponseMessages.Success,
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = BannerImage
                        });
                    }
                    //return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }



        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [HttpPost]
        [Route("SetSettings")]
        public async Task<IHttpActionResult> SetSettings()
        {
            try
            {
                Settings model = new Settings();
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;



                if (httpRequest.Params["Id"] != null)
                    model.Id = Convert.ToInt32(httpRequest.Params["Id"]);

                //if (httpRequest.Params["Tax"] != null)
                //    model.Tax = Convert.ToInt32(httpRequest.Params["Tax"]);

                if (httpRequest.Params["Currency"] != null)
                    model.Currency = httpRequest.Params["Currency"];

                if (httpRequest.Params["DeliveryFee"] != null)
                    model.DeliveryFee = Convert.ToDouble(httpRequest.Params["DeliveryFee"]);

                //if (httpRequest.Params["ContactNo"] != null)
                //    model.ContactNo = httpRequest.Params["ContactNo"];

                //if (httpRequest.Params["ContactNo"] != null)
                //    model.InstagramImage = httpRequest.Params["InstagramImage"];

                if (httpRequest.Params["FreeDeliveryThreshold"] != null)
                    model.FreeDeliveryThreshold = Convert.ToDouble(httpRequest.Params["FreeDeliveryThreshold"]);
                if (httpRequest.Params["BannerImages"] != null)
                    //model.BannerImages = JsonConvert.DeserializeObject<List<Banner_Images>>(httpRequest.Params["BannerImages"]);

                Validate(model);

                #region Validations

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (httpRequest.Files.Count > 0)
                {
                    if (!Request.Content.IsMimeMultipartContent())
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "UnsupportedMediaType",
                            StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                            Result = new Error { ErrorMessage = "Multipart data is not included in request" }
                        });
                    }
                }
                #endregion


                //bool BannerImageChanged = false;

                using (RiscoContext ctx = new RiscoContext())
                {
                    Settings returnModel = new Settings();
                    returnModel = ctx.Settings.FirstOrDefault();

                    string fileExtension = string.Empty;
                    HttpPostedFile postedFile;
                    //List<Banner_Images> bannerImages = model.BannerImages;


                    #region ImageSaving
                    //foreach (var file in httpRequest.Files)
                    //{
                    //    postedFile = httpRequest.Files[file.ToString()];
                    //    if (postedFile != null && postedFile.ContentLength > 0)
                    //    {
                    //        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                    //        var ext = Path.GetExtension(postedFile.FileName);
                    //        fileExtension = ext.ToLower();
                    //        if (!AllowedFileExtensions.Contains(fileExtension))
                    //        {
                    //            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    //            {
                    //                Message = "UnsupportedMediaType",
                    //                StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                    //                Result = new Error { ErrorMessage = "Please Upload image of type .jpg,.gif,.png" }
                    //            });
                    //        }
                    //        else if (postedFile.ContentLength > Global.MaximumImageSize)
                    //        {
                    //            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    //            {
                    //                Message = "UnsupportedMediaType",
                    //                StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                    //                Result = new Error { ErrorMessage = "Please Upload a file upto " + Global.ImageSize }
                    //            });
                    //        }
                    //        else
                    //        {

                    //        }
                    //    }
                    //    var guid = Guid.NewGuid();
                    //    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["BannerImageFolerPath"] + model.Id + "_" + guid + fileExtension);
                    //    postedFile.SaveAs(newFullPath);

                    //    if (Path.GetFileNameWithoutExtension(postedFile.FileName) == "Instagram")
                    //    {
                    //        model.InstagramImage = ConfigurationManager.AppSettings["BannerImageFolerPath"] + model.Id + "_" + guid + fileExtension;
                    //    }
                    //    //else if (Path.GetFileNameWithoutExtension(postedFile.FileName).Contains("Banner"))
                    //    //{
                    //    //    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["BannerImageFolerPath"] + "SettingsBanner_" + guid + fileExtension);
                    //    //    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["BannerImageFolerPath"]));
                    //    //    postedFile.SaveAs(newFullPath);
                    //    //    bannerImages.Add(new Banner_Images { Url = ConfigurationManager.AppSettings["BannerImageFolerPath"] + "SettingsBanner_" + guid + fileExtension });
                    //    //    BannerImageChanged = true;
                    //    //}
                    //}
                    #endregion

                    if (returnModel != null)
                    {

                        ctx.Database.ExecuteSqlCommand("TRUNCATE TABLE [Banner_Images] ");
                        //if (model.BannerImages != null && model.BannerImages.Count > 0)
                        //{
                        //    foreach (var item in bannerImages)
                        //    {
                        //        if (item.Product_Id == 0 )
                        //        {
                        //            item.Product_Id = null;
                        //        }
                                
                        //    }
                        //    ctx.BannerImages.AddRange(bannerImages);
                        //}
                        //if (model.InstagramImage != null)
                        //{
                        //    returnModel.InstagramImage = model.InstagramImage;
                        //}
                        returnModel.Currency = model.Currency;
                        //returnModel.ContactNo = model.ContactNo;
                        returnModel.DeliveryFee = model.DeliveryFee;
                        returnModel.FreeDeliveryThreshold = model.FreeDeliveryThreshold;
                        //returnModel.Tax = model.Tax;
                        ctx.SaveChanges();

                    }
                    else
                    {
                        ctx.Settings.Add(model);
                        ctx.SaveChanges();
                    }

                    BasketSettings.LoadSettings();
                    //returnModel.BannerImages = BasketSettings.Settings.BannerImages;

                    return Ok(new CustomResponse<Settings>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = returnModel
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [HttpPost]
        [Route("SetPrivacyPolicy")]
        public async Task<IHttpActionResult> SetPrivacyPolicy(PrivacyViewModel model)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    ctx.Settings.FirstOrDefault().PrivacyPolicy = model.PrivacyPolicy;
                    ctx.SaveChanges();
                    BasketSettings.Settings.PrivacyPolicy = model.PrivacyPolicy;
                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [HttpPost]
        [Route("SetAboutUs")]
        public async Task<IHttpActionResult> SetAboutUs(AboutUsViewModel model)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    ctx.Settings.FirstOrDefault().AboutUs = model.AboutUs;
                    ctx.SaveChanges();
                    //BasketSettings.Settings.RefundExchange = model.AboutUs;
                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        //[BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        //[HttpPost]
        //[Route("SetRefundExchange")]
        //public async Task<IHttpActionResult> SetRefundExchange(RefundExchangeViewModel model)
        //{
        //    try
        //    {
        //        using (RiscoContext ctx = new RiscoContext())
        //        {
        //            //ctx.Settings.FirstOrDefault().RefundExchange = model.RefundExchange;
        //            ctx.SaveChanges();
        //            //BasketSettings.Settings.RefundExchange = model.RefundExchange;
        //            return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [HttpPost]
        [Route("SetTermsAndConditions")]
        public async Task<IHttpActionResult> SetTermsAndConditions(TermsConditionsViewModel model)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    ctx.Settings.FirstOrDefault().TermsConditions = model.TermsConditions;
                    ctx.SaveChanges();
                    BasketSettings.Settings.TermsConditions = model.TermsConditions;
                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [HttpPost]
        [Route("SetTermsAndConditions")]
        public async Task<IHttpActionResult> SetFAQs(FAQsViewModel model)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    ctx.Settings.FirstOrDefault().FAQs = model.FAQs;
                    ctx.SaveChanges();
                    BasketSettings.Settings.FAQs = model.FAQs;
                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        //[BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        //[HttpPost]
        //[Route("SetUrls")]
        //public async Task<IHttpActionResult> SetUrls(SetUrlsViewModel model)
        //{
        //    try
        //    {
        //        using (RiscoContext ctx = new RiscoContext())
        //        {
        //            var setting = ctx.Settings.FirstOrDefault();
        //            setting.GoogleUrl = model.GoogleUrl;
        //            setting.FacebookUrl = model.FacebookUrl;
        //            setting.InstagramUrl = model.InstagramUrl;
        //            setting.PintrestUrl = model.PintrestUrl;
        //            setting.TwitterUrl = model.TwitterUrl;
        //            ctx.SaveChanges();
        //            BasketSettings.LoadSettings();
        //            return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}
    }
}
