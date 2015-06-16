using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web;
using NLog;
using UOKO.SSO.Core;

namespace UOKO.SSO.Client
{
    public class RelyingPartyClient
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();


        public static ClientConfig ClientInfo { get; private set; }

        public static void Config(ClientConfig config)
        {
            ClientInfo = config;
        }


        public static Func<HttpContextBase, IPrincipal> GeneratePrincipalFromLocalCookieFunc;
        public static Action<HttpContextBase, SSOCookieInfo> SetLocalCookieFunc;

        /// <summary>
        /// 构造身份信息，相当于是 AuthenticateRequest 方法。
        /// 
        /// 判断是否是域内用户，
        /// 如果是域内用户，直接读取 sso cookie 进行身份构建。
        ///     如果构建失败,则跳转 sso 登陆页
        /// 如果非域内用户，则判断是否带 sso token 参数。
        ///     如果带参数，则进行 sso server 验证获取构建身份信息。构建自身的 cookie
        ///         如果构建失败（获取身份信息失败），则跳转 sso 登录页。
        ///     如果不带参数，则检验是否有自身 cookie
        ///         有： 构建身份信息
        ///         否： 跳转 sso 登录页
        /// 
        /// 如果需要实现个性化的登录过程，client 设置 SetLocalCookieFunc,GeneratePrincipalFromLocalCookieFunc
        /// </summary>
        public static IPrincipal GeneratePrincipalForAuthRequest(HttpContextBase ctx)
        {
            IPrincipal principal = null;
            try
            {
                if (ctx == null)
                {
                    return null;
                }


                var request = ctx.Request;
                if (request.Url == null)
                {
                    return null;
                }

                var sameDomain = request.Url.Host.EndsWith(ClientInfo.ServerCookieDomain);

                principal = sameDomain
                                ? GeneratePrincipalForSameDomain()
                                : GeneratePrincipalForOtherDomain(ctx);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, "构造身份信息(IPrincipal)失败");
            }

