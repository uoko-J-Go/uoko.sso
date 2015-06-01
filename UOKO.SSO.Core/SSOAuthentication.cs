using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace UOKO.SSO.Core
{
    /// <summary>
    /// 按照特定方式读写相关 cookie
    /// </summary>
    public sealed class SSOAuthentication
    {
        public static SSOCookieInfo GetAuthCookieInfo(string cookieName)
        {
            SSOCookieInfo cookieInfo = null;

            try
            {
                var ssoCookie = CurrentRequest.Cookies[cookieName];
                if (ssoCookie != null)
                {
                    var decryptValue = DecryptSSOCookieValue(ssoCookie.Value);
                    cookieInfo = JsonConvert.DeserializeObject<SSOCookieInfo>(decryptValue);
                }
            }
            catch (Exception ex)
            {
                // todo: log exception
            }

            return cookieInfo;
        }

        public static ClaimsPrincipal GenerateClaimsPrincipal(SSOCookieInfo cookieInfo)
        {
            if (cookieInfo == null)
            {
                return null;
            }

            // 有身份信息,构建基础身份信息
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, cookieInfo.Name),
                new Claim(ClaimTypes.NameIdentifier, cookieInfo.Alias),
            };
            var identity = new ClaimsIdentity(claims, "uoko-sso-internal");
            var principal = new ClaimsPrincipal(identity);
            return principal;
        }

        public static void SetAuthCookie(SSOCookieInfo cookieInfo, string cookieName, string cookieDomain = null)
        {
            var cookieValue = EncryptSSOCookieValue(cookieInfo);

            var cookie = new HttpCookie(cookieName, cookieValue)
                         {
                             HttpOnly = true,
                             Expires = DateTime.Now.AddMinutes(ServerConfigs.CookieTimeoutMinutes),
                         };

            if (!string.IsNullOrWhiteSpace(cookieDomain))
            {
                cookie.Domain = ServerConfigs.CookieDomain;
            }

            if (CurrentResponse != null)
            {
                CurrentResponse.SetCookie(cookie);
            }
        }

        public static void RemoveCookie(string cookieName,string cookieDomain = null)
        {
            var cookie = new HttpCookie(cookieName)
                         {
                             Expires = DateTime.Now.AddDays(-1),
                             HttpOnly = true,
                             Domain = cookieDomain
                         };

            if (CurrentResponse != null)
            {
                CurrentResponse.SetCookie(cookie);
            }
        }




        /// <summary>
        /// remove sso cookie
        /// </summary>
        public static void SignOut()
        {
            RemoveCookie(ServerConfigs.CookieName, ServerConfigs.CookieDomain);
        }

        private static HttpResponse CurrentResponse
        {
            get
            {
                var current = HttpContext.Current;
                return current != null ? current.Response : null;
            }
        }

        private static HttpRequest CurrentRequest
        {
            get
            {
                var current = HttpContext.Current;
                return current != null ? current.Request : null;
            }
        }

        private static readonly byte[] AesKey = Encoding.UTF8.GetBytes("f3cfe0fd9c904f83965441ee");
        private static readonly byte[] AesIV = Encoding.UTF8.GetBytes("uokoiviv");

        private static string DecryptSSOCookieValue(string cookieValue)
        {
            using (var des = new TripleDESCryptoServiceProvider() {Key =AesKey , IV =AesIV})
            {
                var cookieValueBytes =  Convert.FromBase64String(cookieValue);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, des.CreateDecryptor(),
                                                     CryptoStreamMode.Write))
                    {
                        cs.Write(cookieValueBytes, 0, cookieValueBytes.Length);
                        cs.FlushFinalBlock();
                    }

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

        private static string EncryptSSOCookieValue(SSOCookieInfo cookieInfo)
        {
            var cookieValue = JsonConvert.SerializeObject(cookieInfo);


            using (var des = new TripleDESCryptoServiceProvider() { Key = AesKey, IV = AesIV })
            {
                var cookieValueBytes = Encoding.UTF8.GetBytes(cookieValue);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, des.CreateEncryptor(),
                                                     CryptoStreamMode.Write))
                    {
                        cs.Write(cookieValueBytes, 0, cookieValueBytes.Length);
                        cs.FlushFinalBlock();
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

    }

}