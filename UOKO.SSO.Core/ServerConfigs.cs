using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace UOKO.SSO.Core
{
    /// <summary>
    /// sso server 相关配置信息
    /// </summary>
    public class ServerConfigs
    {
        private readonly static MD5 _md5 = MD5.Create();
        /// <summary>
        /// 需要保证和 sso server 里的设置一致。 默认为 uoko-sso-internal
        /// </summary>
        public static string CookieName
        {
            get
            {
                var cookieNameFromConfig = ConfigurationManager.AppSettings["sso.cookieName"];
                var cookieName = string.IsNullOrEmpty(cookieNameFromConfig) ? "uoko-sso-internal" : cookieNameFromConfig;
                cookieName = string.Join("",
                                         _md5.ComputeHash(Encoding.UTF8.GetBytes(cookieName))
                                             .Select(item => item.ToString("X")));
                return cookieName;

            }
        }

        /// <summary>
        /// 需要保证和 sso server 里的设置一致。 默认为 uoko.com
        /// </summary>
        public static string CookieDomain
        {
            get
            {
                var domainFromConfig = ConfigurationManager.AppSettings["sso.cookieDomain"];
                return string.IsNullOrEmpty(domainFromConfig) ? "uoko.com" : domainFromConfig;
            }
        }

        /// <summary>
        /// 需要保证和 sso server 里的设置一致。 默认为 
        /// </summary>
        public static string TokenParamName
        {
            get { return "sso-uoko-token"; }
        }

        public static int CookieTimeoutMinutes
        {
            get
            {
                var expireMinutes = 20;
                return expireMinutes;
            }
        }
    }
}