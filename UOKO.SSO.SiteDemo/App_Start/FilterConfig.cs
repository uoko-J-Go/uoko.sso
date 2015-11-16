using System.Web;
using System.Web.Mvc;
using UOKO.SSO.Client.MVC;

namespace UOKO.SSO.SiteDemo
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // UnAuthroized is the view name of forbidden result
            // SSOAuthenticationAttribute for authrication (login)
            //filters.Add(new SSOAuthenticationAttribute("UnAuthroized"));

            // SSOAuthorizeAttribute for access control
            //filters.Add(new SSOAuthorizeAttribute());

            filters.Add(new HandleErrorAttribute());
        }
    }
}
