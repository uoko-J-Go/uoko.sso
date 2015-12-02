using System.Web;
using System.Web.Mvc;

namespace UOKO.SSO.SiteDemo
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
