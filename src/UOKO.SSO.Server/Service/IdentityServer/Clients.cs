using System.Collections.Generic;
using System.Configuration;
using System.Web;
using IdentityServer3.Core.Models;
using UOKO.SSO.Server.Utils;

namespace UOKO.SSO.Server.Service.IdentityServer
{
    public class Clients
    {
        public static IEnumerable<ClientExtention> Get()
        {
            var envirConfig = ConfigurationManager.AppSettings["envir.useConfig"];
            var configFilePath =
                HttpContext.Current.Server.MapPath(string.Format("~/Configs/IdentityServer/{0}.ClientsConfig.json",
                    envirConfig));
            var clients = JsonConfigHelper<List<ClientExtention>>.Load(configFilePath);
            return clients;
        }
    }

    public class ClientExtention : Client
    {
        public string Description { get; set;}
    }
}