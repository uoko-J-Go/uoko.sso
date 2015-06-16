using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NLog.Config;
using UOKO.SSO.Core;
using UOKO.SSO.Server.Utils;

namespace UOKO.SSO.Server
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            ConfigurationItemFactory.Default.Targets
                                    .RegisterDefinition("UOKOFrameworkLog", typeof (UOKOFrameworkLogTarget));

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

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
    }
}