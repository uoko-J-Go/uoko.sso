using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace UOKO.SSO.Server.Service.IdentityServer
{
    public class Clients
    {
        public static IEnumerable<Client> Get()
        {
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
        }
    }
}
