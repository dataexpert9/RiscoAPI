using BasketApi;
using BasketApi.Areas.Admin.ViewModels;
using BasketApi.Areas.SubAdmin.Models;
using BasketApi.CustomAuthorization;
using BasketApi.Models;
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
using System.Web.Security;
using WebApplication1.Areas.Admin.ViewModels;
using System.Data.Entity;
using BasketApi.BindingModels;
using static BasketApi.Utility;
using Newtonsoft.Json;
using static BasketApi.Global;
using System.Globalization;
using System.Data.Entity.Core.Objects;
using System.Web.Hosting;
using BasketApi.Components.Helpers;
using WebApplication1.ViewModels;
using WebApplication1.Models;
using WebApplication1.BindingModels;
using Z.EntityFramework.Plus;

namespace WebApplication1.Areas.Admin.Controllers
{
    [RoutePrefix("api/Admin")]
    public class AdminController : ApiController
    {
        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        /// <summary>
        /// Add admin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddAdmin")]
        public async Task<IHttpActionResult> AddAdmin()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                DAL.Admin model = new DAL.Admin();
                DAL.Admin existingAdmin = new DAL.Admin();

                if (httpRequest.Params["Id"] != null)
                    model.Id = Convert.ToInt32(httpRequest.Params["Id"]);

                if (httpRequest.Params["ImageDeletedOnEdit"] != null)
                    model.ImageDeletedOnEdit = Convert.ToBoolean(httpRequest.Params["ImageDeletedOnEdit"]);

                model.FirstName = httpRequest.Params["FirstName"];
                model.LastName = httpRequest.Params["LastName"];
                model.Email = httpRequest.Params["Email"];
                model.Phone = httpRequest.Params["Phone"];
                model.Role = Convert.ToInt16(httpRequest.Params["Role"]);
                model.Password = httpRequest.Params["Password"];
                model.ImageUrl = httpRequest.Params["ImageUrl"];

                model.Status = (int)Global.StatusCode.NotVerified;

                if (httpRequest.Params["Store_Id"] != null)
                    model.Store_Id = Convert.ToInt32(httpRequest.Params["Store_Id"]);

                Validate(model);

                #region Validations

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                //if (!Request.Content.IsMimeMultipartContent())
                //{
                //    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                //    {
                //        Message = "UnsupportedMediaType",
                //        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                //        Result = new Error { ErrorMessage = "Multipart data is not included in request" }
                //    });
                //}
                //else if (httpRequest.Files.Count > 1)
                //{
                //    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                //    {
                //        Message = "UnsupportedMediaType",
                //        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                //        Result = new Error { ErrorMessage = "Multiple images are not supported, please upload one image" }
                //    });
                //}
                #endregion

