using System.Web;
using System.Web.Mvc;
using UOKO.SSO.Client.MVC;

namespace UOKO.SSO.SiteDemo
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new SSOAuthenticationAttribute("UnAuthroized"));
            filters.Add(new SSOAuthorizeAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
