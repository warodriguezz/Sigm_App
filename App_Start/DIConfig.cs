using BaseDataL.Repositories.Dapper;
using BaseDataL.Seguridad;
using BaseDataL.UnitOfWork;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using System.Configuration;
using System.Web.Http;

namespace Sigm_App.App_Start
{
    public class DIConfig
    {
        public static void ConfigureInjector(HttpConfiguration config)
        {
            var container = new Container();
            container.Options.ResolveUnregisteredConcreteTypes = true;
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Register<UserCredentialsService>(Lifestyle.Singleton);
            container.Register<IUnitOfWork>(() => new
            OperacionesUnitOfWork(ConfigurationManager.ConnectionStrings["BvnSeguridadUS"].ToString()));
            container.Verify();
            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }

    }
}
