using BaseDataL.Models;
using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.DirectoryServices.AccountManagement;
using BaseDataL.Seguridad;
using System.Security.Claims;
using System.Web;
using System.Numerics;
using System.Web.UI.WebControls;

namespace Sigm_App.Controllers
{
    public class AutorizacionController : ApiController
    {

        protected readonly UserCredentialsService _userCredentialsService;

        public AutorizacionController(UserCredentialsService userCredentialsService)
        {
            _userCredentialsService = userCredentialsService;
        }

        [Authorize]
        [Route("InfoUsuario")]
        [HttpGet]
        public IHttpActionResult InfoUsuario()
        {
            var user = User as ClaimsPrincipal;

            if (user != null)
            {
                string codigoLogin = user.FindFirst("login")?.Value;
                UsuarioSigm usauriosigm = ProcesaLogin.UsuarioInfo(codigoLogin);
                if (usauriosigm != null)
                {
                    string clavesegura = ProcesaLogin.EjecutarUfnProteger(usauriosigm.Clave, 1) + Convert.ToChar(65 + DateTime.Now.Month);
                    
                    // Guardar los valores en el servicio
                    _userCredentialsService.Login = usauriosigm.Login;
                    _userCredentialsService.Clave = clavesegura;

                    return Ok(new { Mensaje = "Usuario válido", Usuario = usauriosigm });
                }
                else
                    return BadRequest("No se pudo Obtener información del usuario");
            }
            return BadRequest("No se pudo Obtener información de Autorizacion");
        }

        //[Route("ClaveUsuario")]
        //[HttpPost]
        //public IHttpActionResult Encriptar(User acceso)
        //{
        //    byte[] key = Encoding.UTF8.GetBytes("2B7E151628AED2A6"); // 16 bytes (128 bits)
        //    byte[] iv = Encoding.UTF8.GetBytes("1B7E151628AED2A6"); // 16 bytes (128 bits)

        //    string passwordacceso = acceso.Password;
        //    string password;

        //    password = EncryptString(passwordacceso, key, iv);
        //    return Ok(password);
        //}

        //[Route("ValidarUsuario")]
        //[HttpPost]
        //public IHttpActionResult ValidarUsuario(User acceso)
        //{

        //    byte[] key = Encoding.UTF8.GetBytes("2B7E151628AED2A6"); // 16 bytes (128 bits)
        //    byte[] iv = Encoding.UTF8.GetBytes("1B7E151628AED2A6"); // 16 bytes (128 bits)
        //    //byte[] iv = GenerateRandomIV();

        //    // Desencriptar la contraseña recibida en el cuerpo de la solicitud
        //    string password = DecryptString(acceso.Password, key, iv);


        //    // Crear una conexión al dominio de Active Directory
        //    using (PrincipalContext context = new PrincipalContext(ContextType.Domain, "CMBSAA"))
        //    {
        //        // Validar las credenciales del usuario contra el servidor de Active Directory
        //        bool isValid = context.ValidateCredentials(acceso.Usuario, password);

        //        if (isValid)
        //        {

        //            string ConnectionString = ConfigurationManager.ConnectionStrings["BvnSeguridadUS"].ToString();

        //            //Devolver usuario y clave de SQL para la conexion a la BD
        //            string usuarioclavesigm = ProcesaLogin.UsuarioClaveSigm(ConnectionString, acceso.Usuario);
        //            if (usuarioclavesigm != null && usuarioclavesigm != "")
        //            {
        //                //Devolver info adicional de usuario
        //                UsuarioSigm usauriosigm = ProcesaLogin.UsuarioInfo(ConnectionString, acceso.Usuario);
        //                if (usauriosigm != null)

        //                    //Cambiar la cadena de conexion por el usuario asignado

        //                    return Ok(new { Mensaje = "Usuario válido", Usuario = usauriosigm });
        //                else
        //                    return BadRequest("No se pudo Obtener información del usuario");
        //            }
        //            else
        //                return BadRequest("No se pudo realizar validacion SIGM");
        //        }
        //        else
        //        {
        //            // Usuario inválido
        //            return Unauthorized();
        //        }
        //    }
        //}
    }
}

