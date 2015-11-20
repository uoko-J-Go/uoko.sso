using System.Collections.Generic;
using System.Web;
using IdentityServer3.Core.Models;
using UOKO.SSO.Server.Utils;

namespace UOKO.SSO.Server.Service.IdentityServer
{
    public class Clients
    {
        private static string configPath = HttpContext.Current.Server.MapPath("~/Configs/IdentityServer/ClientsConfig.json");
        public static IEnumerable<Client> Get()
        {
//            return new[]
//                   {
//                       new Client
//                       {

//                           ClientName = "波多系统",
//                           ClientId = "etadmin",
//                           Flow = Flows.AuthorizationCode,
//                           RequireConsent = false,
//                           RedirectUris = new List<string>
//                                          {
//                                              "http://etadmin.uoko.ioc/",
//                                              "http://sso.one.com/",
//                                              "http://two.domain.com/",
//                                              "http://sso.three.com/",
//                                          },

//                           PostLogoutRedirectUris = new List<string>
//                                                    {
//                                                        "http://sso.domain.com/",
//                                                        "http://sso.one.com/",
//                                                        "http://two.domain.com/",
//                                                        "http://sso.three.com/",
//                                                    },

//                           AllowedScopes = new List<string>
//                                           {
//                                               "openid",
//                                               "profile",
//                                               "roles",
//                                               "sampleApi"
//                                           },
//                       },


//                       new Client
//                       {
//                           ClientName = "系统配置权限系统",
//                           ClientId = "systemset",
//                           Flow = Flows.Hybrid,
//                           RequireConsent = false,
//                           RedirectUris = new List<string>()
//                                          {
//                                              "http://mgmt.test.uoko.ioc/",
//                                          },
//                           AllowedScopes = new List<string>
//                                           {
//                                               "openid",
//                                               "profile",
//                                           },
//                       },

//                       new Client
//                       {
//                           ClientName = "系统配置权限系统-api",
//                           ClientId = "systemset-api",
//                           Flow = Flows.ClientCredentials,

//                           ClientSecrets = new List<Secret>
//                                           {
//                                               new Secret("secret-for-systemset-api".Sha256())
//                                           }
//                       }
//                   };
            return JsonConfigHelper<List<Client>>.Load(configPath);
        }
    }
}