using System;
using System.Web;
using System.Web.Caching;
using UOKO.SSO.Core;

namespace UOKO.SSO.Server.Utils
{
    /// <summary>
    /// ticket 缓存,方便以后移植到其他存储上,目前使用 webcache 来存储
    /// </summary>
    public class CacheTickets
    {

        public static CasTicket GetTicket(string key)
        {
            if (HttpContext.Current == null || HttpContext.Current.Cache == null)
            {
                return null;
            }

            var ticket = HttpContext.Current.Cache.Get(key) as CasTicket;
            return ticket;
        }

        public static CasTicket RemoveTicket(string key)
        {
            if (HttpContext.Current == null || HttpContext.Current.Cache == null)
            {
                return null;
            }

            var ticket = HttpContext.Current.Cache.Remove(key) as CasTicket;
            return ticket;
        }

        public static CasTicket AddCache(string token, CasTicket ticket, int exprieTimeMinutes = 1)
        {
            if (HttpContext.Current == null || HttpContext.Current.Cache == null)
            {
                return null;
            }

            var exprieTime = DateTime.Now.AddMinutes(exprieTimeMinutes);

            HttpContext.Current.Cache.Add(token, ticket, null, exprieTime, Cache.NoSlidingExpiration,
                CacheItemPriority.Normal, null);
            return ticket;
        }

        /// <summary>
        /// 生成 ticket ,对应 token 缓存起来. 发布 token 以供 client 验证.
        /// </summary>
        public static string IssueToken(CasTicket ticket)
        {
            var token = "ST-" + Guid.NewGuid().ToString("N") + "-cas";

            AddCache(token, ticket);
            return token;
        }

        /// <summary>
        /// 校验用户 token 合法性
        /// </summary>
        public static CasTicket ValidateToken(string token, string appKey)
        {
            var ticket = GetTicket(token);
            RemoveTicket(token);

            if (ticket == null)
            {
                return null;
            }

            if (ticket.AppKey != appKey)
            {
                return null;
            }

            return ticket;
        }
    }

}
