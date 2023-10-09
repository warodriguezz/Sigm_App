using BaseDataL.Models;
using BaseDataL.Seguridad;
using BaseDataL.UnitOfWork;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace Sigm_App.Controllers
{
    
    [RoutePrefix("api")]
    public class AprobacionesController : BaseController
    {

     
        public AprobacionesController(IUnitOfWork unit, UserCredentialsService userCredentials) : base(unit,userCredentials)
        {
            string login = _userCredentialsService.Login;
            string clave = _userCredentialsService.Clave;

            string CadenaConexion = ConfigurationManager.ConnectionStrings["BdOperaciones"].ToString();          
            string patronUserID = @"User\s+ID=[^;]+";
            string patronPassword = @"Password=[^;]+";

            string nuevaCadenaConexion = Regex.Replace(CadenaConexion, patronUserID, $"User ID={login}")
                                  .Replace(Regex.Match(CadenaConexion, patronPassword).Value, $"Password={clave}");
            _unit.CambiarCadenaConexion(nuevaCadenaConexion);
        }


        [Route("listaospend")]
        [HttpGet]
        public IHttpActionResult GetListOsPend()
        {
            string login = _userCredentialsService.Login;
            return Ok(_unit.OrdenServicio.GetListaOrden(login,'P'));
        }

        [Route("listaosapro")]
        [HttpGet]
        public IHttpActionResult GetListOsAprob()
        {
            string login = _userCredentialsService.Login;
            return Ok(_unit.OrdenServicio.GetListaOrden(login, 'A'));
        }

        [Route("aprobaros")]
        [HttpPost]
        public IHttpActionResult AprobacionOs(OrdenServicio orden)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            string login = _userCredentialsService.Login;
            if (!_unit.OrdenServicio.Ejecutar_WorkFlow_Accion(orden, login, 'A', "MOVIL", "0")) 
                return BadRequest("No se pudo realizar apobación");

            return Ok(new
            {
                Message = "Aprobación exitosa"
            });


        }

        [Route("desaprobaros")]
        [HttpPost]
        public IHttpActionResult DesaprobacionOs(OrdenServicio orden)
        {
        

            if (!ModelState.IsValid) return BadRequest(ModelState);
            string login = _userCredentialsService.Login;
            if (!_unit.OrdenServicio.Ejecutar_WorkFlow_Accion(orden, login, 'G', "MOVIL", "0"))
                return BadRequest("No se pudo realizar apobación");

            return Ok(new
            {
                Message = "Desaprobación exitosa"
            });


        }

        [Route("listaconpend")]
        [HttpGet]
        public IHttpActionResult GetListConsPend()
        {
            string login = _userCredentialsService.Login;
            return Ok(_unit.Consolidado.GetListaConsolidado(login, 'P'));
        }

        [Route("listaconapro")]
        [HttpGet]
        public IHttpActionResult GetListConsApro()
        {
            string login = _userCredentialsService.Login;
            return Ok(_unit.Consolidado.GetListaConsolidado(login, 'A'));
        }

        [Route("vercons")]
        [HttpGet, HttpPost]

        public IHttpActionResult GetDetalleCons(Consolidado consolidado)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(_unit.Consolidado.Detalle(consolidado));
        }

        [Route("aprobarcons")]
        [HttpPost]
        public IHttpActionResult AprobacionCons(Consolidado consolidado)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            string login = _userCredentialsService.Login;
            if (!_unit.Consolidado.Ejecutar_WorkFlow_Accion(consolidado, login, 'A', "MOVIL", "0"))
                return BadRequest("No se pudo realizar apobación");

            return Ok(new
            {
                Message = "Aprobación exitosa"
            });

        }

        [Route("desaprobarcons")]
        [HttpPost]
        public IHttpActionResult DesaaprobacionCons(Consolidado consolidado)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            string login = _userCredentialsService.Login;
            if (!_unit.Consolidado.Ejecutar_WorkFlow_Accion(consolidado, login, 'G', "MOVIL", "0"))
                return BadRequest("No se pudo realizar apobación");

            return Ok(new
            {
                Message = "Desaprobación exitosa"
            });

        }

    }
}
 
