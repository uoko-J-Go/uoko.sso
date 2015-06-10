using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using UOKO.SSO.Core;

namespace UOKO.SSO.Client
{
    public static class IdentityExtension
    {
        /// <summary>
        /// 判断用户是否有 某 FuncCode 的操作权限
        /// 目前不支持跨系统查询权限,只支持查询本系统下的权限.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="code"></param>
        /// <returns></returns>

        public static bool HasPermission(this SSOIdentity identity, string code)
        {
            if (identity == null || string.IsNullOrEmpty(code))
            {
                return false;
            }

            var alias = identity.UserAlias;

            var permissions = PermissionService.GetPermissionsFromCache(alias, RelyingPartyClient.ClientInfo.AppKey)
                              ?? new List<string>();
            var hasPermission = permissions.Any(item => string.Equals(item, code, StringComparison.OrdinalIgnoreCase));
            return hasPermission;
        }

        public static string FindFirstClaimValue(this ClaimsIdentity claimsIdentity, string claimType)
        {
            if (claimsIdentity == null
                || claimType == null)
            {
                return null;
            }

            var claim = claimsIdentity.FindFirst(claimType);
            return claim == null ? null : claim.Value;
        }

        /// <summary>
        /// 获取用户所拥有的权限信息
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetPermissions(this SSOIdentity identity)
        {
            if (identity == null)
            {
                return new List<string>();
            }

            var alias = identity.UserAlias;

            var permissions = PermissionService.GetPermissionsFromCache(alias, RelyingPartyClient.ClientInfo.AppKey)
                              ?? new List<string>();
            return permissions;
        }


        /// <summary>
        /// 获取用户所拥有的权限信息
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>

        public static IEnumerable<string> RefreshPermissions(this SSOIdentity identity)
        {
            if (identity == null)
            {
                return new List<string>();
            }

            var alias = identity.UserAlias;

            var permissions = PermissionService.RefreshCachePermissions(alias, RelyingPartyClient.ClientInfo.AppKey)
                              ?? new List<string>();
            return permissions;
        }
    }
}