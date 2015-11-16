using IdentityServer3.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSO.Domain.IdentityServer
{
    public class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new[]
            {
                new Client
                {
                    ClientName = "MVC Client",
                    ClientId = "mvc",
                    Flow = Flows.Implicit,
                    RequireConsent=false,
                    RedirectUris = new List<string>
                    {
                        "http://sso.domain.com/",
                        "http://sso.one.com/",
                        "http://two.domain.com/",
                        "http://sso.three.com/",
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://sso.domain.com/",
                        "http://sso.one.com/",
                        "http://two.domain.com/",
                        "http://sso.three.com/",
                    },
                    //AllowedCorsOrigins = new List<string>
                    //{
                    //    "http://sso.domain.com/",
                    //    "http://sso.one.com/",
                    //    "http://two.domain.com/",
                    //},
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
