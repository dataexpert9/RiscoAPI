using BasketApi.Areas.Admin.ViewModels;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Areas.Admin.ViewModels;
using System.Data.Entity;
using WebApplication1.BindingModels;
using System.Web;
using System.Net.Http;
using System.IO;
using System.Configuration;
using static BasketApi.Global;

namespace BasketApi.Areas.SubAdmin.Controllers
{
    [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User", "Guest")]
    [RoutePrefix("api")]
    public class PostController : ApiController
    {
        [HttpPost]
        [Route("Post")]
        public async Task<IHttpActionResult> PostWithMedia()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                #region InitializingModel
                PostBindingModel model = new PostBindingModel();

                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                model.Text = httpRequest.Params["Text"];
                model.Visibility = Convert.ToInt32(httpRequest.Params["Visibility"]);
                model.RiskLevel = Convert.ToInt32(httpRequest.Params["RiskLevel"]);
                model.Location = httpRequest.Params["Location"];

                #endregion
                Validate(model);

                #region Validations
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multipart data is not included in request." }
                    });
                }
                #endregion

                using (RiscoContext ctx = new RiscoContext())
                {
                    Post post = new Post();
                    post.Text = model.Text;
                    post.Visibility = model.Visibility;
                    post.RiskLevel = model.RiskLevel;
                    post.Location = model.Location;
                    post.User_Id = userId;
                    post.CreatedDate = DateTime.UtcNow;
                    ctx.Posts.Add(post);

                    HttpFileCollection postedFiles = null;
                    string fileExtension = string.Empty;

                    #region ImageSaving
                    if (httpRequest.Files.Count > 0)
                    {
                        postedFiles = httpRequest.Files;

                        foreach (HttpPostedFile postedFile in postedFiles)
                        {
                            int Counter = 1;
                            if (postedFile != null && postedFile.ContentLength > 0)
                            {
                                IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                                var ext = Path.GetExtension(postedFile.FileName);
                                fileExtension = ext.ToLower();
                                if (!AllowedFileExtensions.Contains(fileExtension))
                                {
                                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                    {
                                        Message = "UnsupportedMediaType",
                                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                        Result = new Error { ErrorMessage = "Please Upload image of type .jpg,.gif,.png." }
                                    });
                                }
                                else if (postedFile.ContentLength > Global.MaximumImageSize)
                                {
                                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                    {
                                        Message = "UnsupportedMediaType",
                                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                        Result = new Error { ErrorMessage = "Please Upload a file upto " + Global.ImageSize + "." }
                                    });
                                }

                                newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["PostMediaFolderPath"] + post.Id + Counter + fileExtension);
                                postedFile.SaveAs(newFullPath);

                                Media media = new Media
                                {
                                    Post_Id = post.Id,
                                    Url = ConfigurationManager.AppSettings["PostMediaFolderPath"] + post.Id + Counter + fileExtension,
                                    Type = (int)MediaTypes.Image
                                };
                                ctx.Medias.Add(media);
                                ctx.SaveChanges();
                                Counter++;
                            }
                        }
                    }
                    #endregion

                    CustomResponse<Post> response = new CustomResponse<Post> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = post };
                    return Ok(response);
                }               
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpPost]
        [Route("GetPosts")]
        public async Task<IHttpActionResult> GetPosts()
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {

                    CustomResponse<List<Post>> response = new CustomResponse<List<Post>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = ctx.Posts.Where(x => x.IsDeleted == false).ToList()
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }
    }
}
