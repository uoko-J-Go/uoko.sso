using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using UOKO.SSO.Core;
using UOKO.SSO.Models;
using UOKO.SSO.Server.Service;
using UOKO.SSO.Server.Utils;

namespace UOKO.SSO.Server.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        /// <summary>
        /// 检验用户身份认证状态, 如果用户身份已经登陆
        ///     检查 returnUrl, 如果为 null 则跳转 Home 页    
        ///     否者 如果是域内 url 则直接跳转回该 url
        ///         如果非域内 url 则附加相应的 token 进行跳转
        /// 否者跳转登陆页,让用户进行登陆
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="appKey"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string appKey)
        {
            if (SSOInfo.UserIdentity != null)
            {
                // 登陆状态已经认证通过

                if (string.IsNullOrWhiteSpace(returnUrl))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {

                    // 如果存在 returnUrl 进行判断
                    // 如果这是一个非同域, 那么需要生成token附加链接,
                    // 否者直接跳转进行到同域进行认证登陆.
                    var sameDomain = new Uri(returnUrl).Host.EndsWith(ServerConfig.CookieDomain);
                    if (!sameDomain)
                    {
                        // issue token
                        var ticket = new CasTicket
                                     {
                                         AppKey = appKey,
                                         UserAlias = SSOInfo.UserIdentity.UserAlias,
                                         UserName = SSOInfo.UserIdentity.Name,
                                     };
                        var token = CacheTickets.IssueToken(ticket);
                        returnUrl = GetValidateTokenUrl(returnUrl, token);
                    }

                    return Redirect(returnUrl);
                }

            }


            // 没有身份信息,重新登陆

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.AppKey = appKey;

            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl, string appKey)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userInfo = UserBiz.GetUserInfo(model.UserName, model.Password);
            if (userInfo == null)
            {
                // 验证不通过
                ModelState.AddModelError("", "提供的用户名或密码不正确。");
                return View(model);
            }

            // 验证通过
            var cookieInfo = new SSOCookieInfo() {Alias = userInfo.Alias, Name = userInfo.Name};
            SSOAuthentication.SetAuthCookie(cookieInfo, ServerConfig.CookieName, ServerConfig.CookieDomain);

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return RedirectToAction("Index", "Home");
            }

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                // 如果存在 returnUrl 进行判断

                var sameDomain = true;
                try
                {
                    sameDomain = new Uri(returnUrl).Host.EndsWith(ServerConfig.CookieDomain);
                }
                catch
                {
                }

                // 如果这是一个非同域, 那么需要生成token附加链接,
                // 否者直接跳转进行到同域进行认证登陆.
                if (!sameDomain)
                {
                    // issue token
                    var ticket = new CasTicket()
                                 {
                                     AppKey = appKey,
                                     UserAlias = userInfo.Alias,
                                     UserName = userInfo.Name,
                                 };
                    var token = CacheTickets.IssueToken(ticket);
                    returnUrl = GetValidateTokenUrl(returnUrl, token);
                }

            }

            return Redirect(returnUrl);
        }

        [AllowAnonymous]
        public ActionResult LogOff(string returnUrl)
        {
            HttpContext.Request.GetOwinContext().Authentication.SignOut();
            SSOAuthentication.SignOut(ServerConfig.CookieDomain);
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// 通过校验token,获取用户身份
        /// </summary>
        /// <param name="token"></param>
        /// <param name="appKey">token的ticket信息需要和appkey对应,防止非token发起方获取信息</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult ValidateToken(string token, string appKey)
        {
            var casTicket = CacheTickets.ValidateToken(token, appKey);

            return Json(casTicket);
        }

        /// <summary>
        /// 如果已经包含 认证token 则替换
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="ticketToken"></param>
        /// <returns></returns>
        private static string GetValidateTokenUrl(string returnUrl, string ticketToken)
        {
            if (returnUrl.Contains(ServerConfig.TokenParamName))
            {
                // remove token param
                var regex = new Regex(string.Format(@"{0}=.*?(&|$)", ServerConfig.TokenParamName),
                                      RegexOptions.IgnoreCase);
                returnUrl = regex.Replace(returnUrl,
                                          string.Format(@"{0}={1}", ServerConfig.TokenParamName, ticketToken));
                return returnUrl;
            }
            else
            {

                var connectorChar = returnUrl.IndexOf('?') == -1 ? "?" : "&";

                var redirectUrl = string.Format(@"{0}{1}{2}={3}", returnUrl,
                                                connectorChar,
                                                ServerConfig.TokenParamName,
                                                ticketToken);
                return redirectUrl;
            }
        }

    }
}