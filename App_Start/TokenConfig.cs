using Base;
using BaseDataL.Seguridad;
using BaseDataL.UnitOfWork;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.UI.WebControls;

namespace Sigm_App.App_Start
{
    public static class TokenConfig
    {
        public static void ConfigureOAuth(IAppBuilder app, HttpConfiguration config)
        {
            string accessTokenExpireMinutesStr = ConfigurationManager.AppSettings["AccessTokenExpireMinutes"];
            var unitOfWork = (IUnitOfWork)config.DependencyResolver.GetService(typeof(IUnitOfWork));
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/login"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(Convert.ToInt16(accessTokenExpireMinutesStr)),
                Provider = new CustomAuthorizationServerProvider(unitOfWork)
            };
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
        //public static void ConfigureOAuth(IAppBuilder app, HttpConfiguration config)
        //{
        //    var unitOfWork =(IUnitOfWork)config.DependencyResolver.GetService(typeof(IUnitOfWork));
        //    OAuthAuthorizationServerOptions OAuthServerOptions = new
        //    OAuthAuthorizationServerOptions()
        //    {
        //        AllowInsecureHttp = true,
        //        TokenEndpointPath = new PathString("/token"),
        //        AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
        //        Provider = new SimpleAuthorizationServerProvider(unitOfWork)
        //    };
        //    app.UseOAuthAuthorizationServer(OAuthServerOptions);
        //    app.UseOAuthBearerAuthentication(new
        //   OAuthBearerAuthenticationOptions());
        //}
    }


    public class CustomAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly IUnitOfWork _unit;

        public CustomAuthorizationServerProvider(IUnitOfWork unit)
        {
            _unit = unit;
            string ConnectionString = ConfigurationManager.ConnectionStrings["BvnSeguridadUS"].ToString();
            string clave = ConfigurationManager.AppSettings["Servicio"];
            clave = ProcesaLogin.Proteger(clave, 1);
            string nuevacadena = Funciones.ReemplazarConectionString_UsuarioPassword(ConnectionString, "usr_seguridad", clave);
            _unit.CambiarCadenaConexion(nuevacadena);
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            await Task.Factory.StartNew(() => { context.Validated(); });
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            await Task.Factory.StartNew(async () =>
            {
                using (PrincipalContext adcontext = new PrincipalContext(ContextType.Domain, "CMBSAA"))
                {

                    //byte[] key = Encoding.UTF8.GetBytes("2B7E151628AED2A6"); // 16 bytes (128 bits)
                    //byte[] iv = Encoding.UTF8.GetBytes("1B7E151628AED2A6"); // 16 bytes (128 bits)

                    byte[] key;
                    byte[] iv;

                    ProcesaLogin.GetAesValues(out iv, out key);

                    string pasword=Cryptography.DecryptString(context.Password.Trim(), key, iv);

                    // Validar las credenciales del usuario contra el servidor de Active Directory
                    bool isValid = adcontext.ValidateCredentials(context.UserName, pasword);

                    if (!isValid)
                    {
                        context.SetError("invalid_grant1", "Invalid user credentials.");
                        return;
                    }
                }

                //Continuar con la validación en la base de datos de la aplicación
                var user = _unit.Users.ValidacionesUser(context.UserName, context.Password);
                if (user == null)
                {
                    context.SetError("invalid_grant2", "Wrong user or password.");
                    return;
                }

                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim("sub", context.UserName));
                identity.AddClaim(new Claim("role", user.Roles.ToString()));

                identity.AddClaim(new Claim("login", user.Login.ToString()));

               
                context.Validated(identity);
            });
        }
    }

    //public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    //{
    //    private readonly IUnitOfWork _unit;
    //    public SimpleAuthorizationServerProvider(IUnitOfWork unit)
    //    {
    //        _unit = unit;
    //    }
    //    public override async Task
    //   ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
    //    {
    //        await Task.Factory.StartNew(() => { context.Validated(); });
    //    }
    //    public override async Task
    //   GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
    //    {
    //        await Task.Factory.StartNew(() =>
    //        {

    //            var user = _unit.Users.ValidaterUser(context.UserName, context.Password);
    //            if (user == null)
    //            {
    //                context.SetError("invalid_grant", "Wrong user or password .");
    //                return;
    //            }
    //            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
    //            identity.AddClaim(new Claim("sub", context.UserName));
    //            identity.AddClaim(new Claim("role", "user"));
    //            context.Validated(identity);
    //        });
    //    }
    //}
}