using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using RequireJsNet;
using UOKO.Framework.Core.Logging;
using UOKO.SSO.Core;
using UOKO.SSO.Server.Service;
using UOKO.SSO.Server.Utils;

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
            Logger.Log("title-jiajun-test",LogLevel.Error, ex.ToString());

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


        public ActionResult GetUser()
        {
            using (var db = new TestDbContext())
            {
                var users = db.User.Where(item => item.Age > 10).ToList();

                return CustomerJson(users, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddUser(UserInfo user)
        {
            using (var db = new TestDbContext())
            {
                db.User.Add(user);
                db.SaveChanges();
            }

            return Content("GetUser");
        }


        public class TestDbContext : DbContext
        {
            public TestDbContext()
                : base("DefaultConnection")
            {
            }

            public IDbSet<UserInfo> User { get; set; }


            protected override void OnModelCreating(DbModelBuilder builder)
            {
                base.OnModelCreating(builder);
            }
        }

        public class UserInfo
        {
            public int Id { get; set; }
            public string Name { get; set; } 
            public int Age { get; set; } 
        }
    }
}
