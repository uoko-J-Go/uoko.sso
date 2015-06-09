using System.Web.Mvc;
using UOKO.SSO.Core;
using UOKO.SSO.Server.Service;

namespace UOKO.SSO.Server.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            var userAlias = SSOInfo.UserIdentity.UserAlias;
            var appList = UserBiz.GetUserAppInfo(userAlias);

            ViewBag.AppList = appList;
            return View();
        }

    }
}
