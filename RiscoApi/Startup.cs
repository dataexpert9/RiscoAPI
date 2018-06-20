using System;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.OAuth;
using System.Web.Http;
using BasketApi.CustomAuthorization;
using AutoMapper;
using BasketApi.AdminViewModel;

[assembly: OwinStartup(typeof(BasketApi.Startup))]

namespace BasketApi
{
    public partial class Startup
    {
        public object Httpconfiguration { get; private set; }
        public static OAuthAuthorizationServerOptions OAuthServerOptions;
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions;
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
            //app.Use<CustomAuthenticationMiddleware>(); //Use this middleware to return custom error codes instead of 400 in GrantResourceOwnerCredentials.
            BasketApi.BasketSettings.LoadSettings();

            ConfigureOAuth(app);

            HttpConfiguration config = new HttpConfiguration();
            app.UseWebApi(config);

            Mapper.Initialize(cfg => {
                cfg.CreateMap<DAL.Order, BasketApi.AppsViewModels.OrderViewModel>();
                cfg.CreateMap<DAL.StoreOrder, BasketApi.AppsViewModels.StoreOrderViewModel>();
                cfg.CreateMap<DAL.UserSubscriptions, BasketApi.AdminViewModel.SubscriptionListViewModel >();
            });
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            //app.CreatePerOwinContext(ApplicationDbContext.Create);
            //app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AuthenticationType = OAuthDefaults.AuthenticationType,
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(30),
                Provider = new MyAuthorizationServerProvider(),
                RefreshTokenProvider = new SimpleRefreshTokenProvider()
            };

            // Token Generation
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);
            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "279270651973-o98bchtvhdvacm66ukf137usfjkogr43.apps.googleusercontent.com",
            //    ClientSecret = "C7Puvd3YM2F6IQxqSOEAbm7F"
            //});

        }
    }
}
