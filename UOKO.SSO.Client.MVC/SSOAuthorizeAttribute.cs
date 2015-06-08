using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UOKO.SSO.Core;

namespace UOKO.SSO.Client.MVC
{

    /// <summary>
    /// 做授权用,跳转为 403 
    /// </summary>
    public class SSOAuthorizeAttribute : AuthorizeAttribute
    {

        /// <summary>
        /// 认证失败的,跳转到 sso server login page.
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden, "permission denied.");
        }

        /// <summary>
        /// 在这里进行 授权 处理,判断权限相关.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            var ssoIdentity = SSOInfo.UserIdentity;
            if (ssoIdentity == null)
            {
                return false;
            }

            var controllerName = httpContext.Request.RequestContext.RouteData.Values["controller"];
            var actionName = httpContext.Request.RequestContext.RouteData.Values["action"];
            var urlCode = string.Format("{0}/{1}", controllerName, actionName);

            var hasPermission = ssoIdentity.HasPermission(urlCode);
            return hasPermission;
        }

    }
}