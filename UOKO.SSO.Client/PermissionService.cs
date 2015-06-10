using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Caching;

namespace UOKO.SSO.Client
{
    public static class PermissionService
    {
        /// <summary>
        /// 获取用户相应系统下所有权限
        /// 目前使用 http runtime cache
        /// </summary>
        /// <returns></returns>

        public static IEnumerable<string> GetPermissionsFromCache(string userAlias, string appCode)
        {
            // get permission from db or cache or webapi
            var cacheKey = GenerateCacheKey(userAlias, appCode);

            IEnumerable<string> permissions = null;


            var ctx = HttpContext.Current;
            if (ctx != null)
            {
                permissions = ctx.Cache.Get(cacheKey) as IEnumerable<string>;
                if (permissions == null)
                {
                    permissions = GetPermissions(userAlias, appCode);

                    // 这里默认为 8h 的缓存, 如果要刷新, 通过退出重新登陆实现缓存刷新.(在退出的时候,清空缓存)
                    ctx.Cache.Add(cacheKey, permissions, null, DateTime.Now.AddHours(8), Cache.NoSlidingExpiration,
                                  CacheItemPriority.Normal, null);
                }
            }
            else
            {
                permissions = GetPermissions(userAlias, appCode);
            }

            return permissions;
        }

        private static IEnumerable<string> GetPermissions(string userAlias, string appCode)
        {
            var permission = new List<string>();

            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            var getUserInfoApiUrl = string.Format("{0}/Funcation/GetPermissionByAliasAppCode/{1}/{2}", RelyingPartyClient.ClientInfo.PermissApiUrl, userAlias, appCode);
            var result = client.GetAsync(getUserInfoApiUrl).Result.Content.ReadAsAsync<ApiResult<IEnumerable<PermissionInfo>>>().Result;

            if (result != null)
            {
                if (result.Code == "200" && result.Data != null)
                {
                    permission = result.Data.Select(item => item.Url).ToList();
                }
                else
                {
                    // 吞掉异常... 哎 code smell
                    // throw new Exception(result.Message);
                    return permission;
                }
            }
            else
            {
                throw new Exception("api return null");
            }

            return permission;
        }

        /// <summary>
        /// 目前使用 http runtime cache
        /// </summary>
        /// <param name="userAlias"></param>
        /// <param name="appCode"></param>
        public static IEnumerable<string> RefreshCachePermissions(string userAlias, string appCode)
        {
            var permissions = GetPermissions(userAlias, appCode);

            var cacheKey = GenerateCacheKey(userAlias, appCode);

            var ctx = HttpContext.Current;
            if (ctx != null)
            {
                ctx.Cache.Add(cacheKey, permissions, null, DateTime.Now.AddHours(8), Cache.NoSlidingExpiration,
                              CacheItemPriority.Normal, null);
            }

            return permissions;
        }

        private static string GenerateCacheKey(string userAlias, string appCode)
        {
            var cacheKey = string.Format("alias-{0}-appcode-{1}", userAlias, appCode);
            return cacheKey;
        }

        private class ApiResult<T>
        {
            public string Message { get; set; }
            public string Code { get; set; }
            public T Data { get; set; }
        }

        private class PermissionInfo
        {
            public string Url { get; set; } 
        }
    }
}