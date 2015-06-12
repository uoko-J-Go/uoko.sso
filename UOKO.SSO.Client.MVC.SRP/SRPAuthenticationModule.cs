using System;
using System.Web;

namespace UOKO.SSO.Client.MVC.SRP
{
    /// <summary>
    /// 前后端分离,静态文件的认证 module
    /// 需要设置这个 module 的 preCondition="managedHandler"
    /// 这样可以省去对其他静态资源文件的处理
    /// </summary>
    public class SRPAuthenticationModule:IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += context_AuthenticateRequest;
        }

        void context_AuthenticateRequest(object sender, EventArgs e)
        {
            var ctx = new HttpContextWrapper(((HttpApplication) sender).Context);

            // 如果是对 api 请求的话,走 SSO.Client.MVC 的 filter 进行认证
            if (ctx.Request.RawUrl.StartsWith("/api/"))
            {
                return;
            }

            // 回来到这里的,只有对但也运用的 app 静态页面, 这类页面不受 mvc filter 的控制,因为使用的是 iis 的 url rewrite
            // 所以这类页面的 sso 跳转登陆 需要通过 module 来单独做
            var user = RelyingPartyClient.GeneratePrincipalForAuthRequest(ctx);
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
            {
                string currentUrl = null;
                if (ctx.Request.Url != null)
                {
                    currentUrl = ctx.Request.Url.Scheme + "://" + ctx.Request.Url.Host + ctx.Request.RawUrl;
                }

                var logOnUrl = RelyingPartyClient.GetLogOnUrl(currentUrl);
                ctx.Response.Redirect(logOnUrl);
            }
        }

        public void Dispose()
        {
        }
    }
}