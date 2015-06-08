using System.Security.Claims;

namespace UOKO.SSO.Core
{
    /// <summary>
    /// 方便获取 alias 这个唯一用户标识
    /// </summary>
    public class SSOIdentity
    {
        public ClaimsIdentity ClaimsIdentity { get; private set; }

        public SSOIdentity(ClaimsIdentity claimsIdentity)
        {
            ClaimsIdentity = claimsIdentity;
        }

        public string UserAlias
        {
            get
            {
                if (ClaimsIdentity == null)
                {
                    return null;
                }

                var claim = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                return claim == null ? null : claim.Value;
            }
        }

        public string Name
        {
            get { return ClaimsIdentity == null ? null : ClaimsIdentity.Name; }
        }

    }
}