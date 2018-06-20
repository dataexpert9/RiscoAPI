using BasketApi.BindingModels;
using BasketApi.ViewModels;
using DAL;
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

namespace BasketApi.Controllers
{
    [RoutePrefix("api/Videos")]
    public class VideosController : ApiController
    {
        //[BasketApi.Authorize("User", "SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        //[HttpGet]
        //[Route("GetHowItWorksVideoUrl")]
        //public async Task<IHttpActionResult> GetHowItWorksVideoUrl()
        //{
        //    try
        //    {
        //        using (RiscoContext ctx = new RiscoContext())
        //        {
        //            var setting = ctx.Settings.FirstOrDefault();

        //            if (setting != null)
        //                return Ok(new CustomResponse<HowItWorksViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new HowItWorksViewModel {Thumbnail = setting.HowItWorksUrlThumbnail,  Url = setting.HowItWorksUrl, HowItWorksDescription = setting.HowItWorksDescription } });
        //            else
        //                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        //[BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        //[HttpPost]
        //[Route("AddHowItWorksVideo")]
        //public async Task<IHttpActionResult> AddHowItWorksVideo()
        //{
        //    try
        //    {
        //        var httpRequest = HttpContext.Current.Request;

        //        if (httpRequest.Files.Count > 0)
        //        {
        //            var postedFile = httpRequest.Files[0];
        //            var fileExtension = Path.GetExtension(postedFile.FileName).ToLower();
        //            var newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["VideosFolderPath"] + "HowItWorks" + fileExtension);
        //            postedFile.SaveAs(newFullPath);
        //            using (RiscoContext ctx = new RiscoContext())
        //            {
        //                var setting = ctx.Settings.FirstOrDefault();
        //                if (setting != null)
        //                    setting.HowItWorksUrl = ConfigurationManager.AppSettings["VideosFolderPath"] + "HowItWorks" + fileExtension;
        //                else
        //                    ctx.Settings.Add(new Settings { HowItWorksUrl = ConfigurationManager.AppSettings["VideosFolderPath"] + "HowItWorks" + fileExtension });
        //                ctx.SaveChanges();
        //                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = ConfigurationManager.AppSettings["VideosFolderPath"] + "HowItWorks" + fileExtension });
        //            }
        //        }
        //        else
        //            return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "File not found" } });

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        //[BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        //[HttpPost]
        //[Route("AddHowItWorks")]
        //public async Task<IHttpActionResult> AddHowItWorks(AddHowItWorksBindingModel model)
        //{
        //    try
        //    {
        //        using (RiscoContext ctx = new RiscoContext())
        //        {
        //            var setting = ctx.Settings.FirstOrDefault();
        //            if (setting != null)
        //            {
        //                setting.HowItWorksUrl = model.Url;
        //                setting.HowItWorksDescription = model.HowItWorksDescription;
        //                setting.HowItWorksUrlThumbnail = Youtube.GetThumbnailUrl(model.Url);
        //            }
        //            else
        //                ctx.Settings.Add(new Settings { HowItWorksUrl = model.Url, HowItWorksUrlThumbnail = Youtube.GetThumbnailUrl(model.Url), HowItWorksDescription = model.HowItWorksDescription });
        //            ctx.SaveChanges();
        //            return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = "Success" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}
    }
}
