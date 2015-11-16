using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Web.Helpers;
using IdentityServer3.Core;
using IdentityServer3.Core.Configuration;
using IdentityModel.Client;
using System.Threading.Tasks;
using IdentityServer3.Core.Services.InMemory;
using IdentityServer3.Core.Services;
using UOKO.SSO.Server;
using SSO.Domain.IdentityServer;

[assembly: OwinStartup(typeof(Startup))]
namespace UOKO.SSO.Server
{
    public partial class Startup
    {
        //public void Configuration(IAppBuilder app)
        //{
        //    ConfigureAuth(app);
        //}

        public void Configuration(IAppBuilder app)
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Subject;
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            app.Map("/identity", idsrvApp =>
            {
                var factory = new IdentityServerServiceFactory();
                factory.ClientStore = new Registration<IClientStore>(new InMemoryClientStore(Clients.Get()));
                factory.UserService = new Registration<IUserService>(new InMemoryUserService(Users.Get()));
                factory.ScopeStore = new Registration<IScopeStore>(new InMemoryScopeStore(Scopes.Get()));
               

                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "Embedded IdentityServer",
                    SigningCertificate = LoadCertificate(),
                    RequireSsl = false,
                    Factory = factory,

                    AuthenticationOptions = new IdentityServer3.Core.Configuration.AuthenticationOptions
                    {
                        EnablePostSignOutAutoRedirect = true,
                        //IdentityProviders = ConfigureIdentityProviders
                    }
                });
            });
           // app.UseResourceAuthorization(new AuthorizationManager());

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });


            //app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            //{
            //    Authority = "http://sso.domain.com/identity",

            //    ClientId = "mvc",
            //    Scope = "openid profile roles sampleApi",
            //    ResponseType = "id_token token",
            //    RedirectUri = "http://sso.domain.com/",

            //    SignInAsAuthenticationType = "Cookies",
            //    UseTokenLifetime = false,

            //    Notifications = new OpenIdConnectAuthenticationNotifications
            //    {
            //        SecurityTokenValidated = async n =>
            //        {
            //            var nid = new ClaimsIdentity(
            //                n.AuthenticationTicket.Identity.AuthenticationType,
            //                Constants.ClaimTypes.GivenName,
            //                Constants.ClaimTypes.Role);

            //            // get userinfo data
            //            var userInfoClient = new UserInfoClient(
            //            new Uri(n.Options.Authority + "/connect/userinfo"),
            //            n.ProtocolMessage.AccessToken);

            //            var userInfo = await userInfoClient.GetAsync();
            //            userInfo.Claims.ToList().ForEach(ui => nid.AddClaim(new Claim(ui.Item1, ui.Item2)));

            //            // keep the id_token for logout
            //            nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

            //            // add access token for sample API
            //            nid.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));

            //            // keep track of access token expiration
            //            nid.AddClaim(new Claim("expires_at", DateTimeOffset.Now.AddSeconds(int.Parse(n.ProtocolMessage.ExpiresIn)).ToString()));

            //            // add some other app specific claim
            //            nid.AddClaim(new Claim("app_specific", "some data"));

            //            n.AuthenticationTicket = new AuthenticationTicket(
            //                nid,
            //                n.AuthenticationTicket.Properties);
            //        },

            //        RedirectToIdentityProvider = n =>
            //        {
            //            if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
            //            {
            //                var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

            //                if (idTokenHint != null)
            //                {
            //                    n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
            //                }
            //            }

            //            return Task.FromResult(0);
            //        }
            //    }
            //});
        }

        X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(
                string.Format(@"{0}\bin\identityServer\idsrv3test.pfx", AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }
    }
}
