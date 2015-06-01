using System.Security.Claims;
using System.Web;

namespace UOKO.SSO.Core
{
    public static class Current
    {

        public static CommonIdentity UserIdentity
        {
            get
            {
                var context = HttpContext.Current;
                if (context == null)
                {
                    return null;
                }

                CommonIdentity commonIdentity = null;

                if (context.User != null)
                {
                    var claimsIdentity = context.User.Identity as ClaimsIdentity;
                    if (claimsIdentity != null
                        && claimsIdentity.IsAuthenticated)
                    {
                        commonIdentity = new CommonIdentity(claimsIdentity);
                    }
                }

                return commonIdentity;
            }
        }

    }

}