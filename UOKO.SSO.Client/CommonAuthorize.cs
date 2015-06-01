using System.Web.Mvc;

namespace UOKO.SSO.Client
{
 
    public class CommonAuthorize : AuthorizeAttribute
    {
        /// <summary>
        /// 认证失败的,跳转到 sso server login page.
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            string currentUrl = null;
            if (filterContext.HttpContext.Request.Url != null)
            {
                currentUrl = filterContext.HttpContext.Request.Url.ToString();
            }

            // handle ajax request not authentication here.
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                // 这里默认是返回一个相应的 401 结果,告知用户未授权,需要重新登陆.
                filterContext.Result = new HttpUnauthorizedResult("user unanthorized,please login");
            }
            else
            {
                // 否则进行跳转.
                var ssoLoginUrl = RelyingPartyClient.GetLogOnUrl(currentUrl);

                filterContext.HttpContext.Response.Redirect(ssoLoginUrl);
            }
        }
    }

}