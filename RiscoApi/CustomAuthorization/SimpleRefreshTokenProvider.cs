using DAL;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BasketApi.CustomAuthorization
{
    public class SimpleRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var guid = Guid.NewGuid().ToString();


            //copy properties and set the desired lifetime of refresh token
            var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            {
                IssuedUtc = context.Ticket.Properties.IssuedUtc,
                ExpiresUtc = DateTime.UtcNow.AddDays(35) //SET DATETIME to 5 Minutes // it shoild be greater then the time set for the AccessTokenExpireTimeSpan so that in case token is expired he still can send the refresh_token
                                                           //ExpiresUtc = DateTime.UtcNow.AddMonths(3) 
            };
            /*CREATE A NEW TICKET WITH EXPIRATION TIME OF 5 MINUTES 
             *INCLUDING THE VALUES OF THE CONTEXT TICKET: SO ALL WE 
             *DO HERE IS TO ADD THE PROPERTIES IssuedUtc and 
             *ExpiredUtc to the TICKET*/
            var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);

            //saving the new refreshTokenTicket to a local var of Type ConcurrentDictionary<string,AuthenticationTicket>
            // consider storing only the hash of the handle
            using (RiscoContext ctx = new RiscoContext())
            {
                ctx.RefreshTokens.Add(new RefreshTokens { Token = guid });
                ctx.SaveChanges();
            }
            _refreshTokens.TryAdd(guid, refreshTokenTicket);
            context.SetToken(guid);
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            AuthenticationTicket ticket;
            using (RiscoContext ctx = new RiscoContext())
            {
                if (_refreshTokens.TryRemove(context.Token, out ticket))
                {
                    var refreshToken = ctx.RefreshTokens.FirstOrDefault(x => x.Token == context.Token);
                    if (refreshToken != null)
                    {
                        ctx.RefreshTokens.Remove(refreshToken);
                        ctx.SaveChanges();
                        context.SetTicket(ticket);
                    }
                }
            }
        }
        public void Create(AuthenticationTokenCreateContext context)
        {

        }
        public void Receive(AuthenticationTokenReceiveContext context)
        {

        }

    }
}