using BasketApi;
using BasketApi.Areas.SubAdmin.Models;
using BasketApi.ViewModels;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApplication1.Areas.Admin.Controllers
{
    [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User", "Guest")]
    [RoutePrefix("api")]
    public class CategoryController : ApiController
    {
        [Route("GetAllCategories")]
        public async Task<IHttpActionResult> GetAllCategories(bool IsAdmin = false)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    List<CategoryViewModelAnonymous> categories;
                    if (IsAdmin)
                    {
                        categories = ctx.Categories.Where(x => x.IsDeleted == false)
                                .Select(cat => new CategoryViewModelAnonymous
                                {
                                    Id = cat.Id,
                                    ImageUrl = cat.ImageUrl,
                                    Name = cat.Name,
                                    Store_Id = cat.Store_Id,
                                    Description = cat.Description,
                                    ParentCategoryId = cat.ParentCategoryId,
                                    IsDeleted = cat.IsDeleted,
                                    ProductCount = cat.Products.Count(x2 => x2.IsDeleted == false),
                                    HasSubCategories = ctx.Categories.Any(x1 => x1.ParentCategoryId == cat.Id && x1.IsDeleted == false)
                                })
                                .OrderBy(x => x.Name).ToList();
                    }
                    else
                    {
                        categories = ctx.Categories.Where(x => x.IsDeleted == false && x.ParentCategoryId == 0)
                              .Select(cat => new CategoryViewModelAnonymous
                              {
                                  Id = cat.Id,
                                  ImageUrl = cat.ImageUrl,
                                  Name = cat.Name,
                                  Store_Id = cat.Store_Id,
                                  Description = cat.Description,
                                  ParentCategoryId = cat.ParentCategoryId,
                                  IsDeleted = cat.IsDeleted,
                                  HasSubCategories = ctx.Categories.Any(x1 => x1.ParentCategoryId == cat.Id && x1.IsDeleted == false)
                              })
                              .OrderBy(x => x.Name).ToList();

                        foreach (var category in categories)
                        {
                            var CatIds = ctx.Categories.Where(x => x.Id == category.Id || x.ParentCategoryId == category.Id).Select(x => x.Id).ToList();
                            category.ProductCount = ctx.Products.Count(x => CatIds.Contains(x.Category_Id) && x.IsDeleted == false);
                        }
                    }

                    CustomResponse<CategoriesViewModelAnonymous> response = new CustomResponse<CategoriesViewModelAnonymous>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new CategoriesViewModelAnonymous
                        {
                            Categories = categories
                        }
                    };
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Route("GetSubCategoriesByCatId")]
        public async Task<IHttpActionResult> GetSubCategoriesByCatId(int CatId)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var categories = ctx.Categories.Where(x => x.IsDeleted == false && x.ParentCategoryId == CatId)
                                .Select(cat => new CategoryViewModelAnonymous
                                {
                                    Id = cat.Id,
                                    ImageUrl = cat.ImageUrl,
                                    Name = cat.Name,
                                    Store_Id = cat.Store_Id,
                                    Description = cat.Description,
                                    ParentCategoryId = cat.ParentCategoryId,
                                    IsDeleted = cat.IsDeleted,
                                    HasSubCategories = ctx.Categories.Any(x1 => x1.ParentCategoryId == cat.Id && x1.IsDeleted == false)
                                })
                                .OrderBy(x => x.Name).ToList();


                    foreach (var category in categories)
                    {
                        var CatIds = ctx.Categories.Where(x => x.Id == category.Id || x.ParentCategoryId == category.Id).Select(x => x.Id).ToList();
                        category.ProductCount = ctx.Products.Count(x => CatIds.Contains(x.Category_Id) && x.IsDeleted == false);
                    }

                    var CatIdsParent = ctx.Categories.Where(x => x.Id == CatId || x.ParentCategoryId == CatId).Select(x => x.Id).ToList();

                    categories.Insert(0, new CategoryViewModelAnonymous
                    {
                        Name = "All",
                        Id = CatId,
                        ImageUrl = ctx.Categories.FirstOrDefault(x => x.Id == CatId).ImageUrl,
                        ProductCount = ctx.Products.Count(x => CatIdsParent.Contains(x.Category_Id) && x.IsDeleted == false)
                    });

                    CustomResponse<CategoriesViewModelAnonymous> response = new CustomResponse<CategoriesViewModelAnonymous>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new CategoriesViewModelAnonymous
                        {
                            Categories = categories
                        }
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        #region OldMethod
        //[Route("GetSubCategoriesByCatId")]
        //public async Task<IHttpActionResult> GetSubCategoriesByCatId(int CatId)
        //{
        //    try
        //    {
        //        using (SkriblContext ctx = new SkriblContext())
        //        {
        //            var categories = ctx.Categories.Where(x => x.ParentCategoryId == CatId && x.IsDeleted == false).OrderBy(x => x.Name).ToList();
        //            categories.Insert(0, new Category { Name = "All", Id = CatId });
        //            CustomResponse<CategoriesViewModel> response = new CustomResponse<CategoriesViewModel>
        //            {
        //                Message = Global.ResponseMessages.Success,
        //                StatusCode = (int)HttpStatusCode.OK,
        //                Result = new CategoriesViewModel { Categories = categories }
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
    }
}
