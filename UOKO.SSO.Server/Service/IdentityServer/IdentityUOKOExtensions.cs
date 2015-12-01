using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using Microsoft.AspNet.Identity;
namespace UOKO.SSO.Server.Service.IdentityServer
{
    public static class IdentityUOKOExtensions
    {
        public static string GetNickName(this IIdentity identity)
        {
            if (identity == null)
                throw new ArgumentNullException("identity");
            ClaimsIdentity identity1 = identity as ClaimsIdentity;
            if (identity1 != null)
                return IdentityExtensions.FindFirstValue(identity1, "nickname");
            return (string)null;
        }
    }
}