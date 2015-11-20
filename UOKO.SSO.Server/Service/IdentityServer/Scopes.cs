using System.Collections.Generic;
using System.Web;
using IdentityServer3.Core.Models;
using UOKO.SSO.Server.Utils;

namespace UOKO.SSO.Server.Service.IdentityServer
{
    public static class Scopes
    {
        private static string configPath = HttpContext.Current.Server.MapPath("~/Configs/IdentityServer/ScopesConfig.json");
        public static IEnumerable<Scope> Get()
        {
            var scopes = JsonConfigHelper<List<Scope>>.Load(configPath);
            scopes.AddRange(StandardScopes.All);
            return scopes;
        }
    }
}
