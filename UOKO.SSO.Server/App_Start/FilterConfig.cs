using System.Web.Mvc;
using UOKO.SSO.Server.Utils;

namespace UOKO.SSO.Server
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
           // filters.Add(new CustomerErrorHandleAttribute());
        }
    }
}
