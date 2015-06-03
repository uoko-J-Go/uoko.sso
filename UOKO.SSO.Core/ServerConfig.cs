using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UOKO.SSO.Core
{
    /// <summary>
    /// sso server 相关配置信息
    /// </summary>
    public class ServerConfig
    {
        private static readonly MD5 _md5 = MD5.Create();

        /// <summary>
        /// 默认为 uoko-sso-internal
        /// </summary>
        public static string CookieName
        {
            get
            {
                var cookieName = string.Join("",
                                             _md5.ComputeHash(Encoding.UTF8.GetBytes("uoko-sso-internal"))
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
        /// 默认为 sso-uoko-token
        /// </summary>
        public static string TokenParamName
        {
            get { return "sso-uoko-token"; }
        }

    }
}