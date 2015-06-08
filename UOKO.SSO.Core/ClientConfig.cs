using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UOKO.SSO.Core
{
    /// <summary>
    /// sso client 相关配置信息
    /// </summary>
    public class ClientConfig
    {
        /// <summary>
        /// sso client 相关配置信息
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="ssoServerUrl"></param>
        /// <param name="serverCookieDomain"></param>
        /// <param name="permissApiUrl">权限系统 api 地址</param>
        /// <param name="localCookieName">如果是跨域应用,需要指定本地 cookieName </param>
        public ClientConfig(string appKey, string ssoServerUrl, string serverCookieDomain, string permissApiUrl,
                            string localCookieName = null)
        {
            AppKey = appKey;
            ServerUrl = ssoServerUrl;
            ServerCookieDomain = serverCookieDomain;
            PermissApiUrl = permissApiUrl;

            LocalCookieName = HashName(localCookieName);
        }

        private static readonly MD5 _md5 = MD5.Create();

        /// <summary>
        /// 传递给 sso server，以标示为合法调用方
        /// 对应 AppSys 中的 AppCode
        /// </summary>
        public string AppKey { get; private set; }

        /// <summary>
        /// sso server 的 url
        /// </summary>
        public string ServerUrl { get; private set; }

        public string ServerCookieDomain { get; private set; }

        /// <summary>
        /// 本地自定义 cookieName
        /// </summary>
        public string LocalCookieName { get; private set; }

        private static string HashName(string cookieName)
        {
            if (string.IsNullOrEmpty(cookieName))
            {
                return null;
            }

            var hashName = string.Join("",
                                       _md5.ComputeHash(Encoding.UTF8.GetBytes(cookieName))
                                           .Select(item => item.ToString("X")));
            return hashName;
        }



        public string PermissApiUrl { get; private set; }

    }
}