using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UOKO.SSO.Core
{
    /// <summary>
    /// sso client 相关配置信息
    /// </summary>
    public class ClientConfigs
    {
        private readonly static MD5 _md5 = MD5.Create();

        /// <summary>
        /// 传递给 sso server，以标示为合法调用方
        /// 对应 AppSys 中的 AppCode
        /// </summary>
        public static string AppKey
        {
            get { return ConfigurationManager.AppSettings["sso.appKey"]; }
        }

        /// <summary>
        /// sso server 的 url
        /// </summary>
        public static string ServerUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["sso.serverUrl"];
            }
        }

        /// <summary>
        /// 本地自定义 cookieName
        /// </summary>
        public static string CookieName
        {
            get
            {
                var cookieName = ConfigurationManager.AppSettings["identity.cookieName"];
                cookieName = string.Join("",
                                    _md5.ComputeHash(Encoding.UTF8.GetBytes(cookieName))
                                        .Select(item => item.ToString("X")));
                return cookieName;
            }
        }
    }
}