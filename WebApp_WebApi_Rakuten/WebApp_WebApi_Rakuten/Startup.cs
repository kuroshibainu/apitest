using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebApp_WebApi_Rakuten.Startup))]
namespace WebApp_WebApi_Rakuten
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