            return principal;
        }


        /// <summary>
        /// 在这里构造身份信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void OnAuthenticateRequest(object sender, EventArgs eventArgs)
        {
            var application = sender as HttpApplication;
            if (application == null || application.Context == null)
            {
                return;
            }
            var ctx = new HttpContextWrapper(application.Context);

            var principal = GeneratePrincipalForAuthRequest(ctx);
            ctx.User = principal;
            Thread.CurrentPrincipal = principal;
        }

        /// <summary>
        /// Logoff
        /// </summary>
        public static void RemoveLocalCookie()
        {
            SSOAuthentication.RemoveCookie(ClientInfo.LocalCookieName);
        }

        #region For views

        /// <summary>
        /// 提供 sso server登录 url
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public static string GetLogOnUrl(string returnUrl = null)
        {
            var ctx = new HttpContextWrapper(HttpContext.Current);
            if (!string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = ctx.Server.UrlEncode(returnUrl);
            }
            var logOnUrl = string.Format("{0}/Account/Login?appKey={1}&returnUrl={2}",
                                         ClientInfo.ServerUrl,
                                         Uri.EscapeDataString(ClientInfo.AppKey ?? string.Empty),
                                         returnUrl);
            return logOnUrl;
        }

        /// <summary>
        ///  提供登出链接，用户网站完成自己的登出逻辑（清除一些自定义cookie等）后，
        ///  跳转到 sso 登出链接，进行登出,可以带 跳转 url，回跳用户方，否则进入 sso 登录页。
        /// 
        ///  同时刷新用户权限
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public static string GetLogOutUrl(string returnUrl = null)
        {
            var ctx = new HttpContextWrapper(HttpContext.Current);
            if (!string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = ctx.Server.UrlEncode(returnUrl);
            }

            var logOffUrl = string.Format("{0}/Account/LogOff?appKey={1}&returnUrl={2}",
                                          ClientInfo.ServerUrl,
                                          Uri.EscapeDataString(ClientInfo.AppKey ?? string.Empty),
                                          returnUrl);

            var identity = SSOInfo.UserIdentity;
            if (identity != null)
            {
                var alias = identity.UserAlias;
                PermissionService.RefreshCachePermissions(alias, ClientInfo.AppKey);
            }

            return logOffUrl;
        }

        #endregion

        #region Private help method

        private static IPrincipal GeneratePrincipalForSameDomain()
        {
            IPrincipal userInfo = null;
            var cookieInfo = SSOAuthentication.GetAuthCookieInfo(ServerConfig.CookieName);
            if (cookieInfo != null && !string.IsNullOrWhiteSpace(cookieInfo.Alias))
            {
                userInfo = SSOAuthentication.GenerateClaimsPrincipal(cookieInfo);
            }

            return userInfo;
        }

        private static IPrincipal GeneratePrincipalForOtherDomain(HttpContextBase ctx)
        {
            if (ctx == null)
            {
                return null;
            }

            var request = ctx.Request;
            if (request.QueryString == null)
            {
                return null;
            }

            var validateToken = request.QueryString[ServerConfig.TokenParamName];
            if (!string.IsNullOrWhiteSpace(validateToken))
            {
                // 有 token 进行 sso 服务端校验获取身份信息
                var userInfo = GetPrincipalInfoFromServer(ctx, validateToken);

                return userInfo;
            }
            else
            {
                // 没有 token ,则检验是否有自身 cookie
                var userInfo = GeneratePrincipalFromLocalCookieFunc == null
                                   ? GeneratePrincipalFromLocalCookie(ctx)
                                   : GeneratePrincipalFromLocalCookieFunc(ctx);
                return userInfo;
            }
        }

        private static IPrincipal GetPrincipalInfoFromServer(HttpContextBase ctx, string token)
        {
            var requestUrl = ClientInfo.ServerUrl + "/Account/ValidateToken";
            var client = new HttpClient();
            var dic = new Dictionary<string, string>()
                      {
                          {"token", token},
                          {"appKey", ClientInfo.AppKey}
                      };
            var sendInfo = new FormUrlEncodedContent(dic);

            CasTicket ticket = null;
            try
            {
                ticket = client.PostAsync(new Uri(requestUrl), sendInfo)
                               .Result.Content.ReadAsAsync<CasTicket>()
                               .Result;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, "ValidateToken from server (http) failed");
            }

            if (ticket == null)
            {
                return null;
            }

            var cookieInfo = new SSOCookieInfo()
                             {
                                 Alias = ticket.UserAlias,
                                 Name = ticket.UserName,
                             };


            IPrincipal userInfo = null;
            if (!string.IsNullOrWhiteSpace(cookieInfo.Alias))
            {
                userInfo = SSOAuthentication.GenerateClaimsPrincipal(cookieInfo);
                if (SetLocalCookieFunc != null)
                {

                    SetLocalCookieFunc(ctx, cookieInfo);
                }
                else
                {
                    SetLocalCookie(ctx, cookieInfo);
                }
            }

            return userInfo;
        }

        private static IPrincipal GeneratePrincipalFromLocalCookie(HttpContextBase ctx)
        {
            if (ctx == null)
            {
                return null;
            }

            var request = ctx.Request;
            if (request.QueryString == null)
            {
                return null;
            }

            IPrincipal userInfo = null;
            var localCookieInfo = SSOAuthentication.GetAuthCookieInfo(ClientInfo.LocalCookieName);
            if (localCookieInfo != null && !string.IsNullOrWhiteSpace(localCookieInfo.Alias))
            {
                // 有身份信息,构建基础身份信息
                userInfo = SSOAuthentication.GenerateClaimsPrincipal(localCookieInfo);
            }

            return userInfo;
        }

        private static void SetLocalCookie(HttpContextBase ctx, SSOCookieInfo cookieInfo)
        {
            if (ctx == null)
            {
                return;
            }

            SSOAuthentication.SetAuthCookie(cookieInfo, ClientInfo.LocalCookieName);
        }

        #endregion

    }
}