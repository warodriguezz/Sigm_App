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
            return Ok(_unit.OrdenServicio.GetListaPendientes(login));
        }

        [Route("listaos")]
        [HttpGet, HttpPost]
        public IHttpActionResult GetListOsAll(OrdenServicio orden)
        {
            return Ok(_unit.OrdenServicio.GetLista(orden));
        }

        [Route("aprobaros")]
        [HttpPost]
        public IHttpActionResult AprobacionOs(OrdenServicio orden)
        {
            // Lógica adicional para validar los datos recibidos, si es necesario


            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!_unit.OrdenServicio.Aprobacion(orden,"A")) return BadRequest("No se pudo realizar apobación");
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
            if (!_unit.OrdenServicio.Aprobacion(orden, "D")) return BadRequest("No se pudo realizar desapobación");
            return Ok(new
            {
                Message = "Desaprobación exitosa"
            });


        }

        [Route("listaconpend")]
        [HttpGet]
        public IHttpActionResult GetListCons()
        {
            return Ok(_unit.Consolidado.GetListaPendientes());
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
            // Lógica adicional para validar los datos recibidos, si es necesario


            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!_unit.Consolidado.Aprobacion(consolidado)) return BadRequest("No se pudo realizar apobación");
            return Ok(new
            {
                Message = "Aprobación exitosa"
            });

        }
    }
}
 
