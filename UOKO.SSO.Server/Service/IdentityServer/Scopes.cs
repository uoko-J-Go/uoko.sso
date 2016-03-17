using System.Collections.Generic;
using System.Configuration;
using System.Web;
using IdentityServer3.Core.Models;
using UOKO.SSO.Server.Utils;

namespace UOKO.SSO.Server.Service.IdentityServer
{
    public static class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            var envirConfig = ConfigurationManager.AppSettings["envir.useConfig"];
            var configFilePath =
                HttpContext.Current.Server.MapPath(string.Format("~/Configs/IdentityServer/{0}.ScopesConfig.json",
                    envirConfig));
            var scopes = JsonConfigHelper<List<Scope>>.Load(configFilePath);
            scopes.AddRange(StandardScopes.All);
            return scopes;
        }   
    }
}