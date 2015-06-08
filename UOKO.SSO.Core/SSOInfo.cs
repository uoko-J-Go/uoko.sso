using System.Security.Claims;
using System.Web;

namespace UOKO.SSO.Core
{
    public static class SSOInfo
    {
        public static SSOIdentity UserIdentity
        {
            get
            {
                var context = HttpContext.Current;
                if (context == null)
                {
                    return null;
                }

                SSOIdentity commonIdentity = null;

                if (context.User != null)
                {
                    var claimsIdentity = context.User.Identity as ClaimsIdentity;
                    if (claimsIdentity != null
                        && claimsIdentity.IsAuthenticated)
                    {
                        commonIdentity = new SSOIdentity(claimsIdentity);
                    }
                }

                return commonIdentity;
            }
        }

    }
}