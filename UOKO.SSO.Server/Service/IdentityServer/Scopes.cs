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

            // todo: after-remove 调试方便, 先不走配置
            scopes = new List<Scope>
                         {
                             new Scope
                             {
                                 Enabled = true,
                                 Name = "roles",
                                 Type = ScopeType.Identity,
                                 Claims = new List<ScopeClaim>
                                          {
                                              new ScopeClaim("role")
                                          }
                             },

                             new Scope
                             {
                                 Name = "systemset-api",
                                 DisplayName = "系统配置权限系统-api",
                                 Description = "Access to a sample API",
                                 Type = ScopeType.Resource,
                             }
                         };

            scopes.AddRange(StandardScopes.All);
            return scopes;
        }   
    }
}