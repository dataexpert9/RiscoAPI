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
    [RoutePrefix("api/Post")]
    public class PostController : ApiController
    {
        [HttpPost]
        [Route("ImageUpload")]

        public async Task<IHttpActionResult> ImageUpload()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;
                string folderPath = string.Empty;

                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multipart data is not included in request." }
                    });
                }
                else if (httpRequest.Files.Count == 0)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "NotFound",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Result = new Error { ErrorMessage = "Please upload an attachment." }
                    });
                }
                else if (httpRequest.Files.Count > 1)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multiple attachments are not allowed. Please upload one attachment." }
                    });
                }
                else
                {
                    var postedFile = httpRequest.Files[0];

                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 10; //Size = 1 MB  

                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "UnsupportedMediaType",
                                StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                Result = new Error { ErrorMessage = "Please Upload image of type .jpg, .gif, .png." }
                            });
                        }
                        if (postedFile.ContentLength > MaxContentLength)
                        {

                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "UnsupportedMediaType",
                                StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                Result = new Error { ErrorMessage = "Please Upload a file upto 1 mb." }
                            });
                        }
                        else
                        {
                            folderPath = ConfigurationManager.AppSettings["PostMediaFolderPath"] + DateTime.Now.Ticks.ToString() + postedFile.FileName;
                            newFullPath = HttpContext.Current.Server.MapPath("~/" + folderPath);
                            postedFile.SaveAs(newFullPath);

                            return Content(HttpStatusCode.OK, new CustomResponse<string>
                            {
                                Message = Global.ResponseMessages.Success,
                                StatusCode = (int)HttpStatusCode.OK,
                                Result = folderPath
                            });
                        }
                    }
                    else
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = Global.ResponseMessages.BadRequest,
                            StatusCode = (int)HttpStatusCode.BadRequest,
                            Result = new Error { ErrorMessage = "Something went wrong Please try again." }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpPost]
        [Route("CreatePost")]
        public async Task<IHttpActionResult> CreatePost(CreatePostBindingModel model)
        {
            try
            {
                Validate(model);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    Post post = new Post
                    {
                        Text = model.Text,
                        RiskLevel = model.RiskLevel,
                        Location = model.Location,
                        Visibility = model.Visibility,
                        User_Id = userId,
                        CreatedDate = DateTime.UtcNow,
                    };

                    ctx.Posts.Add(post);
                    ctx.SaveChanges();

                    SetTrends(Text: post.Text, User_Id: userId, Post_Id: post.Id);

                    if (!string.IsNullOrEmpty(model.ImageUrls))
                    {
                        var ImageUrls = model.ImageUrls.Split(',');
                        foreach (var ImageUrl in ImageUrls)
                        {
                            Media media = new Media
                            {
                                Type = (int)MediaTypes.Image,
                                Url = ImageUrl,
                                Post_Id = post.Id,
                                CreatedDate = DateTime.UtcNow
                            };
                            ctx.Medias.Add(media);
                            ctx.SaveChanges();
                        }
                    }

                    //if (!string.IsNullOrEmpty(model.VideoUrls))
                    //{
                    //    var VideoUrls = model.VideoUrls.Split(',');
                    //    foreach (var VideoUrl in VideoUrls)
                    //    {
                    //        Media media = new Media
                    //        {
                    //            Type = (int)MediaTypes.Image,
                    //            Url = VideoUrl,
                    //            Post_Id = post.Id,
                    //            CreatedDate = DateTime.UtcNow
                    //        };
                    //        ctx.Medias.Add(media);
                    //        ctx.SaveChanges();
                    //    }
                    //}

                    CustomResponse<Post> response = new CustomResponse<Post>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = post
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
        [Route("GetPosts")]
        public async Task<IHttpActionResult> GetPosts(int PageSize = int.MaxValue, int PageNo = 0)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    List<Post> posts = new List<Post>();

                    // Hide All Post Users

                    var HideAllUsersIds = ctx.HideAllPosts.Where(x => x.FirstUserAll_Id == userId && x.IsDeleted == false).Select(x => x.SecondUserAll_Id).ToList();

                    var HidePostsIds = ctx.HidePosts.Where(x => x.FirstUser_Id == userId && x.IsDeleted == false).Select(x => x.Post_Id).ToList();

                    posts = ctx.Posts
                        .Include(x => x.User)
                        .Include(x => x.Medias)
                        .Where(x => x.IsDeleted == false && x.User_Id != userId && !HideAllUsersIds.Contains(x.User_Id) && !HidePostsIds.Contains(x.Id))
                        .OrderByDescending(x=> x.Id)
                        .ToList();

                    int PostCount = posts.Count();
                    posts = posts.Skip(PageNo * PageSize).Take(PageSize).ToList();

                    foreach (Post post in posts)
                    {
                        post.IsLiked = ctx.Likes.Any(x => x.Post_Id == post.Id && x.User_Id == userId && x.IsDeleted == false);
                        post.LikesCount = ctx.Posts.Sum(p => p.Likes.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.CommentsCount = ctx.Posts.Sum(p => p.Comments.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.ShareCount = ctx.Posts.Sum(p => p.Shares.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.IsUserFollow = ctx.FollowFollowers.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == post.User_Id && x.IsDeleted == false);
                    }

                    CustomResponse<PostListViewModel> response = new CustomResponse<PostListViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new PostListViewModel
                        {
                            Posts = posts,
                            PostCount = PostCount
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

        [HttpGet]
        [Route("GetPostsByUserId")]
        public async Task<IHttpActionResult> GetPostsByUserId(int User_Id, int PageSize = int.MaxValue, int PageNo = 0)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    List<Post> posts = new List<Post>();
                    posts = ctx.Posts
                        .Include(x => x.User)
                        .Include(x => x.Medias)
                        .Where(x => x.User_Id == User_Id && x.IsDeleted == false).OrderByDescending(x => x.Id)
                        .OrderByDescending(x => x.Id)
                        .ToList();

                    int PostCount = posts.Count();
                    posts = posts.Skip(PageNo * PageSize).Take(PageSize).ToList();

                    foreach (Post post in posts)
                    {
                        post.IsLiked = ctx.Likes.Any(x => x.Post_Id == post.Id && x.User_Id == User_Id && x.IsDeleted == false);
                        post.LikesCount = ctx.Posts.Sum(p => p.Likes.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.CommentsCount = ctx.Posts.Sum(p => p.Comments.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.ShareCount = ctx.Posts.Sum(p => p.Shares.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.IsUserFollow = true;
                    }

                    CustomResponse<PostListViewModel> response = new CustomResponse<PostListViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new PostListViewModel
                        {
                            Posts = posts,
                            PostCount = PostCount
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

        [HttpGet]
        [Route("GetPostByPostId")]
        public async Task<IHttpActionResult> GetPostByPostId(int Post_Id)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    Post post = new Post();
                    post = ctx.Posts.Include(x => x.User)
                        .FirstOrDefault(x => x.Id == Post_Id && x.IsDeleted == false);

                    post.IsLiked = ctx.Likes.Any(x => x.Post_Id == Post_Id && x.User_Id == userId && x.IsDeleted == false);
                    post.LikesCount = ctx.Posts.Sum(p => p.Likes.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                    post.CommentsCount = ctx.Posts.Sum(p => p.Comments.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                    post.ShareCount = ctx.Posts.Sum(p => p.Shares.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());

                    // For comments and their child comments including self-like

                    List<Comment> comments = ctx.Comments
                        .Include(x => x.User)
                        .Where(x => x.Post_Id == Post_Id && x.ParentComment_Id == 0 && x.IsDeleted == false).ToList();
                    foreach (Comment comment in comments)
                    {
                        comment.ChildComments = ctx.Comments
                            .Include(x => x.User)
                            .Where(x => x.ParentComment_Id == comment.Id && x.IsDeleted == false).ToList();
                        comment.IsLiked = ctx.Likes.Any(x => x.Comment_Id == comment.Id && x.User_Id == userId && x.IsDeleted == false);
                        foreach (Comment childComment in comment.ChildComments)
                        {
                            childComment.IsLiked = ctx.Likes.Any(x => x.Comment_Id == childComment.Id && x.User_Id == userId && x.IsDeleted == false);
                        }
                    }

                    post.Comments = comments;

                    CustomResponse<Post> response = new CustomResponse<Post>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = post
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
        [Route("LikePost")]
        public async Task<IHttpActionResult> LikePost(int Post_Id)
        {
            try
            { 
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    Like like = new Like
                    {
                        Post_Id = Post_Id,
                        User_Id = userId,
                        CreatedDate = DateTime.UtcNow
                    };

                    ctx.Likes.Add(like);
                    ctx.SaveChanges();

                    int SecondUser_Id = ctx.Posts.FirstOrDefault(x => x.Id == Post_Id && x.IsDeleted == false).User_Id;
                    SetTopFollowerLog(FirstUser_Id: userId, SecondUser_Id: SecondUser_Id);

                    CustomResponse<Like> response = new CustomResponse<Like>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = like
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
        [Route("UnLikePost")]
        public async Task<IHttpActionResult> UnLikePost(int Post_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {

                    Like like = ctx.Likes.FirstOrDefault(x => x.Post_Id == Post_Id && x.User_Id == userId && x.IsDeleted == false);
                    like.IsDeleted = true;
                    ctx.SaveChanges();

                    CustomResponse<Like> response = new CustomResponse<Like>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = like
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
        [Route("LikeComment")]
        public async Task<IHttpActionResult> LikeComment(int Comment_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    Like like = new Like
                    {
                        Comment_Id = Comment_Id,
                        User_Id = userId,
                        CreatedDate = DateTime.UtcNow
                    };

                    ctx.Likes.Add(like);
                    ctx.SaveChanges();

                    CustomResponse<Like> response = new CustomResponse<Like>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = like
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
        [Route("UnLikeComment")]
        public async Task<IHttpActionResult> UnLikeComment(int Comment_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {

                    Like like = ctx.Likes.FirstOrDefault(x => x.Comment_Id == Comment_Id && x.User_Id == userId && x.IsDeleted == false);
                    like.IsDeleted = true;
                    ctx.SaveChanges();

                    CustomResponse<Like> response = new CustomResponse<Like>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = like
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpPost]
        [Route("Comment")]
        public async Task<IHttpActionResult> Comment(CreateCommentBindingModel model)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    Comment comment = new Comment
                    {
                        Text = model.Text,
                        Post_Id = model.Post_Id,
                        User_Id = userId,
                        CreatedDate = DateTime.UtcNow
                    };

                    ctx.Comments.Add(comment);
                    ctx.SaveChanges();

                    SetTrends(Text: comment.Text, User_Id: userId, Comment_Id: comment.Id);

                    int SecondUser_Id = ctx.Posts.FirstOrDefault(x => x.Id == model.Post_Id && x.IsDeleted == false).User_Id;
                    SetTopFollowerLog(FirstUser_Id: userId, SecondUser_Id: SecondUser_Id);
                    CustomResponse<Comment> response = new CustomResponse<Comment>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = comment
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpPost]
        [Route("CommentReply")]
        public async Task<IHttpActionResult> CommentReply(CreateCommentBindingModel model)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    Comment comment = new Comment
                    {
                        Text = model.Text,
                        Post_Id = model.Post_Id,
                        ParentComment_Id = model.ParentComment_Id,
                        User_Id = userId,
                        CreatedDate = DateTime.UtcNow,
                    };

                    ctx.Comments.Add(comment);
                    ctx.SaveChanges();

                    SetTrends(Text: comment.Text, User_Id: userId, Comment_Id: comment.Id);

                    int SecondUser_Id = ctx.Posts.FirstOrDefault(x => x.Id == model.Post_Id && x.IsDeleted == false).User_Id;
                    SetTopFollowerLog(FirstUser_Id: userId, SecondUser_Id: SecondUser_Id);

                    CustomResponse<Comment> response = new CustomResponse<Comment>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = comment
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
        [Route("Repost")]
        public async Task<IHttpActionResult> Repost(int Post_Id, string Location)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    Post post = ctx.Posts
                        .Include(x => x.Medias).AsNoTracking()
                        .FirstOrDefault(x => x.Id == Post_Id);

                    post.User_Id = userId;
                    post.Location = Location;
                    post.CreatedDate = DateTime.UtcNow;
                    foreach (Media media in post.Medias)
                    {
                        media.CreatedDate = DateTime.UtcNow;
                    }

                    ctx.Posts.Add(post);
                    ctx.SaveChanges();

                    SetTrends(Text: post.Text, User_Id: userId, Post_Id: post.Id);

                    Share share = new Share
                    {
                        Post_Id = Post_Id,
                        User_Id = userId,
                        CreatedDate = DateTime.UtcNow,
                    };



                    ctx.Shares.Add(share);
                    ctx.SaveChanges();

                    CustomResponse<Post> response = new CustomResponse<Post>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = post
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
        [Route("HidePost")]
        public async Task<IHttpActionResult> HidePost(int Post_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    Post post = ctx.Posts.FirstOrDefault(x => x.Id == Post_Id);

                    HidePost hidePost = new HidePost
                    {
                        FirstUser_Id = userId,
                        SecondUser_Id = post.User_Id,
                        Post_Id = Post_Id,
                        CreatedDate = DateTime.UtcNow
                    };

                    ctx.HidePosts.Add(hidePost);
                    ctx.SaveChanges();

                    CustomResponse<HidePost> response = new CustomResponse<HidePost>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = hidePost
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
        [Route("HideAllPost")]
        public async Task<IHttpActionResult> HideAllPost(int HideAllPostsUser_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    HideAllPost hideAllPost = new HideAllPost
                    {
                        FirstUserAll_Id = userId,
                        SecondUserAll_Id = HideAllPostsUser_Id,
                        CreatedDate = DateTime.UtcNow
                    };

                    ctx.HideAllPosts.Add(hideAllPost);
                    ctx.SaveChanges();

                    CustomResponse<HideAllPost> response = new CustomResponse<HideAllPost>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = hideAllPost
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
        [Route("GetTrends")]
        public async Task<IHttpActionResult> GetTrends()
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    string query = "select top 20 Text, count(Id) as [Count] from trendlogs  group by Text order by Count desc";
                    List<TrendBindingModel> trends = ctx.Database.SqlQuery<TrendBindingModel>(query).ToList();

                    CustomResponse<TrendListViewModel> response = new CustomResponse<TrendListViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new TrendListViewModel
                        {
                            Trends = trends
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

        [HttpGet]
        [Route("TurnOffNotifications")]
        public async Task<IHttpActionResult> TurnOffNotifications(int Post_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    TurnOffNotification turnOffNotification = new TurnOffNotification
                    {
                        User_Id = userId,
                        Post_Id = Post_Id,
                        CreatedDate = DateTime.UtcNow
                    };

                    ctx.TurnOffNotifications.Add(turnOffNotification);
                    ctx.SaveChanges();

                    CustomResponse<TurnOffNotification> response = new CustomResponse<TurnOffNotification>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = turnOffNotification
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
        [Route("ReportPost")]
        public async Task<IHttpActionResult> ReportPost(int Post_Id, int ReportType, string Text)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    ReportPost reportPost = new ReportPost
                    {
                        User_Id = userId,
                        Post_Id = Post_Id,
                        ReportType = ReportType,
                        Text = Text,
                        CreatedDate = DateTime.UtcNow
                    };

                    ctx.ReportPosts.Add(reportPost);
                    ctx.SaveChanges();

                    CustomResponse<ReportPost> response = new CustomResponse<ReportPost>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = reportPost
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

        private void SetTrends(string Text, int User_Id, int Post_Id = 0, int Comment_Id = 0)
        {
            try
            {
                var regex = new Regex(@"(?<=#)\w+");
                var matches = regex.Matches(Text);

                using (RiscoContext ctx = new RiscoContext())
                {
                    TrendLog trendLog;
                    foreach (Match m in matches)
                    {
                        trendLog = new TrendLog
                        {
                            Text = "#" + m.Value,
                            User_Id = User_Id,
                            Post_Id = Post_Id,
                            Comment_Id = Comment_Id,
                            CreatedDate = DateTime.UtcNow,
                        };
                        ctx.TrendLogs.Add(trendLog);
                    }

                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        private void SetTopFollowerLog(int FirstUser_Id, int SecondUser_Id)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    if(FirstUser_Id != SecondUser_Id)
                    {
                        TopFollowerLog topFollowerLog = new TopFollowerLog
                        {
                            FirstUser_Id = FirstUser_Id,
                            SecondUser_Id = SecondUser_Id,
                            CreatedDate = DateTime.UtcNow
                        };

                        ctx.TopFollowerLogs.Add(topFollowerLog);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }
        #endregion
    }
}
