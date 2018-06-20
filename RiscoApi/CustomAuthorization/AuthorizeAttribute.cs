using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;

namespace BasketApi
{
    public class AuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(actionContext);
            }
            else
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.Forbidden,
                    ReasonPhrase = "You are not authorized to call this method."
                };
            }
        }

        public AuthorizeAttribute(params string[] roles) : base()
        {
            Roles = string.Join(",", roles);
        }
        
    }
}