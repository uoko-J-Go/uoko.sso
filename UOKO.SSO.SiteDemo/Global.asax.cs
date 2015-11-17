using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using UOKO.SSO.Client;
using UOKO.SSO.Core;

namespace UOKO.SSO.SiteDemo
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {

            // config relying party client
            //RelyingPartyClient.Config(new ClientConfig("test-demo-site",
            //                                           "http://sso.uoko.ioc",
            //                                           "uoko.ioc",
            //                                           "http://api.permission.uoko.ioc/",
            //                                           "self-cookie"));

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}