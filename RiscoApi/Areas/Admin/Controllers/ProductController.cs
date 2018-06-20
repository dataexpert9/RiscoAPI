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

namespace BasketApi.Areas.SubAdmin.Controllers
{
    [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User", "Guest")]
    [RoutePrefix("api")]
    public class ProductController : ApiController
    {
        [HttpGet]
        [Route("GetProductsByCategoryId")]
        public async Task<IHttpActionResult> GetProductsByCategoryId(int CatId, int UserId, int PageSize, int PageNo, string filterTypes = "", bool IsAll = false)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userFavourites = ctx.Favourites.Where(x => x.User_ID == UserId && x.IsDeleted == false).ToList();
                    List<Product> products;
                    int TotalCount;

                    if (IsAll)
                    {
                        var CatIds = ctx.Categories.Where(x => x.Id == CatId || x.ParentCategoryId == CatId).Select(x => x.Id).ToList();
                        TotalCount = ctx.Products.Count(x => CatIds.Contains(x.Category_Id) && x.IsDeleted == false);
                        products = ctx.Products.Include(x => x.ProductImages).Where(x => CatIds.Contains(x.Category_Id) && x.IsDeleted == false).OrderByDescending(x => x.Id).Page(PageSize, PageNo).ToList();
                    }
                    else
                    {
                        TotalCount = ctx.Products.Count(x => x.Category_Id == CatId && x.IsDeleted == false);
                        products = ctx.Products.Include(x => x.ProductImages).Where(x => x.Category_Id == CatId && x.IsDeleted == false).OrderByDescending(x => x.Id).Page(PageSize, PageNo).ToList();
                    }

                    foreach (var product in products)
                    {
                        product.Weight = Convert.ToString(product.WeightInGrams) + " gm";

                        if (userFavourites.Any(x => x.Product_Id == product.Id))
                            product.IsFavourite = true;
                        else
                            product.IsFavourite = false;
                    }

                    return Ok(new CustomResponse<ProductsViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new ProductsViewModel
                        {
                            Count = TotalCount,
                            Products = products
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("GetProductCount")]
        public async Task<IHttpActionResult> GetProductCount()
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    ProductCountViewModel model = new ProductCountViewModel { TotalProducts = ctx.Products.Count(x => x.IsDeleted == false) };
                    CustomResponse<ProductCountViewModel> response = new CustomResponse<ProductCountViewModel>
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

        [HttpGet]
        [Route("GetAllProducts")]
        public async Task<IHttpActionResult> GetAllProducts(int UserId, int PageSize, int PageNo, string filterTypes = "")
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    return Ok(new CustomResponse<ProductsViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new ProductsViewModel
                        {
                            Count = ctx.Products.Count(x => x.IsDeleted == false),
                            Products = ctx.Products.Include(x => x.ProductImages).Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).Page(PageSize, PageNo).ToList()
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("GetPopularProducts")]
        public async Task<IHttpActionResult> GetPopularProducts(int Count)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userFavourites = ctx.Favourites.Where(x => x.User_ID == userId && x.IsDeleted == false).ToList();

                    //var products = ctx.Products.Include(x => x.ProductImages).Where(x => x.IsDeleted == false).OrderByDescending(x => x.OrderedCount).Take(Count).ToList();
                    var products = ctx.Products.Include(x => x.ProductImages).Where(x => x.IsDeleted == false && x.IsPopular).Take(Count).ToList();

                    foreach (var product in products)
                    {
                        if (userFavourites.Any(x => x.Product_Id == product.Id))
                            product.IsFavourite = true;
                        else
                            product.IsFavourite = false;
                    }

                    return Ok(new CustomResponse<ProductsViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new ProductsViewModel { Products = products }
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
