using System.Web.Mvc;
using UOKO.SSO.Core;
using UOKO.SSO.Service;

namespace UOKO.SSO.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            var userAlias = Current.UserIdentity.UserAlias;
            var appList = UserBiz.GetUserAppInfo(userAlias);

            ViewBag.AppList = appList;
            return View();
        }

    }
}
