using System;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using UOKO.SSO.Core;

namespace UOKO.SSO
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
        }


        public override void Init()
        {
            base.Init();

            this.AuthenticateRequest += OnAuthenticateRequest;
        }

        /// <summary>
        /// 在这里构造身份信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnAuthenticateRequest(object sender, EventArgs eventArgs)
        {
            try
            {
                var cookieInfo = SSOAuthentication.GetAuthCookieInfo(ServerConfig.CookieName);

                if (cookieInfo != null
                    && !string.IsNullOrWhiteSpace(cookieInfo.Alias))
                {
                    // 有身份信息,构建基础身份信息
                    var userInfo = SSOAuthentication.GenerateClaimsPrincipal(cookieInfo);

                    if (this.Context != null)
                    {
                        this.Context.User = userInfo;
                    }

                    Thread.CurrentPrincipal = userInfo;
                }
            }
            catch (Exception ex)
            {
                // todo: log exception
            }
        }
    }
}