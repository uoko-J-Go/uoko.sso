using System.Web.Mvc;
using RequireJsNet;
using UOKO.SSO.Core;
using UOKO.SSO.Server.Service;

namespace UOKO.SSO.Server.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {

        public ActionResult Index()
        {
            var userAlias = SSOInfo.UserIdentity.UserAlias;
            var appList = UserBiz.GetUserAppInfo(userAlias);
             
            RequireJsOptions.Add("appList", appList);

            return View();
        }


        public ActionResult Test()
        {
            var userAlias = SSOInfo.UserIdentity.UserAlias;
            var appList = UserBiz.GetUserAppInfo(userAlias);

            ViewBag.AppList = appList;
            return View();
        }
    }
}
