using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace UOKO.SSO.Server.Service.IdentityServer
{
    public static class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            var scopes = new List<Scope>
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