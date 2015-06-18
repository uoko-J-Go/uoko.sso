using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Owin;
using UOKO.SSO.Core;

namespace UOKO.SSO.Server
{
    public partial class Startup
    {
        // 有关配置身份验证的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseSSOAuth();
            app.UseStageMarker(PipelineStage.Authenticate);
        }
    }


    public static class SSOAuthExtension
    {
        public static void UseSSOAuth(this IAppBuilder app)
        {
            app.Use(typeof (SSOAuthenticationMiddleware));
        }
    }

    public class SSOAuthenticationMiddleware : OwinMiddleware
    {
        public SSOAuthenticationMiddleware(OwinMiddleware next) : base(next)
        {

        }


        public override async Task Invoke(IOwinContext context)
        {
            var cookieInfo = SSOAuthentication.GetAuthCookieInfo(ServerConfig.CookieName);

            if (cookieInfo != null && !string.IsNullOrWhiteSpace(cookieInfo.Alias))
            {
                // 有身份信息,构建基础身份信息
                var userInfo = SSOAuthentication.GenerateClaimsPrincipal(cookieInfo);
                context.Authentication.User = userInfo;
                Thread.CurrentPrincipal = userInfo;
            }

            await this.Next.Invoke(context);
        }
    }
}