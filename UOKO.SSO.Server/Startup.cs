using Microsoft.Owin;
using Owin;
using UOKO.SSO.Server;

[assembly: OwinStartup(typeof(Startup))]
namespace UOKO.SSO.Server
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
