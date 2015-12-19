using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;
using IdentityModel.Client;
using IdentityServer3.Core;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Validation;
using IdentityServer3.Host.Config;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using UOKO.SSO.Server.IdentityServer.CustomService;
using UOKO.SSO.Server.Service.IdentityServer;
using AuthenticationOptions = IdentityServer3.Core.Configuration.AuthenticationOptions;

namespace UOKO.SSO.Server
{
    public partial class Startup
    {

        private readonly string _ssoUrl = ConfigurationManager.AppSettings["sso.url"];

        // 有关配置身份验证的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            //AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;

            AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Subject;
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            app.Map("/identity",
                    idsrvApp =>
                    {
                        var factory = new IdentityServerServiceFactory()
                            .UseInMemoryClients(Clients.Get())
                            .UseInMemoryScopes(Scopes.Get());

                        factory.ViewService = new Registration<IViewService>(typeof (UOKOViewService));

                        var userService = new UOKOUserService();
                        factory.UserService = new Registration<IUserService>(resolver => userService);

                        factory.SecretValidators = new List<Registration<ISecretValidator>>()
                                                   {
                                                       new Registration<ISecretValidator,HashedSharedSecretValidator>(),
                                                       new Registration<ISecretValidator,X509CertificateThumbprintSecretValidator>(),
                                                       new Registration<ISecretValidator,PlainTextSharedSecretValidator>(),
                                                   };
                        
                        idsrvApp.UseIdentityServer(new IdentityServerOptions
                                                   {
                                                       SiteName = "UOKO-SSO",
                                                       SigningCertificate = Cert.Load(),
                                                       RequireSsl = false,
                                                       Factory = factory,
                                                        
                                                       AuthenticationOptions = new AuthenticationOptions
                                                       {
                                                           EnablePostSignOutAutoRedirect = true,
                                                           EnableSignOutPrompt = false,
                                                           //IdentityProviders = ConfigureIdentityProviders
                                                           InvalidSignInRedirectUrl = _ssoUrl
                                                       }
                                                   });
                    });


            app.UseCookieAuthentication(new CookieAuthenticationOptions
                                        {
                                            AuthenticationType = OpenIdConnectAuthenticationDefaults.AuthenticationType,
                                             
                                        });

            
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = _ssoUrl.TrimEnd('/')+"/identity",

                ClientId = "uoko-sso",
                Scope = "openid profile",
                ResponseType = "id_token token",
                RedirectUri = _ssoUrl,
                 
                SignInAsAuthenticationType = OpenIdConnectAuthenticationDefaults.AuthenticationType,
                UseTokenLifetime = false,
                
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = async n =>
                    {
                        var nid = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType, Constants.ClaimTypes.Name, Constants.ClaimTypes.Role);

                        // get userinfo data
                        var userInfoClient = new UserInfoClient(
                        new Uri(n.Options.Authority + "/connect/userinfo"),
                        n.ProtocolMessage.AccessToken);

                        var userInfo = await userInfoClient.GetAsync();
                        userInfo.Claims.ToList().ForEach(ui => nid.AddClaim(new Claim(ui.Item1, ui.Item2)));

                        // keep the id_token for logout
                        nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
                        // add access token for sample API
                        nid.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));
                        // keep track of access token expiration
                        nid.AddClaim(new Claim("expires_at", DateTimeOffset.Now.AddSeconds(int.Parse(n.ProtocolMessage.ExpiresIn)).ToString()));
                        // add some other app specific claim
                        nid.AddClaim(new Claim("app_specific", "some data"));
                        n.AuthenticationTicket = new AuthenticationTicket( nid,n.AuthenticationTicket.Properties);
                    },

                    RedirectToIdentityProvider = n =>
                    {
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                        {
                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                            if (idTokenHint != null)
                            {
                                n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                            }
                        }
                         
                        return Task.FromResult(0);
                    },

                    AuthenticationFailed = faildMsg =>
                    {
                        if (faildMsg.Exception is OpenIdConnectProtocolInvalidNonceException)
                        {
                            if (faildMsg.Exception.Message.Contains("IDX10311"))
                            {
                                faildMsg.SkipToNextMiddleware();
                            }
                        }
                        return Task.FromResult(0);
                    }
                }
            });
        }
    }

}