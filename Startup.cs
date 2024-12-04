using Microsoft.Owin;
using Owin;
using Sigm_App.App_Start;
using System.Web.Http;
using Microsoft.Owin.Cors;
using System.Reflection;

[assembly: OwinStartup(typeof(Sigm_App.Startup))]

namespace Sigm_App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Para obtener más información sobre cómo configurar la aplicación, visite https://go.microsoft.com/fwlink/?LinkID=316888
            var config = new HttpConfiguration();
            app.UseCors(CorsOptions.AllowAll);
            DIConfig.ConfigureInjector(config);
            TokenConfig.ConfigureOAuth(app, config);
             RouteConfig.Register(config);
            app.UseWebApi(config);
          
        }
    }
}
