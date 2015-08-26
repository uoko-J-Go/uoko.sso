using System;
using System.Net;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace UOKO.SSO.Client.MVC
{
    /// <summary>
    /// 做 认证 用,跳转为 401
    /// </summary>
    public class SSOAuthenticationAttribute : FilterAttribute, IAuthenticationFilter
    {
        private readonly string _forbiddenName;

        public SSOAuthenticationAttribute()
        {

        }

        public SSOAuthenticationAttribute(string forbiddenViewName)
        {
            if (string.IsNullOrWhiteSpace(forbiddenViewName))
            {
                throw new ArgumentNullException("forbiddenViewName");
            }

            this._forbiddenName = forbiddenViewName;
        }

        public void OnAuthentication(AuthenticationContext filterContext)
        {
            if (filterContext == null)
            {
                return;
            }

            if (filterContext.ActionDescriptor.IsDefined(typeof (AllowAnonymousAttribute), true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof (AllowAnonymousAttribute), true))
            {
                return;
            }

            var user = RelyingPartyClient.GeneratePrincipalForAuthRequest(filterContext.HttpContext);
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
            {
                // 如果没有 身份信息 就返回 401 

                filterContext.Result = new HttpUnauthorizedResult("user unauthenticated,please login");
            }
            else
            {
                filterContext.Principal = user;
            }
        }

        /// <summary>
        /// The purpose of the ChallengeAsync method is to add authentication challenges to the response, if needed. Here is the method signature:
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            // 如果非 ajax 请求
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var statusCodeResult = filterContext.Result as HttpStatusCodeResult;

                if (statusCodeResult == null)
                {
                    return;
                }

                // 如果未认证, 跳转登陆页面
                if (statusCodeResult.StatusCode == (int) HttpStatusCode.Unauthorized)
                {
                    string currentUrl = null;
                    if (filterContext.HttpContext.Request.Url != null)
                    {
                        currentUrl = filterContext.HttpContext.Request.Url.ToString();
                    }

                    var ssoLoginUrl = RelyingPartyClient.GetLogOnUrl(currentUrl);

                    filterContext.HttpContext.Response.Redirect(ssoLoginUrl);

                }
                else if (statusCodeResult.StatusCode == (int) HttpStatusCode.Forbidden)
                {
                    if (!string.IsNullOrEmpty(_forbiddenName))
                    {
                        // 如果未授权且存在 自定义页面, 跳转 未授权页面
                        filterContext.Result = new ViewResult()
                        {
                            ViewName = this._forbiddenName
                        };
                    }
                }
            }
        }
    }
}