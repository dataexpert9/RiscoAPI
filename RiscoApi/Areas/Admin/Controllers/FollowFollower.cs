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
using System.Text.RegularExpressions;

namespace BasketApi.Areas.SubAdmin.Controllers
{
    [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User", "Guest")]
    [RoutePrefix("api/FollowFollower")]
    public class FollowFollowerController : ApiController
    { 
        [HttpGet]
        [Route("Follow")]
        public async Task<IHttpActionResult> Follow(int FollowUser_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    FollowFollower followFollower = new FollowFollower
                    {
                        FirstUser_Id = userId,
                        SecondUser_Id = FollowUser_Id,
                        CreatedDate = DateTime.UtcNow,
                    };

                    ctx.FollowFollowers.Add(followFollower);
                    ctx.SaveChanges();

                    CustomResponse<FollowFollower> response = new CustomResponse<FollowFollower>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = followFollower
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
        [Route("UnFollow")]
        public async Task<IHttpActionResult> UnFollow(int UnFollowUser_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    FollowFollower followFollower = ctx.FollowFollowers.FirstOrDefault(x => x.FirstUser_Id == userId && x.SecondUser_Id == UnFollowUser_Id && x.IsDeleted == false);
                    followFollower.IsDeleted = true;
                    ctx.SaveChanges();

                    CustomResponse<FollowFollower> response = new CustomResponse<FollowFollower>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = followFollower
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
        [Route("GetFollowers")]
        public async Task<IHttpActionResult> GetFollowers()
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    List<FollowFollower> followFollowers = new List<FollowFollower>();

                    followFollowers = ctx.FollowFollowers
                        .Include(x => x.FirstUser)
                        .Where(x => x.IsDeleted == false && x.SecondUser_Id == userId)
                        .ToList();

                    CustomResponse<List<FollowFollower>> response = new CustomResponse<List<FollowFollower>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = followFollowers
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
        [Route("GetFollowings")]
        public async Task<IHttpActionResult> GetFollowings()
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    List<FollowFollower> followFollowers = new List<FollowFollower>();

                    followFollowers = ctx.FollowFollowers
                        .Include(x => x.SecondUser)
                        .Where(x => x.IsDeleted == false && x.FirstUser_Id == userId)
                        .ToList();

                    CustomResponse<List<FollowFollower>> response = new CustomResponse<List<FollowFollower>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = followFollowers
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        #region Private Regions
        #endregion
    }
}
