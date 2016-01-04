using System.Collections.Generic;
using System.Web;
using IdentityServer3.Core.Models;
using UOKO.SSO.Server.Utils;

namespace UOKO.SSO.Server.Service.IdentityServer
{
    public class Clients
    {
        private static string configPath = HttpContext.Current.Server.MapPath("~/Configs/IdentityServer/ClientsConfig.json");
        public static IEnumerable<ClientExtention> Get()
        {
            var clients = JsonConfigHelper<List<ClientExtention>>.Load(configPath);
            return clients;
        }
    }

    public class ClientExtention : Client
    {
        public string Description { get; set;}
    }
}