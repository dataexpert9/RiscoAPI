using BasketApi;
using BasketApi.Areas.Admin.ViewModels;
using BasketApi.ViewModels;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using static BasketApi.Utility;

namespace WebApplication1.Areas.Admin.Controllers
{
    [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User")]
    [RoutePrefix("api/Packages")]
    public class PackageController : ApiController
    {
        [HttpGet]
        [Route("GetPackageProducts")]
        public async Task<IHttpActionResult> GetPackageProducts(int PackageId, int StoreId)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {

                    #region query
                    var query = @"
SELECT
  COALESCE(t2.Product_Id, t1.Product_Id) AS Id,
  COALESCE(t2.Product_Id, t1.Product_Id) AS Product_Id,
  COALESCE(t2.Name, t1.Name) AS Name,
  COALESCE(t2.Price, t1.Price) AS Price,
  COALESCE(t2.StoreName, t1.StoreName) AS StoreName,
  COALESCE(t2.CategoryName, t1.CategoryName) AS CategoryName,
  COALESCE(t2.ImageUrl, t1.ImageUrl) AS ImageUrl,
  COALESCE(t2.Qty, 1) AS Qty,
  COALESCE(t2.PackageProductId, 0) AS PackageProductId,
  COALESCE(t2.Package_Id, 0) AS Package_Id,
  COALESCE(t2.IsChecked, CAST(0 AS bit)) AS IsChecked
FROM (SELECT
  Products.Id AS Product_Id,
  Products.Name AS Name,
  Products.Price AS Price,
  Stores.Name AS StoreName,
  Categories.Name AS CategoryName,
  Products.ImageUrl AS ImageUrl,
  1 AS Qty,
  0 AS PackageProductId,
  0 AS Package_Id,
  CAST(1 AS bit) AS IsChecked
FROM Products
JOIN Categories
  ON Products.Category_Id = Categories.Id
JOIN Stores
  ON Products.Store_Id = Stores.Id
WHERE Stores.Id = " + StoreId + @"
AND Products.IsDeleted = 0
AND Categories.IsDeleted = 0
AND Stores.IsDeleted = 0) t1
LEFT JOIN (SELECT
  Products.Id AS Product_Id,
  Products.Name AS Name,
  Products.Price AS Price,
  Stores.Name AS StoreName,
  Categories.Name AS CategoryName,
  Products.ImageUrl AS ImageUrl,
  Package_Products.Qty AS Qty,
  Package_Products.Id AS PackageProductId,
  Package_Products.Package_Id,
  CAST(1 AS bit) AS IsChecked
FROM Packages
JOIN Package_Products
  ON Package_Products.Package_Id = Packages.Id
JOIN Products
  ON Products.Id = Package_Products.Product_Id
JOIN Stores
  ON Stores.Id = Products.Store_Id
JOIN Categories
  ON Categories.Id = Products.Category_Id
WHERE Packages.Id = " + PackageId + @"
AND Packages.IsDeleted = 0
AND Products.IsDeleted = 0
AND Categories.IsDeleted = 0) t2
  ON t1.Product_Id = t2.Product_Id
order by IsChecked desc
";
                    #endregion

                    var packageProducts = ctx.Database.SqlQuery<SearchProductViewModel>(query).ToList();
                    return Ok(new CustomResponse<SearchProductListViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new SearchProductListViewModel { Products = packageProducts } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }
    }
}
