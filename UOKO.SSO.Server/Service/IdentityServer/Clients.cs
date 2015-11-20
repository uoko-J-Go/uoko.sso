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
<<<<<<< HEAD
            return JsonConfigHelper<List<Client>>.Load(configPath);
=======
            return new[]
            {
                new Client
                {
                    
                    ClientName = "波多系统",
                    ClientId = "etadmin",
                    Flow = Flows.Implicit,
                    RequireConsent=false,
                    RedirectUris = new List<string>
                    {
                        "http://etadmin.uoko.ioc/",
                        "http://sso.one.ioc/",
                        "http://two.uoko.ioc/",
                        "http://sso.three.ioc/",
                    },

                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://sso.uoko.ioc/",
                        "http://sso.one.ioc/",
                        "http://two.uoko.ioc/",
                        "http://sso.three.ioc/",
                        "http://etadmin.uoko.ioc/",
                    },
                    
                    AllowedScopes = new List<string>
                    {
                        "openid",
                        "profile",
                        "roles",
                        "sampleApi"
                    },
                },
                new Client
                {
                    ClientName = "MVC Client (service communication)",
                    ClientId = "mvc_service",
                    Flow = Flows.ClientCredentials,

                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = new List<string>
                    {
                        "sampleApi"
                    }
                }
            };
>>>>>>> b600961349faf2d743e6d5fe8c38db198d9a9712
        }
    }
}