                using (RiscoContext ctx = new RiscoContext())
                {
                    if (model.Id == 0)
                    {

                        if (ctx.Admins.Any(x => x.Email == model.Email && x.IsDeleted == false))
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "Conflict",
                                StatusCode = (int)HttpStatusCode.Conflict,
                                Result = new Error { ErrorMessage = "Admin with same email already exists" }
                            });
                        }
                    }
                    else
                    {
                        existingAdmin = ctx.Admins.FirstOrDefault(x => x.Id == model.Id);
                        model.Password = existingAdmin.Password;
                        if (existingAdmin.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase) == false || existingAdmin.Store_Id != model.Store_Id)
                        {
                            if (ctx.Admins.Any(x => x.IsDeleted == false && x.Store_Id == model.Store_Id && x.Email.Equals(model.Email.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "Conflict",
                                    StatusCode = (int)HttpStatusCode.Conflict,
                                    Result = new Error { ErrorMessage = "Admin with same email already exists" }
                                });
                            }
                        }
                    }

                    string fileExtension = string.Empty;
                    HttpPostedFile postedFile = null;
                    //#region ImageSaving
                    //if (httpRequest.Files.Count > 0)
                    //{
                    //    postedFile = httpRequest.Files[0];
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
                    //            //int count = 1;
                    //            //fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
                    //            //newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["AdminImageFolderPath"] + postedFile.FileName);

                    //            //while (File.Exists(newFullPath))
                    //            //{
                    //            //    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                    //            //    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["AdminImageFolderPath"] + tempFileName + extension);
                    //            //}
                    //            //postedFile.SaveAs(newFullPath);
                    //        }
                    //    }
                    //    //model.ImageUrl = ConfigurationManager.AppSettings["AdminImageFolderPath"] + Path.GetFileName(newFullPath);
                    //}
                    //#endregion

                    if (model.Id == 0)
                    {
                        ctx.Admins.Add(model);
                        ctx.SaveChanges();
                     //   var guid = Guid.NewGuid();
                      //  newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["AdminImageFolderPath"] + model.Id + "_" + guid + fileExtension);
                     ////   postedFile.SaveAs(newFullPath);
                      //  model.ImageUrl = ConfigurationManager.AppSettings["AdminImageFolderPath"] + model.Id + "_" + guid + fileExtension;
                        ctx.SaveChanges();

                    }
                    else
                    {
                        //existingProduct = ctx.Products.FirstOrDefault(x => x.Id == model.Id);
                        //if (httpRequest.Files.Count == 0)
                        //{
                            // Check if image deleted
                            //if (model.ImageDeletedOnEdit == false)
                            //{
                            //    model.ImageUrl = existingAdmin.ImageUrl;
                            //}
                        //}
                        //else
                        //{
                        //    Utility.DeleteFileIfExists(existingAdmin.ImageUrl);
                        //    var guid = Guid.NewGuid();
                        //    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["AdminImageFolderPath"] + model.Id + "_" + guid + fileExtension);
                        //    postedFile.SaveAs(newFullPath);
                        //    model.ImageUrl = ConfigurationManager.AppSettings["AdminImageFolderPath"] + model.Id + "_" + guid + fileExtension;
                        //}

                        ctx.Entry(existingAdmin).CurrentValues.SetValues(model);
                        ctx.SaveChanges();
                    }

                    await model.GenerateToken(Request);

                    CustomResponse<DAL.Admin> response = new CustomResponse<DAL.Admin>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = model
                    };

                    return Ok(response);

                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        /// <summary>
        /// Add category with image. This is multipart request
        /// </summary>
        /// <returns></returns>
        [Route("AddCategory")]
        public async Task<IHttpActionResult> AddCategoryWithImage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                Category model = new Category();
                Category existingCategory = new Category();

                if (httpRequest.Params["Id"] != null)
                {
                    model.Id = Convert.ToInt32(httpRequest.Params["Id"]);
                }
                if (httpRequest.Params["ParentCategoryId"] != null)
                {
                    model.ParentCategoryId = Convert.ToInt32(httpRequest.Params["ParentCategoryId"]);
                }
                if (httpRequest.Params["ImageDeletedOnEdit"] != null)
                {
                    model.ImageDeletedOnEdit = Convert.ToBoolean(httpRequest.Params["ImageDeletedOnEdit"]);
                }
                model.Name = httpRequest.Params["Name"];
                model.Description = httpRequest.Params["Description"];
                model.Store_Id = Convert.ToInt32(httpRequest.Params["Store_Id"]);

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
                        Result = new Error { ErrorMessage = "Multipart data is not included in request" }
                    });
                }
                else if (httpRequest.Files.Count > 1)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multiple images are not supported, please upload one image" }
                    });
                }
                #endregion

                using (RiscoContext ctx = new RiscoContext())
                {
                    if (model.Id == 0)
                    {
                        if (ctx.Categories.Any(x => x.Store_Id == model.Store_Id && x.Name.Equals(model.Name.Trim(), StringComparison.InvariantCultureIgnoreCase) && x.IsDeleted == false))
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "Conflict",
                                StatusCode = (int)HttpStatusCode.Conflict,
                                Result = new Error { ErrorMessage = "Category already exist under same store" }
                            });
                        }
                    }
                    else
                    {
                        existingCategory = ctx.Categories.FirstOrDefault(x => x.Id == model.Id);
                        if (existingCategory.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase) == false || existingCategory.Store_Id != model.Store_Id)
                        {
                            if (ctx.Categories.Any(x => x.IsDeleted == false && x.Store_Id == model.Store_Id && x.Name.Equals(model.Name.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "Conflict",
                                    StatusCode = (int)HttpStatusCode.Conflict,
                                    Result = new Error { ErrorMessage = "Category with same name already exist under same store" }
                                });
                            }
                        }

                        if (existingCategory.Id == model.ParentCategoryId)
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "Conflict",
                                StatusCode = (int)HttpStatusCode.Conflict,
                                Result = new Error { ErrorMessage = "Parent category name and child category name must be different" }
                            });
                        }
                    }

                    HttpPostedFile postedFile = null;
                    string fileExtension = string.Empty;

                    #region ImageSaving
                    if (httpRequest.Files.Count > 0)
                    {
                        postedFile = httpRequest.Files[0];
                        if (postedFile != null && postedFile.ContentLength > 0 && Request.Content.IsMimeMultipartContent())
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
                                    Result = new Error { ErrorMessage = "Please Upload image of type .jpg,.gif,.png" }
                                });
                            }
                            else if (postedFile.ContentLength > Global.MaximumImageSize)
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload a file upto " + Global.ImageSize }
                                });
                            }
                            else
                            {
                                //int count = 1;
                                //fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
                                //newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["CategoryImageFolderPath"] + postedFile.FileName);

                                //while (File.Exists(newFullPath))
                                //{
                                //    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                //    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["CategoryImageFolderPath"] + tempFileName + extension);
                                //}
                                //postedFile.SaveAs(newFullPath);
                            }
                        }
                        //model.ImageUrl = ConfigurationManager.AppSettings["CategoryImageFolderPath"] + Path.GetFileName(newFullPath);
                    }
                    #endregion


                    if (model.Id == 0)
                    {
                        ctx.Categories.Add(model);
                        ctx.SaveChanges();
                        var guid = Guid.NewGuid();
                        newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["CategoryImageFolderPath"] + model.Id + "_" + guid + fileExtension);
                        postedFile.SaveAs(newFullPath);
                        model.ImageUrl = ConfigurationManager.AppSettings["CategoryImageFolderPath"] + model.Id + "_" + guid + fileExtension;
                        ctx.SaveChanges();
                    }
                    else
                    {
                        //var existingCategory = ctx.Categories.FirstOrDefault(x => x.Id == model.Id);
                        if (httpRequest.Files.Count == 0)
                        {
                            // Check if image deleted
                            if (model.ImageDeletedOnEdit == false)
                            {
                                model.ImageUrl = existingCategory.ImageUrl;
                            }
                        }
                        else
                        {
                            Utility.DeleteFileIfExists(existingCategory.ImageUrl);
                            var guid = Guid.NewGuid();
                            newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["CategoryImageFolderPath"] + model.Id + "_" + guid + fileExtension);
                            postedFile.SaveAs(newFullPath);
                            model.ImageUrl = ConfigurationManager.AppSettings["CategoryImageFolderPath"] + model.Id + "_" + guid + fileExtension;
                        }

                        ctx.Entry(existingCategory).CurrentValues.SetValues(model);
                        ctx.SaveChanges();
                    }


                    CustomResponse<Category> response = new CustomResponse<Category>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = model
                    };

                    return Ok(response);

                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        /// <summary>
        /// Add product with image. This is multipart request
        /// </summary>
        /// <returns></returns>
        [Route("AddProduct")]
        public async Task<IHttpActionResult> AddProductWithImage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                Product model = new Product();
                Product existingProduct = new Product();

                if (httpRequest.Params["Id"] != null)
                {
                    model.Id = Convert.ToInt32(httpRequest.Params["Id"]);
                }

                //if (httpRequest.Params["ImageDeletedOnEdit"] != null)
                //{
                //    model.ImageDeletedOnEdit = Convert.ToBoolean(httpRequest.Params["ImageDeletedOnEdit"]);
                //}

                model.WeightUnit = Convert.ToInt32(httpRequest.Params["WeightUnit"]);

                if (model.WeightUnit == 1) // 1 for gm, 2 for kg
                    model.WeightInGrams = Convert.ToDouble(httpRequest.Params["WeightInGrams"]);
                else
                    model.WeightInKiloGrams = Convert.ToDouble(httpRequest.Params["WeightInKiloGrams"]);

                model.Name = httpRequest.Params["Name"];
                model.Price = Convert.ToDouble(httpRequest.Params["Price"]);
                model.Category_Id = Convert.ToInt32(httpRequest.Params["Category_Id"]);
                model.Description = httpRequest.Params["Description"];
                model.Store_Id = Convert.ToInt32(httpRequest.Params["Store_Id"]);

                model.IsPopular = Convert.ToBoolean(httpRequest.Params["IsPopular"]);

                List<Product_Images> lstVideos = new List<Product_Images>();

                if (httpRequest.Params["ProductVideos"] != null)
                {
                    lstVideos = JsonConvert.DeserializeObject<List<Product_Images>>(httpRequest.Params["ProductVideos"]);
                }

                List<Product_Images> lstImages = new List<Product_Images>();

                if (httpRequest.Params["ProductImages"] != null)
                {
                    lstImages = JsonConvert.DeserializeObject<List<Product_Images>>(httpRequest.Params["ProductImages"]);
                }


                Validate(model);

                #region Validations

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                //if (httpRequest.Files.Count > 0)
                //{
                //    if (!Request.Content.IsMimeMultipartContent())
                //    {
                //        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                //        {
                //            Message = "UnsupportedMediaType",
                //            StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                //            Result = new Error { ErrorMessage = "Multipart data is not included in request" }
                //        });
                //    }
                //}
                #endregion

                using (RiscoContext ctx = new RiscoContext())
                {
                    if (model.Id == 0)
                    {
                        if (ctx.Products.Any(x => x.Category_Id == model.Category_Id && x.Name == model.Name && x.Store_Id == model.Store_Id && x.IsDeleted == false))
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "Conflict",
                                StatusCode = (int)HttpStatusCode.Conflict,
                                Result = new Error { ErrorMessage = "Product already exist under same store and category" }
                            });
                        }
                    }
                    else
                    {
                        existingProduct = ctx.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == model.Id);
                        if (existingProduct.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase) == false || existingProduct.Category_Id != model.Category_Id || existingProduct.Store_Id != model.Store_Id)
                        {
                            if (ctx.Products.Any(x => x.IsDeleted == false && x.Category_Id == model.Category_Id && x.Store_Id == model.Store_Id && x.Name.Equals(model.Name.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "Conflict",
                                    StatusCode = (int)HttpStatusCode.Conflict,
                                    Result = new Error { ErrorMessage = "Product with same name already exist under same store and category" }
                                });
                            }
                        }
                    }


                   

                    //HttpPostedFile postedFile = null;
                    //string fileExtension = string.Empty;
                    //bool ProductImageChanged = false;
                    //bool BannerImageChanged = false;
                    //#region ImageSaving
                    //List<Product_Images> productImages = new List<Product_Images>();
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
                    //    //var guid = Guid.NewGuid();
                    //    //if (Path.GetFileNameWithoutExtension(postedFile.FileName) == "Banner")
                    //    //{
                    //    //    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["BannerImageFolderPath"] + model.Id + "_" + guid + fileExtension);
                    //    //    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["BannerImageFolderPath"]));
                    //    //    postedFile.SaveAs(newFullPath);
                    //    //    BannerImageChanged = true;
                    //    //}
                    //    //else if (Path.GetFileNameWithoutExtension(postedFile.FileName).Contains("Product"))
                    //    //{
                    //    //    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["ProductImageFolderPath"] + model.Id + "_" + guid + fileExtension);
                    //    //    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["ProductImageFolderPath"]));
                    //    //    postedFile.SaveAs(newFullPath);
                    //    //    model.ImageUrl = ConfigurationManager.AppSettings["ProductImageFolderPath"] + model.Id + "_" + guid + fileExtension;
                    //    //    productImages.Add(new Product_Images { Url = ConfigurationManager.AppSettings["ProductImageFolderPath"] + model.Id + "_" + guid + fileExtension });
                    //    //    ProductImageChanged = true;
                    //    //}
                    //}
                    //#endregion

                   // var productCategory = ctx.Categories.FirstOrDefault(x => x.Id == model.Category_Id);

                    if (model.Id == 0)
                    {
                        foreach (var item in lstImages)
                        {
                            model.ProductImages.Add(new Product_Images { Url = item.Url, IsVideo = false });
                        }
                        foreach (var video in lstVideos)
                        {
                            model.ProductImages.Add(new Product_Images { IsVideo = true, Url = video.Url, ThumbnailUrl = Youtube.GetThumbnailUrl(video.Url),Title=video.Title,Description=video.Description });
                        }

                        ctx.Products.Add(model);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        //existingProduct = ctx.Products.FirstOrDefault(x => x.Id == model.Id);

                        // Check if image deleted
                        //if (model.ImageDeletedOnEdit == false && ProductImageChanged == false)
                        //    model.ImageUrl = existingProduct.ImageUrl;

                        ctx.Entry(existingProduct).CurrentValues.SetValues(model);

                        //bool videoChanged = false;

                        //var existingVideos = existingProduct.ProductImages.Where(x => x.IsVideo == true).ToList();

                        //if (lstVideos.Count != existingProduct.ProductImages.Count(x => x.IsVideo == true))
                        //{
                        //    videoChanged = true;
                        //}
                        //else
                        //{
                        //    foreach (var video in lstVideos)
                        //    {
                        //        foreach (var existingVideo in existingVideos)
                        //        {
                        //            if (existingVideo.Url != video.Url || existingVideo.Title!=video.Title)
                        //            {
                        //                videoChanged = true;
                        //                break;
                        //            }
                        //        }
                        //        if (videoChanged)
                        //            break;
                        //    }
                        //}

                        //foreach (var item in lstImages)
                        //{
                        //    model.ProductImages.Add(new Product_Images { Url = item.Url, IsVideo = false });
                        //}

                        //if (model.ProductImages.Count > 0 || videoChanged)
                        //{
                        //    var newImagesReceived = lstImages.Count > 0;
                        //    foreach (var oldImage in existingProduct.ProductImages.ToList())
                        //    {
                        //        if (newImagesReceived == false)
                        //        {
                        //            model.ProductImages.Add(new Product_Images { Product_Id = oldImage.Product_Id, Url = oldImage.Url, IsVideo = oldImage.IsVideo,Title=oldImage.Title });
                        //        }
                        //        ctx.ProductImages.Remove(oldImage);
                        //    }

                        //    productImages.RemoveAll(x => x.IsVideo == true);

                        //    foreach (var video in lstVideos)
                        //        productImages.Add(new Product_Images { IsVideo = true, Url = video.Url, ThumbnailUrl = Youtube.GetThumbnailUrl(video.Url),Title=video.Title });
                        //}
                        //else
                        //    productImages = existingProduct.ProductImages.ToList();
                        ctx.ProductImages.RemoveRange(existingProduct.ProductImages);
                        //existingProduct.ProductImages.ToList().RemoveRange()

                        foreach (var item in lstImages)
                        {
                            existingProduct.ProductImages.Add(new Product_Images { Url = item.Url, IsVideo = false, Product_Id = existingProduct.Id });
                        }
                        foreach (var video in lstVideos)
                        {
                            existingProduct.ProductImages.Add(new Product_Images { IsVideo = true, Url = video.Url, ThumbnailUrl = Youtube.GetThumbnailUrl(video.Url), Title = video.Title, Product_Id = existingProduct.Id,Description=video.Description });
                        }
                        ctx.SaveChanges();
                    }

                    CustomResponse<Product> response = new CustomResponse<Product>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = model
                    };

                    return Ok(response);

                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

     
        #region  Commented old AddProductWithImage

        //public async Task<IHttpActionResult> AddProductWithImage()
        //{
        //    try
        //    {
        //        var httpRequest = HttpContext.Current.Request;
        //        string newFullPath = string.Empty;
        //        string fileNameOnly = string.Empty;

        //        Product model = new Product();
        //        Product existingProduct = new Product();

        //        if (httpRequest.Params["Id"] != null)
        //        {
        //            model.Id = Convert.ToInt32(httpRequest.Params["Id"]);
        //        }

        //        if (httpRequest.Params["ImageDeletedOnEdit"] != null)
        //        {
        //            model.ImageDeletedOnEdit = Convert.ToBoolean(httpRequest.Params["ImageDeletedOnEdit"]);
        //        }

        //        model.WeightUnit = Convert.ToInt32(httpRequest.Params["WeightUnit"]);

        //        if (model.WeightUnit == 1) // 1 for gm, 2 for kg
        //            model.WeightInGrams = Convert.ToDouble(httpRequest.Params["WeightInGrams"]);
        //        else
        //            model.WeightInKiloGrams = Convert.ToDouble(httpRequest.Params["WeightInKiloGrams"]);

        //        model.Name = httpRequest.Params["Name"];
        //        model.Price = Convert.ToDouble(httpRequest.Params["Price"]);
        //        model.Category_Id = Convert.ToInt32(httpRequest.Params["Category_Id"]);
        //        model.Description = httpRequest.Params["Description"];
        //        model.Store_Id = Convert.ToInt32(httpRequest.Params["Store_Id"]);

        //        Validate(model);

        //        #region Validations

        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        if (!Request.Content.IsMimeMultipartContent())
        //        {
        //            return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //            {
        //                Message = "UnsupportedMediaType",
        //                StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
        //                Result = new Error { ErrorMessage = "Multipart data is not included in request" }
        //            });
        //        }
        //        else if (httpRequest.Files.Count > 1)
        //        {
        //            return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //            {
        //                Message = "UnsupportedMediaType",
        //                StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
        //                Result = new Error { ErrorMessage = "Multiple images are not supported, please upload one image" }
        //            });
        //        }
        //        #endregion

        //        using (SkriblContext ctx = new SkriblContext())
        //        {
        //            if (model.Id == 0)
        //            {
        //                if (ctx.Products.Any(x => x.Category_Id == model.Category_Id && x.Name == model.Name && x.Store_Id == model.Store_Id && x.IsDeleted == false))
        //                {
        //                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //                    {
        //                        Message = "Conflict",
        //                        StatusCode = (int)HttpStatusCode.Conflict,
        //                        Result = new Error { ErrorMessage = "Product already exist under same store and category" }
        //                    });
        //                }
        //            }
        //            else
        //            {
        //                existingProduct = ctx.Products.FirstOrDefault(x => x.Id == model.Id);
        //                if (existingProduct.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase) == false || existingProduct.Category_Id != model.Category_Id || existingProduct.Store_Id != model.Store_Id)
        //                {
        //                    if (ctx.Products.Any(x => x.IsDeleted == false && x.Category_Id == model.Category_Id && x.Store_Id == model.Store_Id && x.Name.Equals(model.Name.Trim(), StringComparison.InvariantCultureIgnoreCase)))
        //                    {
        //                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //                        {
        //                            Message = "Conflict",
        //                            StatusCode = (int)HttpStatusCode.Conflict,
        //                            Result = new Error { ErrorMessage = "Product with same name already exist under same store and category" }
        //                        });
        //                    }
        //                }
        //            }

        //            HttpPostedFile postedFile = null;
        //            string fileExtension = string.Empty;

        //            #region ImageSaving
        //            if (httpRequest.Files.Count > 0)
        //            {
        //                postedFile = httpRequest.Files[0];
        //                if (postedFile != null && postedFile.ContentLength > 0)
        //                {
        //                    IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
        //                    var ext = Path.GetExtension(postedFile.FileName);
        //                    fileExtension = ext.ToLower();
        //                    if (!AllowedFileExtensions.Contains(fileExtension))
        //                    {
        //                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //                        {
        //                            Message = "UnsupportedMediaType",
        //                            StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
        //                            Result = new Error { ErrorMessage = "Please Upload image of type .jpg,.gif,.png" }
        //                        });
        //                    }
        //                    else if (postedFile.ContentLength > Global.MaximumImageSize)
        //                    {
        //                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //                        {
        //                            Message = "UnsupportedMediaType",
        //                            StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
        //                            Result = new Error { ErrorMessage = "Please Upload a file upto " + Global.ImageSize }
        //                        });
        //                    }
        //                    else
        //                    {
        //                        //int count = 1;
        //                        //fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
        //                        //newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["ProductImageFolderPath"] + postedFile.FileName);

        //                        //while (File.Exists(newFullPath))
        //                        //{
        //                        //    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
        //                        //    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["ProductImageFolderPath"] + tempFileName + fileExtension);
        //                        //}
        //                        //postedFile.SaveAs(newFullPath);
        //                    }
        //                }
        //                //model.ImageUrl = ConfigurationManager.AppSettings["ProductImageFolderPath"] + Path.GetFileName(newFullPath);
        //            }
        //            #endregion

        //            if (model.Id == 0)
        //            {
        //                ctx.Products.Add(model);
        //                ctx.SaveChanges();
        //                var guid = Guid.NewGuid();
        //                newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["ProductImageFolderPath"] + model.Id + "_" + guid + fileExtension);
        //                postedFile.SaveAs(newFullPath);
        //                model.ImageUrl = ConfigurationManager.AppSettings["ProductImageFolderPath"] + model.Id + "_" + guid + fileExtension;
        //                ctx.SaveChanges();
        //            }
        //            else
        //            {
        //                //existingProduct = ctx.Products.FirstOrDefault(x => x.Id == model.Id);
        //                if (httpRequest.Files.Count == 0)
        //                {
        //                    // Check if image deleted
        //                    if (model.ImageDeletedOnEdit == false)
        //                    {
        //                        model.ImageUrl = existingProduct.ImageUrl;
        //                    }
        //                }
        //                else
        //                {
        //                    Utility.DeleteFileIfExists(existingProduct.ImageUrl);
        //                    var guid = Guid.NewGuid();
        //                    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["ProductImageFolderPath"] + model.Id + "_" + guid + fileExtension);
        //                    postedFile.SaveAs(newFullPath);
        //                    model.ImageUrl = ConfigurationManager.AppSettings["ProductImageFolderPath"] + model.Id + "_" + guid + fileExtension;
        //                }

        //                ctx.Entry(existingProduct).CurrentValues.SetValues(model);
        //                ctx.SaveChanges();
        //            }



        //            CustomResponse<Product> response = new CustomResponse<Product>
        //            {
        //                Message = Global.ResponseMessages.Success,
        //                StatusCode = (int)HttpStatusCode.OK,
        //                Result = model
        //            };

        //            return Ok(response);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        #endregion
        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        /// <summary>
        /// Add store with image, multipart request
        /// </summary>
        /// <returns></returns>
        [Route("AddStore")]
        public async Task<IHttpActionResult> AddStoreWithImage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                StoreBindingModel model = new StoreBindingModel();
                Store existingStore = new Store();

                if (httpRequest.Params["Id"] != null)
                {
                    model.Id = Convert.ToInt32(httpRequest.Params["Id"]);
                }

                if (httpRequest.Params["ImageDeletedOnEdit"] != null)
                {
                    model.ImageDeletedOnEdit = Convert.ToBoolean(httpRequest.Params["ImageDeletedOnEdit"]);
                }

                model.Name = httpRequest.Params["StoreName"];
                model.Latitude = Convert.ToDouble(httpRequest.Params["Lat"]);
                model.Longitude = Convert.ToDouble(httpRequest.Params["Long"]);
                model.Description = httpRequest.Params["Description"];
                model.Address = httpRequest.Params["Address"];

                TimeSpan openFrom, openTo;
                TimeSpan.TryParse(httpRequest.Params["Open_From"], out openFrom);
                TimeSpan.TryParse(httpRequest.Params["Open_To"], out openTo);

                if (openFrom != null)
                    model.Open_From = openFrom;

                if (openTo != null)
                    model.Open_To = openTo;

                if (httpRequest.Params["StoreDeliveryHours"] != null)
                {
                    var storeDeliveryHours = JsonConvert.DeserializeObject<StoreDeliveryHours>(httpRequest.Params["StoreDeliveryHours"]);

                    if (model.Id > 0)
                    {
                    }
                    else
                    {
                    }
                    model.StoreDeliveryHours = storeDeliveryHours;
                }


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
                        Result = new Error { ErrorMessage = "Multipart data is not included in request" }
                    });
                }
                else if (httpRequest.Files.Count > 1)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multiple images are not supported, please upload one image" }
                    });
                }
                #endregion

                using (RiscoContext ctx = new RiscoContext())
                {
                    if (model.Id == 0)
                    {
                        if (ctx.Stores.Any(x => x.Name == model.Name && x.Longitude == model.Longitude && x.Latitude == model.Latitude && x.IsDeleted == false))
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "Conflict",
                                StatusCode = (int)HttpStatusCode.Conflict,
                                Result = new Error { ErrorMessage = "Store with same name and location already exists." }
                            });
                        }
                    }
                    else
                    {
                        existingStore = ctx.Stores.Include(x => x.StoreDeliveryHours).FirstOrDefault(x => x.Id == model.Id);
                        if (existingStore.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase) == false)
                        {
                            if (ctx.Stores.Any(x => x.IsDeleted == false && x.Id == model.Id && x.Name.Equals(model.Name.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "Conflict",
                                    StatusCode = (int)HttpStatusCode.Conflict,
                                    Result = new Error { ErrorMessage = "Store with same name already exist" }
                                });
                            }
                        }
                    }

                    HttpPostedFile postedFile = null;
                    string fileExtension = string.Empty;

                    #region ImageSaving
                    if (httpRequest.Files.Count > 0)
                    {
                        postedFile = httpRequest.Files[0];
                        if (postedFile != null && postedFile.ContentLength > 0)
                        {
                            int MaxContentLength = 1024 * 1024 * 10; //Size = 10 MB  

                            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                            var ext = Path.GetExtension(postedFile.FileName);
                            fileExtension = ext.ToLower();
                            if (!AllowedFileExtensions.Contains(fileExtension))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload image of type .jpg,.gif,.png" }
                                });
                            }
                            else if (postedFile.ContentLength > MaxContentLength)
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload a file upto 1 mb" }
                                });
                            }
                            else
                            {
                                //    int count = 1;
                                //    fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
                                //    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["StoreImageFolderPath"] + postedFile.FileName);

                                //    while (File.Exists(newFullPath))
                                //    {
                                //        string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                //        newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["StoreImageFolderPath"] + tempFileName + fileExtension);
                                //    }
                                //    postedFile.SaveAs(newFullPath);
                            }
                        }
                        //model.ImageUrl = ConfigurationManager.AppSettings["StoreImageFolderPath"] + Path.GetFileName(newFullPath);
                    }
                    #endregion

                    Store storeModel = new Store();
                    storeModel.Id = model.Id;
                    storeModel.Name = model.Name;
                    storeModel.Open_From = model.Open_From;
                    storeModel.Open_To = model.Open_To;
                    storeModel.Description = model.Description;
                    storeModel.Latitude = model.Latitude;
                    storeModel.Longitude = model.Longitude;
                    storeModel.ImageUrl = model.ImageUrl;
                    storeModel.StoreDeliveryHours = model.StoreDeliveryHours;
                    storeModel.ImageDeletedOnEdit = model.ImageDeletedOnEdit;
                    storeModel.Location = Utility.CreatePoint(model.Latitude, model.Longitude);
                    storeModel.StoreDeliveryHours.Id = storeModel.Id;
                    storeModel.Address = model.Address;

                    if (storeModel.Id == 0)
                    {
                        ctx.Stores.Add(storeModel);
                        ctx.SaveChanges();
                        var guid = Guid.NewGuid();
                        newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["StoreImageFolderPath"] + storeModel.Id + "_" + guid + fileExtension);
                        postedFile.SaveAs(newFullPath);
                        storeModel.ImageUrl = ConfigurationManager.AppSettings["StoreImageFolderPath"] + storeModel.Id + "_" + guid + fileExtension;
                        ctx.SaveChanges();
                    }
                    else
                    {
                        if (httpRequest.Files.Count == 0)
                        {
                            // Check if image deleted
                            if (storeModel.ImageDeletedOnEdit == false)
                            {
                                storeModel.ImageUrl = existingStore.ImageUrl;
                            }
                        }
                        else
                        {
                            Utility.DeleteFileIfExists(existingStore.ImageUrl);
                            var guid = Guid.NewGuid();
                            newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["StoreImageFolderPath"] + storeModel.Id + "_" + guid + fileExtension);
                            postedFile.SaveAs(newFullPath);
                            storeModel.ImageUrl = ConfigurationManager.AppSettings["StoreImageFolderPath"] + storeModel.Id + "_" + guid + fileExtension;
                        }

                        ctx.Entry(existingStore).CurrentValues.SetValues(storeModel);

                        if (existingStore.StoreDeliveryHours == null)
                            ctx.StoreDeliveryHours.Add(storeModel.StoreDeliveryHours);
                        else
                            ctx.Entry(existingStore.StoreDeliveryHours).CurrentValues.SetValues(storeModel.StoreDeliveryHours);
                        ctx.SaveChanges();
                    }


                    CustomResponse<Store> response = new CustomResponse<Store>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = storeModel
                    };
                    return Ok(response);

                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        /// <summary>
        /// Get Dashboard Stats
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAdminDashboardStats")]
        public async Task<IHttpActionResult> GetAdminDashboardStats()
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    DateTime TodayDate = DateTime.UtcNow.Date;

                    WebDashboardStatsViewModel model = new WebDashboardStatsViewModel
                    {
                        TotalProducts = ctx.Products.Count(x => x.IsDeleted == false),
                        TotalStores = ctx.Stores.Count(x => x.IsDeleted == false),
                        TotalUsers = ctx.Users.Count(),
                        TodayOrders = ctx.Orders.Count(x => DbFunctions.TruncateTime(x.OrderDateTime) == TodayDate.Date),
                        DeviceUsage = ctx.Database.SqlQuery<DeviceStats>("select Count(Platform) as Count, (Count(Platform) * 100)/(select COUNT(Id) from UserDevices) as Percentage from UserDevices group by Platform order by Platform").ToList()
                    };

                    CustomResponse<WebDashboardStatsViewModel> response = new CustomResponse<WebDashboardStatsViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = model
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [HttpGet]
        [Route("SearchAdmins")]
        public async Task<IHttpActionResult> SearchAdmins(string FirstName, string LastName, string Email, string Phone, int? StoreId)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    string conditions = string.Empty;

                    if (!String.IsNullOrEmpty(FirstName))
                        conditions += " And Admins.FirstName Like '%" + FirstName.Trim() + "%'";

                    if (!String.IsNullOrEmpty(LastName))
                        conditions += " And Admins.LastName Like '%" + LastName.Trim() + "%'";

                    if (!String.IsNullOrEmpty(Email))
                        conditions += " And Admins.Email Like '%" + Email.Trim() + "%'";

                    if (!String.IsNullOrEmpty(Phone))
                        conditions += " And Admins.Phone Like '%" + Phone.Trim() + "%'";

                    if (StoreId.HasValue && StoreId.Value != 0)
                        conditions += " And Admins.Store_Id = " + StoreId;

                    #region query
                    var query = @"SELECT
  Admins.Id,
  Admins.FirstName,
  Admins.LastName,
  Admins.Email,
  Admins.Phone,
  Admins.Role,
  Admins.ImageUrl,
  Stores.Name AS StoreName
