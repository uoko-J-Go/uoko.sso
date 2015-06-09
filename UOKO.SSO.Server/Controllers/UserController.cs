using System.Web.Mvc;

namespace UOKO.SSO.Server.Controllers
{
    public class UserController : Controller
    {

        public ActionResult Login(string userName,string pwd)
        {

            if (userName != pwd)
            {
                return Json(new {success = false, errorMsg = "用户名/密码不正确"});
            }

            return Json(new {success = true, data = (object)null});
        }


        public ActionResult GetInfo()
        {
            return Json(new { success = true, data = new{userName = "panpan",email="panpan@uoko.com"} });
        }

    }
}
