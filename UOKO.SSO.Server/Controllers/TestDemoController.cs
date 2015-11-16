using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UOKO.SSO.Server.Controllers
{
    public class TestDemoController : Controller
    {
        [Authorize]
        // GET: TestDemo
        public ActionResult Index()
        {
            return View();
        }
    }
}