FROM Admins
LEFT OUTER JOIN Stores
  ON Stores.Id = Admins.Store_Id
WHERE Admins.IsDeleted = 0
AND Stores.IsDeleted = 0 " + conditions + @" UNION
SELECT
  Admins.Id,
  Admins.FirstName,
  Admins.LastName,
  Admins.Email,
  Admins.Phone,
  Admins.Role,
  Admins.ImageUrl,
  '' AS StoreName
FROM Admins
WHERE Admins.IsDeleted = 0
AND ISNULL(Admins.Store_Id, 0) = 0 " + conditions;

                    #endregion


                    var admins = ctx.Database.SqlQuery<SearchAdminViewModel>(query).ToList();

                    return Ok(new CustomResponse<SearchAdminListViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new SearchAdminListViewModel { Admins = admins } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User", "Guest")]
        [HttpGet]
        [Route("SearchProducts")]
        public async Task<IHttpActionResult> SearchProducts(string ProductName, float? ProductPrice, string CategoryName, int? StoreId, int? CatId)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    var userFavourites = ctx.Favourites.Where(x => x.User_ID == userId && x.IsDeleted == false).ToList();

                    var query = "select Products.*, Stores.Name as StoreName, Categories.Name as CategoryName from Products join Categories on Products.Category_Id = Categories.Id join Stores on Products.Store_Id = Stores.Id where Products.IsDeleted = 0 and Categories.IsDeleted = 0 and Stores.IsDeleted = 0";
                    if (!String.IsNullOrEmpty(CategoryName))
                        query += " And Categories.Name Like '%" + CategoryName + "%'";

                    if (!String.IsNullOrEmpty(ProductName))
                        query += " And Products.Name Like '%" + ProductName + "%'";

                    if (ProductPrice.HasValue)
                        query += " And Price = " + ProductPrice.Value;

                    if (StoreId.HasValue && StoreId.Value != 0)
                        query += " And Products.Store_Id = " + StoreId;

                    if (CatId.HasValue && CatId.Value != 0)
                        query += " And Products.Category_Id = " + CatId;

                    var products = ctx.Database.SqlQuery<SearchProductViewModel>(query).ToList();

                    foreach (var product in products)
                    {
                        product.Weight = product.WeightUnit == (int)WeightUnits.gm ? Convert.ToString(product.WeightInGrams) + " gm" : Convert.ToString(product.WeightInKiloGrams + " kg");
                        var avgRating = ctx.Database.SqlQuery<double?>("select Avg(Cast(Rating as float))  from ProductRatings where Product_Id = " + product.Id).ToList().FirstOrDefault();
                        if (avgRating != null)
                        {
                            product.AverageRating = avgRating.Value;
                        }

                        product.ProductImages = ctx.ProductImages.Where(x => x.Product_Id == product.Id).ToList();

                        if (userFavourites.Any(x => x.Product_Id == product.Id))
                            product.IsFavourite = true;
                        else
                            product.IsFavourite = false;

                    }

                    return Ok(new CustomResponse<SearchProductListViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new SearchProductListViewModel { Products = products } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User")]
        [HttpGet]
        [Route("SearchCategories")]
        public async Task<IHttpActionResult> SearchCategories(string CategoryName, int? StoreId, int? CatId)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var query = @"select
Categories.Id,
Categories.Name,
Categories.Description,
Categories.Status,
Categories.ImageUrl,
Categories.ParentCategoryId,
Categories.IsDeleted,
Stores.Name as StoreName
from Categories
join Stores on Categories.Store_Id = Stores.Id 
where Categories.IsDeleted = 0 and Stores.IsDeleted = 0 ";

                    if (!String.IsNullOrEmpty(CategoryName))
                        query += " And Categories.Name Like '%" + CategoryName + "%'";

                    if (StoreId.HasValue && StoreId.Value != 0)
                        query += " And Categories.Store_Id = " + StoreId;

                    if (CatId.HasValue && CatId.Value != 0)
                        query += " And Categories.ParentCategoryId = " + CatId;

                    query += "group by Categories.Id,Categories.Name, Categories.Description, Categories.Status, Categories.ImageUrl, Categories.ParentCategoryId, Categories.IsDeleted, Stores.Name";

                    var categories = ctx.Database.SqlQuery<SearchCategoryViewModel>(query).ToList();

                    foreach (var category in categories)
                    {
                        var CatIds = ctx.Categories.Where(x => x.Id == category.Id || x.ParentCategoryId == category.Id).Select(x => x.Id).ToList();
                        category.ProductCount = ctx.Products.Count(x => CatIds.Contains(x.Category_Id) && x.IsDeleted == false);
                    }

                    return Ok(new CustomResponse<SearchCategoryListViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new SearchCategoryListViewModel { Categories = categories } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User")]
        [HttpGet]
        [Route("SearchOffers")]
        public async Task<IHttpActionResult> SearchOffers(string OfferName, int? StoreId = null)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var query = "select Offers.*, Stores.Name as StoreName from Offers join Stores on Offers.Store_Id = Stores.Id where Offers.IsDeleted = 0 and Stores.IsDeleted = 0";


                    if (!String.IsNullOrEmpty(OfferName))
                        query += " And Offers.Name Like '%" + OfferName + "%'";

                    if (StoreId.HasValue && StoreId.Value != 0)
                        query += " And Offers.Store_Id = " + StoreId;

                    var offers = ctx.Database.SqlQuery<SearchOfferViewModel>(query).ToList();

                    return Ok(new CustomResponse<SearchOfferListViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new SearchOfferListViewModel { Offers = offers } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User")]
        [HttpGet]
        [Route("SearchPackages")]
        public async Task<IHttpActionResult> SearchPackages(string PackageName, int? StoreId)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var query = "select Packages.*, Stores.Name as StoreName from Packages join Stores on Packages.Store_Id = Stores.Id where Packages.IsDeleted = 0 and Stores.IsDeleted = 0";

                    if (!String.IsNullOrEmpty(PackageName))
                        query += " And Packages.Name Like '%" + PackageName + "%'";

                    if (StoreId.HasValue && StoreId.Value != 0)
                        query += " And Packages.Store_Id = " + StoreId;

                    var packages = ctx.Database.SqlQuery<SearchPackageViewModel>(query).ToList();
                    return Ok(new CustomResponse<SearchPackageListViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new SearchPackageListViewModel { Packages = packages } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [HttpGet]
        [Route("DeleteEntity")]
        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        public async Task<IHttpActionResult> DeleteEntity(int EntityType, int Id)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    switch (EntityType)
                    {
                        case (int)BasketEntityTypes.Product:
                            ctx.Products.FirstOrDefault(x => x.Id == Id).IsDeleted = true;
                            break;
                        case (int)BasketEntityTypes.Category:
                            ctx.Categories.FirstOrDefault(x => x.Id == Id).IsDeleted = true;
                            ctx.Database.ExecuteSqlCommand("update products set isdeleted = 1 where category_id = " + Id);
                            break;
                        case (int)BasketEntityTypes.Store:
                            ctx.Stores.FirstOrDefault(x => x.Id == Id).IsDeleted = true;
                            ctx.Database.ExecuteSqlCommand("update products set isdeleted = 1 where store_id = " + Id + @"; 
                            update packages set isdeleted = 1 where store_id = " + Id + @"; 
                            update categories set isdeleted = 1 where store_id = " + Id + @"; 
                            update offers set isdeleted = 1 where store_id = " + Id);
                            break;
                        case (int)BasketEntityTypes.Package:
                            ctx.Packages.FirstOrDefault(x => x.Id == Id).IsDeleted = true;
                            break;
                        case (int)BasketEntityTypes.Admin:
                            ctx.Admins.FirstOrDefault(x => x.Id == Id).IsDeleted = true;
                            break;
                        case (int)BasketEntityTypes.Offer:
                            ctx.Offers.FirstOrDefault(x => x.Id == Id).IsDeleted = true;
                            break;
                        case (int)BasketEntityTypes.Box:
                            ctx.Boxes.FirstOrDefault(x => x.Id == Id).IsDeleted = true;
                            ctx.Database.ExecuteSqlCommand("update BoxVideos set isdeleted = 1 where Box_Id = " + Id + @"; 
                            update UserSubscriptions set isdeleted = 1 where Box_Id = " + Id);
                            break;
                        default:
                            break;
                    }
                    ctx.SaveChanges();
                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [HttpGet]
        [Route("GetReadyForDeliveryOrders")]
        public async Task<IHttpActionResult> GetReadyForDeliveryOrders()
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    #region query
                    var query = @"
select 
Orders.Id,
Orders.OrderDateTime as CreatedOn,
Orders.Total as OrderTotal,
Orders.DeliveryMan_Id as DeliveryManId,
Case When Orders.PaymentMethod = 0 Then 'Pending' Else 'Paid' End As PaymentStatus,
Stores.Name as StoreName,
Stores.Id as StoreId,
Stores.Location as StoreLocation,
Users.FullName as CustomerName
from Orders
join Users on Users.ID = Orders.User_ID
join StoreOrders on StoreOrders.Order_Id = Orders.Id
join Stores on Stores.Id = StoreOrders.Store_Id
where 
Orders.IsDeleted = 0
and Orders.Status = " + (int)OrderStatuses.ReadyForDelivery;
                    #endregion

                    SearchOrdersListViewModel responseModel = new SearchOrdersListViewModel { Orders = ctx.Database.SqlQuery<SearchOrdersViewModel>(query).ToList() };

                    foreach (var order in responseModel.Orders)
                    {
                        var deliveryMen = ctx.DeliveryMen.Where(x => x.Location.Distance(order.StoreLocation) < Global.NearbyStoreRadius).ToList();

                        foreach (var deliverer in deliveryMen)
                        {
                            order.DeliveryMen.Add(new DelivererOptionsViewModel { Id = deliverer.Id, Name = deliverer.FullName });
                        }
                    }

                    //If a deliverer is in radius any of store in order. That deliverer will be selected.

                    var duplicateOrders = responseModel.Orders.GroupBy(x => x.Id).Where(g => g.Count() > 1).Select(y => y.Key);

                    var DuplicateDeliveryMenUnion = responseModel.Orders.Where(x => duplicateOrders.Contains(x.Id)).SelectMany(x1 => x1.DeliveryMen).Distinct(new DelivererOptionsViewModel.Comparer()).ToList();

                    foreach (var order in responseModel.Orders.Where(x => duplicateOrders.Contains(x.Id)))
                    {
                        order.DeliveryMen = DuplicateDeliveryMenUnion;
                    }

                    return Ok(new CustomResponse<SearchOrdersListViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = responseModel });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [HttpPost]
        [Route("AssignOrdersToDeliverer")]
        public async Task<IHttpActionResult> AssignOrdersToDeliverer(SearchOrdersListViewModel model)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    foreach (var order in model.Orders)
                    {
                        var existingOrder = ctx.Orders.Include(x => x.StoreOrders).FirstOrDefault(x => x.Id == order.Id);
                        if (existingOrder != null)
                        {
                            foreach (var storeOrder in existingOrder.StoreOrders)
                            {
                                storeOrder.Status = (int)OrderStatuses.AssignedToDeliverer;
                            }
                            existingOrder.DeliveryMan_Id = order.DeliveryManId;
                            existingOrder.Status = (int)OrderStatuses.AssignedToDeliverer;
                        }
                    }
                    ctx.SaveChanges();
                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [HttpGet]
        [Route("SearchOrders")]
        public async Task<IHttpActionResult> SearchOrders(string StartDate, string EndDate, int? OrderStatusId, int? PaymentMethodId, int? PaymentStatusId, int? StoreId)
        {
            try
            {
                DateTime startDateTime;
                DateTime endDateTime;
                startDateTime = DateTime.ParseExact(StartDate, "d/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                endDateTime = DateTime.ParseExact(EndDate, "d/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                #region query
                var query = @"
select 
Orders.Id,
StoreOrders.Id as StoreOrder_Id,
Orders.OrderDateTime as CreatedOn,
Orders.Total as OrderTotal,
StoreOrders.Status as OrderStatus,
Orders.DeliveryMan_Id as DeliveryManId,
Case When Orders.PaymentStatus = 0 Then 'Pending' Else 'Paid' End As PaymentStatus,
Stores.Name as StoreName,
Stores.Id as StoreId,
Stores.Location as StoreLocation,
Users.FullName as CustomerName
from Orders
join Users on Users.ID = Orders.User_ID
join StoreOrders on StoreOrders.Order_Id = Orders.Id
join Stores on Stores.Id = StoreOrders.Store_Id
where 
Orders.IsDeleted = 0
and 
 CAST(orders.OrderDateTime AS DATE) >= '" + startDateTime.Date + "' and CAST(orders.OrderDateTime as DATE) <= '" + endDateTime.Date + "'";
                #endregion

                if (OrderStatusId.HasValue)
                    query += " and orders.Status = " + OrderStatusId.Value;

                if (PaymentMethodId.HasValue)
                    query += " and orders.PaymentMethod = " + PaymentMethodId.Value;

                if (PaymentStatusId.HasValue)
                    query += " and orders.PaymentStatus = " + PaymentStatusId.Value;

                if (StoreId.HasValue)
                    query += " and Stores.Id = " + StoreId.Value;

                SearchOrdersListViewModel returnModel = new SearchOrdersListViewModel();

                using (RiscoContext ctx = new RiscoContext())
                {
                    returnModel.Orders = ctx.Database.SqlQuery<SearchOrdersViewModel>(query).ToList();

                    foreach (var order in returnModel.Orders)
                    {
                        order.OrderStatusName = Utility.GetOrderStatusName(order.OrderStatus);
                        order.OrderItemsComaSep = string.Join(",", (from x in ctx.Order_Items.Where(x => x.StoreOrder_Id == order.Id) select x.Name).ToArray());
                    }
                    return Ok(new CustomResponse<SearchOrdersListViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = returnModel });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [HttpPost]
        [Route("ChangeOrderStatus")]
        public async Task<IHttpActionResult> ChangeOrderStatus(ChangeOrderStatusListBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using (RiscoContext ctx = new RiscoContext())
                {
                    //Mark Statuses for StoreOrders
                    foreach (var order in model.Orders)
                    {
                        var existingStoreOrder = ctx.StoreOrders.Include(x => x.Order.User.UserDevices).FirstOrDefault(x => x.Id == order.StoreOrder_Id);
                        if (existingStoreOrder != null)
                        {
                            if (order.Status > existingStoreOrder.Status)
                            {
                                PushNotificationType pushType = PushNotificationType.Announcement;
                                Notification Notification = new Notification();
                                if (order.Status == (int)OrderStatuses.Accepted)
                                {
                                    Notification.Title = "Order Accepted";
                                    Notification.Text = "Your order# " + existingStoreOrder.Order.Id + " has been accepted by store.";
                                    pushType = PushNotificationType.OrderAccepted;
                                    existingStoreOrder.Order.User.Notifications.Add(Notification);
                                }
                                else if (order.Status == (int)OrderStatuses.AssignedToDeliverer)
                                {
                                    Notification.Title = "Order Assigned To Delivery Boy";
                                    Notification.Text = "Your order#" + existingStoreOrder.Order.Id + " has been assigned to delivery boy.";
                                    pushType = PushNotificationType.OrderAssignedToDeliverer;
                                    existingStoreOrder.Order.User.Notifications.Add(Notification);
                                }
                                else if (order.Status == (int)OrderStatuses.Dispatched)
                                {
                                    Notification.Title = "Order Dispatched";
                                    Notification.Text = "Your order#" + existingStoreOrder.Order.Id + " has been dispatched.";
                                    pushType = PushNotificationType.OrderDispatched;
                                    existingStoreOrder.Order.User.Notifications.Add(Notification);
                                }
                                else if (order.Status == (int)OrderStatuses.Completed)
                                {
                                    Notification.Title = "Order Completed";
                                    Notification.Text = "Your order#" + existingStoreOrder.Order.Id + " has been marked as completed.";
                                    pushType = PushNotificationType.OrderCompleted;
                                    existingStoreOrder.Order.User.Notifications.Add(Notification);
                                    existingStoreOrder.Order.PaymentStatus = (int)PaymentStatus.Paid;
                                }
                                else if (order.Status == (int)OrderStatuses.Rejected)
                                {
                                    Notification.Title = "Order Rejected";
                                    Notification.Text = "Your order#" + existingStoreOrder.Order.Id + " has been rejected by store, we are very sorry for inconvenience.";
                                    pushType = PushNotificationType.OrderRejected;
                                    existingStoreOrder.Order.User.Notifications.Add(Notification);
                                }
                                existingStoreOrder.Status = order.Status;
                                ctx.SaveChanges();
                                if (existingStoreOrder.Order.User.IsNotificationsOn)
                                {
                                    var usersToPushAndroid = existingStoreOrder.Order.User.UserDevices.Where(x => x.Platform == true).ToList();
                                    var usersToPushIOS = existingStoreOrder.Order.User.UserDevices.Where(x => x.Platform == false).ToList();
                                    Utility.SendPushNotifications(usersToPushAndroid, usersToPushIOS, Notification, (int)pushType);
                                }
                            }
                        }
                    }
                    //Mark Statuses for Orders
                    foreach (var order in model.Orders)
                    {
                        var existingOrder = ctx.Orders.Include(x => x.StoreOrders).FirstOrDefault(x => x.Id == order.OrderId);
                        existingOrder.Status = existingOrder.StoreOrders.Min(x => x.Status);
                    }

                    ctx.SaveChanges();
                }
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [Route("AddPackage")]
        public async Task<IHttpActionResult> AddPackage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                Package model = new Package();
                Package existingPackage = new Package();

                if (httpRequest.Params["Id"] != null)
                {
                    model.Id = Convert.ToInt32(httpRequest.Params["Id"]);
                }

                if (httpRequest.Params["ImageDeletedOnEdit"] != null)
                {
                    model.ImageDeletedOnEdit = Convert.ToBoolean(httpRequest.Params["ImageDeletedOnEdit"]);
                }
                model.Name = httpRequest.Params["Name"];
                model.Price = Convert.ToDouble(httpRequest.Params["Price"]);
                model.Description = httpRequest.Params["Description"];
                model.Store_Id = Convert.ToInt32(httpRequest.Params["Store_Id"]);
                model.Status = 0;
                if (httpRequest.Params["package_products"] != null)
                {
                    var packageProducts = JsonConvert.DeserializeObject<List<Package_Products>>(httpRequest.Params["package_products"]);

                    if (model.Id > 0)
                    {
                        foreach (var item in packageProducts)
                        {
                            //item.Product_Id = item.Id;
                            item.Id = item.PackageProductId;

                        }
                    }
                    else
                    {
                        foreach (var item in packageProducts)
                            item.Product_Id = item.Id;
                    }
                    model.Package_Products = packageProducts;
                }

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
                        Result = new Error { ErrorMessage = "Multipart data is not included in request" }
                    });
                }
                else if (httpRequest.Files.Count > 1)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multiple images are not supported, please upload one image" }
                    });
                }
                #endregion

                using (RiscoContext ctx = new RiscoContext())
                {
                    if (model.Id == 0)
                    {
                        if (ctx.Packages.Any(x => x.Store_Id == model.Store_Id && x.Name == model.Name && x.IsDeleted == false))
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "Conflict",
                                StatusCode = (int)HttpStatusCode.Conflict,
                                Result = new Error { ErrorMessage = "Package already exist under same store" }
                            });
                        }
                    }
                    else
                    {
                        existingPackage = ctx.Packages.Include(x => x.Package_Products).FirstOrDefault(x => x.Id == model.Id);
                        if (existingPackage.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase) == false || existingPackage.Store_Id != model.Store_Id)
                        {
                            if (ctx.Packages.Any(x => x.IsDeleted == false && x.Store_Id == model.Store_Id && x.Name.Equals(model.Name.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "Conflict",
                                    StatusCode = (int)HttpStatusCode.Conflict,
                                    Result = new Error { ErrorMessage = "Package with same name already exist under same store" }
                                });
                            }
                        }
                    }

                    HttpPostedFile postedFile = null;
                    string fileExtension = string.Empty;

                    #region ImageSaving
                    if (httpRequest.Files.Count > 0)
                    {
                        postedFile = httpRequest.Files[0];
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
                                    Result = new Error { ErrorMessage = "Please Upload image of type .jpg,.gif,.png" }
                                });
                            }
                            else if (postedFile.ContentLength > Global.MaximumImageSize)
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload a file upto " + Global.ImageSize }
                                });
                            }
                            else
                            {
                                //int count = 1;
                                //fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
                                //newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["ProductImageFolderPath"] + postedFile.FileName);

                                //while (File.Exists(newFullPath))
                                //{
                                //    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                //    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["ProductImageFolderPath"] + tempFileName + fileExtension);
                                //}
                                //postedFile.SaveAs(newFullPath);
                            }
                        }
                        //model.ImageUrl = ConfigurationManager.AppSettings["ProductImageFolderPath"] + Path.GetFileName(newFullPath);
                    }
                    #endregion

                    if (model.Id == 0)
                    {
                        ctx.Packages.Add(model);
                        ctx.SaveChanges();
                        var guid = Guid.NewGuid();
                        newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["PackageImageFolderPath"] + model.Id + "_" + guid + fileExtension);
                        postedFile.SaveAs(newFullPath);
                        model.ImageUrl = ConfigurationManager.AppSettings["PackageImageFolderPath"] + model.Id + "_" + guid + fileExtension;
                        ctx.SaveChanges();
                    }
                    else
                    {
                        //existingProduct = ctx.Products.FirstOrDefault(x => x.Id == model.Id);
                        if (httpRequest.Files.Count == 0)
                        {
                            // Check if image deleted
                            if (model.ImageDeletedOnEdit == false)
                            {
                                model.ImageUrl = existingPackage.ImageUrl;
                            }
                        }
                        else
                        {
                            Utility.DeleteFileIfExists(existingPackage.ImageUrl);
                            var guid = Guid.NewGuid();
                            newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["PackageImageFolderPath"] + model.Id + "_" + guid + fileExtension);
                            postedFile.SaveAs(newFullPath);
                            model.ImageUrl = ConfigurationManager.AppSettings["PackageImageFolderPath"] + model.Id + "_" + guid + fileExtension;
                        }

                        ctx.Entry(existingPackage).CurrentValues.SetValues(model);

                        foreach (var oldPP in existingPackage.Package_Products.ToList())
                        {
                            ctx.Package_Products.Remove(oldPP);
                        }

                        foreach (var packageProduct in model.Package_Products)
                        {
                            packageProduct.Package_Id = existingPackage.Id;
                            existingPackage.Package_Products.Add(packageProduct);

                            #region commented

                            //var originalPackageProduct = existingPackage.Package_Products.Where(c => c.Id == packageProduct.Id).SingleOrDefault();

                            //if (originalPackageProduct != null)
                            //{
                            //    // Yes -> Update scalar properties of child item
                            //    packageProduct.Package_Id = originalPackageProduct.Package_Id;
                            //    ctx.Entry(originalPackageProduct).CurrentValues.SetValues(packageProduct);
                            //}
                            //else
                            //{
                            //    // No -> It's a new child item -> Insert
                            //    packageProduct.Package_Id = existingPackage.Id;
                            //    existingPackage.Package_Products.Add(packageProduct);
                            //} 
                            #endregion
                        }
                        ctx.SaveChanges();
                    }

                    CustomResponse<Package> response = new CustomResponse<Package>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = model
                    };

                    return Ok(response);

                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [Route("AddOffer")]
        public async Task<IHttpActionResult> AddOffer()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                Offer model = new Offer();
                Offer existingOffer = new Offer();

                if (httpRequest.Params["Id"] != null)
                {
                    model.Id = Convert.ToInt32(httpRequest.Params["Id"]);
                }

                if (httpRequest.Params["ImageDeletedOnEdit"] != null)
                {
                    model.ImageDeletedOnEdit = Convert.ToBoolean(httpRequest.Params["ImageDeletedOnEdit"]);
                }
                model.Name = httpRequest.Params["Name"];
                model.ValidFrom = DateTime.ParseExact(httpRequest.Params["ValidFrom"], "dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
                model.ValidUpto = DateTime.ParseExact(httpRequest.Params["ValidTo"], "dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
                //model.Price = Convert.ToDouble(httpRequest.Params["Price"]);
                model.Description = httpRequest.Params["Description"];
                model.Store_Id = Convert.ToInt32(httpRequest.Params["Store_Id"]);
                model.Status = 0;
                if (httpRequest.Params["offer_products"] != null)
                {
                    var offerProducts = JsonConvert.DeserializeObject<List<Offer_Products>>(httpRequest.Params["offer_products"]);

                    if (model.Id > 0)
                    {
                        foreach (var item in offerProducts)
                        {
                            //item.Product_Id = item.Id;
                            item.Id = item.OfferProductId;

                        }
                    }
                    else
                    {
                        foreach (var item in offerProducts)
                            item.Product_Id = item.Id;
                    }
                    model.Offer_Products = offerProducts;
                }

                if (httpRequest.Params["offer_packages"] != null)
                {
                    var offerPackages = JsonConvert.DeserializeObject<List<Offer_Packages>>(httpRequest.Params["offer_packages"]);

                    if (model.Id > 0)
                    {
                        foreach (var item in offerPackages)
                        {
                            //item.Product_Id = item.Id;
                            item.Id = item.OfferPackageId;

                        }
                    }
                    else
                    {
                        foreach (var item in offerPackages)
                            item.Package_Id = item.Id;
                    }
                    model.Offer_Packages = offerPackages;
                }
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
                        Result = new Error { ErrorMessage = "Multipart data is not included in request" }
                    });
                }
                else if (httpRequest.Files.Count > 1)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multiple images are not supported, please upload one image" }
                    });
                }
                #endregion

                using (RiscoContext ctx = new RiscoContext())
                {
                    if (model.Id == 0)
                    {
                        if (ctx.Offers.Any(x => x.Store_Id == model.Store_Id && x.Name == model.Name && x.IsDeleted == false))
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "Conflict",
                                StatusCode = (int)HttpStatusCode.Conflict,
                                Result = new Error { ErrorMessage = "Offer with same name already exist under same store" }
                            });
                        }
                    }
                    else
                    {
                        existingOffer = ctx.Offers.Include(x => x.Offer_Products).Include(x => x.Offer_Packages).FirstOrDefault(x => x.Id == model.Id);
                        if (existingOffer.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase) == false || existingOffer.Store_Id != model.Store_Id)
                        {
                            if (ctx.Offers.Any(x => x.IsDeleted == false && x.Store_Id == model.Store_Id && x.Name.Equals(model.Name.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "Conflict",
                                    StatusCode = (int)HttpStatusCode.Conflict,
                                    Result = new Error { ErrorMessage = "Offer with same name already exist under same store" }
                                });
                            }
                        }
                    }

                    HttpPostedFile postedFile = null;
                    string fileExtension = string.Empty;

                    #region ImageSaving
                    if (httpRequest.Files.Count > 0)
                    {
                        postedFile = httpRequest.Files[0];
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
                                    Result = new Error { ErrorMessage = "Please Upload image of type .jpg,.gif,.png" }
                                });
                            }
                            else if (postedFile.ContentLength > Global.MaximumImageSize)
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload a file upto " + Global.ImageSize }
                                });
                            }
                            else
                            {
                                //int count = 1;
                                //fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
                                //newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["ProductImageFolderPath"] + postedFile.FileName);

                                //while (File.Exists(newFullPath))
                                //{
                                //    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                //    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["ProductImageFolderPath"] + tempFileName + fileExtension);
                                //}
                                //postedFile.SaveAs(newFullPath);
                            }
                            //model.ImageUrl = ConfigurationManager.AppSettings["ProductImageFolderPath"] + Path.GetFileName(newFullPath);
                        }
                    }
                    #endregion

                    if (model.Id == 0)
                    {
                        ctx.Offers.Add(model);
                        ctx.SaveChanges();
                        var guid = Guid.NewGuid();
                        newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["OfferImageFolderPath"] + model.Id + "_" + guid + fileExtension);
                        postedFile.SaveAs(newFullPath);
                        model.ImageUrl = ConfigurationManager.AppSettings["OfferImageFolderPath"] + model.Id + "_" + guid + fileExtension;
                        ctx.SaveChanges();
                    }
                    else
                    {
                        //existingProduct = ctx.Products.FirstOrDefault(x => x.Id == model.Id);
                        if (httpRequest.Files.Count == 0)
                        {
                            // Check if image deleted
                            if (model.ImageDeletedOnEdit == false)
                            {
                                model.ImageUrl = existingOffer.ImageUrl;
                            }
                        }
                        else
                        {
                            Utility.DeleteFileIfExists(existingOffer.ImageUrl);
                            var guid = Guid.NewGuid();
                            newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["OfferImageFolderPath"] + model.Id + "_" + guid + fileExtension);
                            postedFile.SaveAs(newFullPath);
                            model.ImageUrl = ConfigurationManager.AppSettings["OfferImageFolderPath"] + model.Id + "_" + guid + fileExtension;
                        }

                        ctx.Entry(existingOffer).CurrentValues.SetValues(model);

                        //Delete and Insert OfferProducts

                        foreach (var oldOP in existingOffer.Offer_Products)
                        {
                            oldOP.IsDeleted = true;
                        }

                        foreach (var newOP in model.Offer_Products)
                        {
                            var oldOP = ctx.Offer_Products.FirstOrDefault(x => x.Id == newOP.Id);
                            if (oldOP == null)
                            {
                                newOP.Offer_Id = existingOffer.Id;
                                existingOffer.Offer_Products.Add(newOP);
                            }
                            else
                            {
                                newOP.Offer_Id = existingOffer.Id;
                                ctx.Entry(ctx.Offer_Products.FirstOrDefault(x => x.Id == oldOP.Id)).CurrentValues.SetValues(newOP);
                            }
                        }

                        //Delete and Insert OfferPackages
                        foreach (var oldOP in existingOffer.Offer_Packages)
                        {
                            oldOP.IsDeleted = true;
                        }

                        foreach (var newOP in model.Offer_Packages)
                        {
                            var oldOP = ctx.Offer_Packages.FirstOrDefault(x => x.Id == newOP.Id);
                            if (oldOP == null)
                            {
                                newOP.Offer_Id = existingOffer.Id;
                                existingOffer.Offer_Packages.Add(newOP);
                            }
                            else
                            {
                                newOP.Offer_Id = existingOffer.Id;
                                ctx.Entry(ctx.Offer_Packages.FirstOrDefault(x => x.Id == oldOP.Id)).CurrentValues.SetValues(newOP);
                            }
                        }
                        ctx.SaveChanges();
                    }



                    CustomResponse<Offer> response = new CustomResponse<Offer>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = model
                    };

                    return Ok(response);

                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(AdminSetPasswordBindingModel model)
        {
            try
            {
                var userEmail = User.Identity.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    throw new Exception("User Email is empty in user.identity.name.");
                }
                else if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using (RiscoContext ctx = new RiscoContext())
                {
                    var hashedPassword = CryptoHelper.Hash(model.OldPassword);
                    var user = ctx.Admins.FirstOrDefault(x => x.Email == userEmail && x.Password == hashedPassword);
                    if (user != null)
                    {
                        user.Password = CryptoHelper.Hash(model.NewPassword);
                        ctx.SaveChanges();
                        return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                    }
                    else
                        return Ok(new CustomResponse<Error> { Message = "Forbidden", StatusCode = (int)HttpStatusCode.Forbidden, Result = new Error { ErrorMessage = "Invalid old password." } });


                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [Route("AddBox")]
        public async Task<IHttpActionResult> AddBox(AddBoxBindingModel model)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    if (model.Id > 0)
                    {
                        var existingBox = ctx.Boxes.Include(x => x.BoxVideos).FirstOrDefault(x => x.Id == model.Id);
                        if (model.IntroUrl != null)
                        {
                            model.IntroUrlThumbnail = Youtube.GetThumbnailUrl(model.IntroUrl);
                        }

                        ctx.Entry(existingBox).CurrentValues.SetValues(model);

                        foreach (var oldPP in existingBox.BoxVideos.ToList())
                        {
                            ctx.BoxVideos.Remove(oldPP);
                        }

                        foreach (var boxVideo in model.BoxVideos)
                        {
                            existingBox.BoxVideos.Add(new BoxVideos { VideoUrl = boxVideo.VideoUrl, ThumbnailUrl = Youtube.GetThumbnailUrl(boxVideo.VideoUrl),Title= boxVideo.Title,Description=boxVideo.Description });
                        } 
                        ctx.SaveChanges();
                        return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                    }
                    else
                    {
                        if (ctx.Boxes.Any(x => x.BoxCategory_Id == model.BoxCategory_Id && x.ReleaseDate.Month == model.ReleaseDate.Month && x.ReleaseDate.Year == model.ReleaseDate.Year && x.IsDeleted == false))
                        {
                            return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Conflict, StatusCode = (int)HttpStatusCode.Conflict, Result = new Error { ErrorMessage = "Box with same type for the same month already exists." } });
                        }

                        TimeZoneInfo UAETimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time"); DateTime utc = DateTime.UtcNow;
                        DateTime UAE = TimeZoneInfo.ConvertTimeFromUtc(utc, UAETimeZone);

                        Box box = new Box { Name = model.Name, Price = model.Price, IntroUrl = model.IntroUrl, IntroUrlThumbnail = Youtube.GetThumbnailUrl(model.IntroUrl), CreatedDate = UAE, ReleaseDate = model.ReleaseDate, BoxCategory_Id = model.BoxCategory_Id, Status = 1, Description = model.Description };

                        foreach (var video in model.BoxVideos)
                        {
                            box.BoxVideos.Add(new BoxVideos { VideoUrl = video.VideoUrl, ThumbnailUrl = Youtube.GetThumbnailUrl(video.VideoUrl),Title=video.Title,Description =video.Description });
                        }

                        ctx.Boxes.Add(box);
                        ctx.SaveChanges();

                        return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [Route("GetAllBoxes")]
        public async Task<IHttpActionResult> GetAllBoxes()
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    return Ok(new CustomResponse<SearchBoxesViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new SearchBoxesViewModel { Boxes = ctx.Boxes.Where(x => x.IsDeleted == false).ToList() }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [Route("AddNotification")]
        public async Task<IHttpActionResult> AddNotification(NotificationBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                TimeZoneInfo UAETimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time"); DateTime utc = DateTime.UtcNow;
                DateTime UAE = TimeZoneInfo.ConvertTimeFromUtc(utc, UAETimeZone);

                using (RiscoContext ctx = new RiscoContext())
                {
                    AdminNotifications adminNotification = new AdminNotifications { CreatedDate = UAE, Title = model.Title, TargetAudienceType = model.TargetAudience, Description = model.Description };

                    ctx.AdminNotifications.Add(adminNotification);
                    ctx.SaveChanges();
                    if (model.TargetAudience == (int)NotificationTargetAudienceTypes.User || model.TargetAudience == (int)NotificationTargetAudienceTypes.UserAndDeliverer)
                    {
                        var users = ctx.Users.Where(x => x.IsDeleted == false).Include(x => x.UserDevices).Where(x => x.IsDeleted == false);

                        await users.ForEachAsync(a => a.Notifications.Add(new Notification { Title = model.Title, Text = model.Description, Status = 0, AdminNotification_Id = adminNotification.Id }));

                        await ctx.SaveChangesAsync();

                        var usersToPushAndroid = users.Where(x => x.IsNotificationsOn).SelectMany(x => x.UserDevices.Where(x1 => x1.Platform == true)).ToList();
                        var usersToPushIOS = users.Where(x => x.IsNotificationsOn).SelectMany(x => x.UserDevices.Where(x1 => x1.Platform == false)).ToList();

                        HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                        {
                            Global.objPushNotifications.SendAndroidPushNotification(usersToPushAndroid, adminNotification);
                            Global.objPushNotifications.SendIOSPushNotification(usersToPushIOS, adminNotification);

                        });

                    }

                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [HttpGet]
        [Route("SearchNotifications")]
        public async Task<IHttpActionResult> SearchNotifications()
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    return Ok(new CustomResponse<SearchAdminNotificationsViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new SearchAdminNotificationsViewModel
                        {
                            Notifications = ctx.AdminNotifications.ToList()
                        }
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
        [Route("ChangeUserStatuses")]
        public async Task<IHttpActionResult> ChangeUserStatuses(ChangeUserStatusListBindingModel model)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    foreach (var user in model.Users)
                        ctx.Users.FirstOrDefault(x => x.Id == user.UserId).IsDeleted = user.Status;

                    ctx.SaveChanges();
                }

                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [HttpPost]
        [Route("ChangeBoxStatuses")]
        public async Task<IHttpActionResult> ChangeBoxStatuses(ChangeBoxStatusListBindingModel model)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    foreach (var box in model.Boxes)
                        ctx.Boxes.FirstOrDefault(x => x.Id == box.BoxId).Status = box.Status;
                    ctx.SaveChanges();
                }

                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }



        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [HttpGet]
        [Route("GetUsers")]
        public async Task<IHttpActionResult> GetUsers()
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    return Ok(new CustomResponse<SearchUsersViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new SearchUsersViewModel
                        {
                            Users = ctx.Users.ToList()
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        //[BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]

        [HttpGet]
        [Route("GetUser")]
        public async Task<IHttpActionResult> GetUser(int UserId, int SignInType)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    BasketSettings.LoadSettings();

                    if (SignInType == (int)RoleTypes.User)
                    {
                        var user = ctx.Users.Include(x => x.Notifications.Select(x1 => x1.AdminNotification)).Include(x => x.Orders).Include(x => x.UserSubscriptions.Select(y => y.Box)).Include(x => x.Favourites.Select(y => y.Product)).Include(x => x.Feedback.Select(z => z.Store)).Include(x => x.UserAddresses).Include(x => x.PaymentCards).FirstOrDefault(x => x.Id == UserId);
                        return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = user });
                    }
                    else
                    {
                        var Deliverer = ctx.DeliveryMen.Include(x => x.DelivererAddresses).FirstOrDefault(x => x.Id == UserId);
                        Deliverer.SignInType = (int)RoleTypes.Deliverer;
                        return Ok(new CustomResponse<DeliveryMan> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = Deliverer });
                    }

                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Route("GetSalesGraphData")]
        public async Task<IHttpActionResult> GetSalesGraphData()
        {
            try
            {
                var query = "select Sum(Total) as Total, CAST(OrderDateTime AS DATE) as OrderDateTime from Orders where IsDeleted = 0 group by CAST(OrderDateTime AS DATE) order by CAST(OrderDateTime AS DATE) desc";

                using (RiscoContext ctx = new RiscoContext())
                {
                    return Ok(new CustomResponse<ListOrderSalesGraph>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new ListOrderSalesGraph
                        {
                            Orders = ctx.Database.SqlQuery<OrdersSalesGraph>(query).ToList()
                        }
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
        [Route("SendNotificationToUser")]
        public async Task<IHttpActionResult> SendNotificationToUser(SendNotificationToUserBindingModel model)
        {
            try
            {
                TimeZoneInfo UAETimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time"); DateTime utc = DateTime.UtcNow;
                DateTime UAE = TimeZoneInfo.ConvertTimeFromUtc(utc, UAETimeZone);

                AdminNotifications adminNotification = new AdminNotifications
                {
                    CreatedDate = UAE,
                    TargetAudienceType = (int)NotificationTargetAudienceTypes.IndividualUser,
                    Title = model.Title,
                    Description = model.Text
                };

                using (RiscoContext ctx = new RiscoContext())
                {
                    ctx.AdminNotifications.Add(adminNotification);
                    ctx.SaveChanges();

                    Notification Notification = new Notification
                    {
                        AdminNotification_Id = adminNotification.Id,
                        Title = model.Title,
                        Text = model.Text,
                        User_ID = model.User_Id
                    };

                    ctx.Notifications.Add(Notification);
                    ctx.SaveChanges();

                    var users = ctx.Users.Include(x => x.UserDevices).Where(x => x.IsNotificationsOn && x.IsDeleted == false && x.Id == model.User_Id).ToList();

                    var androidDevices = users.Where(x => x.IsNotificationsOn).SelectMany(x => x.UserDevices.Where(x1 => x1.Platform == true)).ToList();
                    var iosDevices = users.Where(x => x.IsNotificationsOn).SelectMany(x => x.UserDevices.Where(x1 => x1.Platform == false)).ToList();
                    if (androidDevices.Count > 0 || iosDevices.Count > 0)
                    {
                        HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                        {
                            Global.objPushNotifications.SendAndroidPushNotification(androidDevices, adminNotification);
                            Global.objPushNotifications.SendIOSPushNotification(iosDevices, adminNotification);
                        });
                    }
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
        [Route("AddUpdateFCMToken")]
        public async Task<IHttpActionResult> AddUpdateFCMToken(FCMTokenModel model)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                using (RiscoContext ctx = new RiscoContext())
                {
                    var adminToken = ctx.AdminTokens.FirstOrDefault(x => x.Admin_Id == userId && x.Token == model.Token);

                    if (adminToken != null)
                        adminToken.IsActive = true;
                    else
                        ctx.AdminTokens.Add(new AdminTokens { Admin_Id = userId, Token = model.Token, IsActive = true });

                    ctx.SaveChanges();

                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("TestNotificationToAdmin")]
        public async Task<IHttpActionResult> TestNotificationToAdmin(string Text)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var adminsToPush = ctx.AdminTokens.ToList();
                    var orderUrl = ConfigurationManager.AppSettings["WebsiteBaseUrl"] + "Dashboard/Orders?OrderId=2204";

                    HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        Global.objPushNotifications.SendWebGCMPushNotification(adminsToPush, "New Order Received! #2204", "Brush Type A, WaterColor, BrushType D", orderUrl);
                    });
                }

                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin")]
        [HttpGet]
        [Route("GetOrderCount")]
        public async Task<IHttpActionResult> GetOrderCount()
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    int orderCount = 0;
                    var TodayDate = DateTime.Now;
                    orderCount = ctx.Orders.Count(x => DbFunctions.TruncateTime(x.OrderDateTime) == TodayDate.Date);

                    return Ok(new CustomResponse<OrderCountViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new OrderCountViewModel
                        {
                            Count = orderCount
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }
    }
}
