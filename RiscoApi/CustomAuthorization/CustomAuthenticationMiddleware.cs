using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BasketApi.CustomAuthorization
{
    public class CustomAuthenticationMiddleware : OwinMiddleware
    {
        public CustomAuthenticationMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            await Next.Invoke(context);

            
            //if (context.Response.StatusCode == 400 && context.Response.Headers.ContainsKey("AuthorizationResponse"))
            //{
            //    context.Response.Headers.Remove("AuthorizationResponse");
            //    context.Response.StatusCode = 200;
            //}
        }
    }
}