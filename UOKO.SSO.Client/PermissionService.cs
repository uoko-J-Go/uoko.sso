using System;
using System.Collections.Generic;
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

            // todo: 调用 api 获取权限信息


            return permission;
        }

        /// <summary>
        /// 目前使用 http runtime cache
        /// </summary>
        /// <param name="userAlias"></param>
        /// <param name="appCode"></param>
        public static void RefreshCachePermissions(string userAlias, string appCode)
        {
            var permissions = GetPermissions(userAlias, appCode);

            var cacheKey = GenerateCacheKey(userAlias, appCode);

            var ctx = HttpContext.Current;
            if (ctx != null)
            {
                ctx.Cache.Add(cacheKey, permissions, null, DateTime.Now.AddHours(8), Cache.NoSlidingExpiration,
                              CacheItemPriority.Normal, null);
            }
        }

        private static string GenerateCacheKey(string userAlias, string appCode)
        {
            var cacheKey = string.Format("alias-{0}-appcode-{1}", userAlias, appCode);
            return cacheKey;
        }
    }
}