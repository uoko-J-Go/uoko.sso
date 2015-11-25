using System;
using System.Web.Mvc;
using RequireJsNet;
using UOKO.SSO.Core;
using UOKO.SSO.Server.Service;
using UOKO.SSO.Server.Utils;
using System.Web;

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

            var ex = new Exception("just a test log error");

            NLog.LogManager.GetCurrentClassLogger().Log(NLog.LogLevel.Error, ex);

            ViewBag.AppList = appList;
            return View();
        }

        public ActionResult TestUIException()
        {
            ExceptionThrow();
            return Content("ah ha, exception handled");
        }

        public ActionResult TestUnHandleException()
        {
            string t = null;
            t.ToString();
            return Content("ah ha, exception handled");
        }


        private void ExceptionThrow()
        {
            try
            {
                string t = null;
                t.ToString();
            }
            catch (Exception ex)
            {
                throw new UITipException("我只是一个可怜的UI异常...", ex);
            }
        }
        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
        }
    }
}
