using BaseDataL.Models;
using BaseDataL.Models.Lims;
using BaseDataL.Models.Planta;
using BaseDataL.Seguridad;
using BaseDataL.UnitOfWork;
using System;
using System.Configuration;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Results;

namespace Sigm_App.Controllers
{

    [RoutePrefix("api")]
    public class AprobacionesController : BaseController
    {


        public AprobacionesController(IUnitOfWork unit, UserCredentialsService userCredentials) : base(unit, userCredentials)
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

        //**********************************************************ORDEN DE SERVICIO *****************************************

        [Route("listaospend")]
        [HttpGet]
        public IHttpActionResult GetListOsPend()
        {
            string login = _userCredentialsService.Login;
            return Ok(_unit.OrdenServicio.GetListaOrden(login, 'P'));

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

            var resultado = _unit.OrdenServicio.Ejecutar_WorkFlow_Accion(orden, "sigm_os", login, 'A', "MOVIL", "0");

            if (resultado.Exitoso)
            {
                // return Json(new { success = true, message = resultado.Mensaje });

                return Ok(new
                {
                    Message = resultado.Mensaje
                });
            }
            else
            {
                //return Json(new { success = false, message = resultado.Mensaje });
                //return BadRequest("No se pudo realizar apobación");
                //return BadRequest(new
                //{
                //    Message = resultado.Mensaje
                //});


                string mensajeError = "No se pudo realizar apobación de Orden : " + resultado.Mensaje;
                return BadRequest(mensajeError);

                //return BadRequest("No se pudo realizar apobación");


            }
            //if (!_unit.OrdenServicio.Ejecutar_WorkFlow_Accion(orden, "sigm_os", login, 'A', "MOVIL", "0")) 
            //    return BadRequest("No se pudo realizar apobación");

            //return Ok(new
            //{
            //    Message = "Aprobación exitosa"
            //});


        }

        [Route("desaprobaros")]
        [HttpPost]
        public IHttpActionResult DesaprobacionOs(OrdenServicio orden)
        {


            if (!ModelState.IsValid) return BadRequest(ModelState);
            string login = _userCredentialsService.Login;

            var resultado = _unit.OrdenServicio.Ejecutar_WorkFlow_Accion(orden, "sigm_os", login, 'G', "MOVIL", "0");


            //if (!_unit.OrdenServicio.Ejecutar_WorkFlow_Accion(orden, "sigm_os" ,login, 'G', "MOVIL", "0"))
            //    return BadRequest("No se pudo realizar apobación");

            if (resultado.Exitoso)
            {
                return Ok(new
                {
                    Message = resultado.Mensaje
                });
            }
            else
            {
                string mensajeError = "No se pudo realizar desapobación de orden: " + resultado.Mensaje;
                return BadRequest(mensajeError);
            }

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
            var resultado = _unit.Consolidado.Ejecutar_WorkFlow_Accion(consolidado, "sigm_os", login, 'A', "MOVIL", "0");

            if (resultado.Exitoso)
            {
                return Ok(new
                {
                    Message = resultado.Mensaje
                });
            }
            else
            {
                string mensajeError = "No se pudo realizar apobación de consolidado: " + resultado.Mensaje;
                return BadRequest(mensajeError);
            }
        }

        [Route("desaprobarcons")]
        [HttpPost]
        public IHttpActionResult DesaaprobacionCons(Consolidado consolidado)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            string login = _userCredentialsService.Login;

            var resultado = _unit.Consolidado.Ejecutar_WorkFlow_Accion(consolidado, "sigm_os", login, 'G', "MOVIL", "0");

            if (resultado.Exitoso)
            {
                return Ok(new
                {
                    Message = resultado.Mensaje
                });
            }
            else
            {
                string mensajeError = "No se pudo realizar desapobación de consolidado: " + resultado.Mensaje;
                return BadRequest(mensajeError);
            }

        }


        //**********************************************************LIMS*****************************************


        [Route("listsolcambiopend")]
        [HttpGet]
        public IHttpActionResult GetListaSolCambioPend()
        {
            string login = _userCredentialsService.Login;
            return Ok(_unit.SolicitudCambio.GetListaSolicitudAprobacion(login));
        }

        [Route("aprobarsolcambio")]
        [HttpPost]
        public IHttpActionResult AprobacionSolCambio(SolicitudCambio solcambio)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            string login = _userCredentialsService.Login;
            var resultado = _unit.SolicitudCambio.Ejecutar_WorkFlow_Accion(solcambio, "sigm_lims", login, 'A', "MOVIL", "NO");

            if (resultado.Exitoso)
            {
                return Ok(new
                {
                    Message = resultado.Mensaje
                });
            }
            else
            {
                string mensajeError = "No se pudo realizar apobación de la solicitud de cambio: " + resultado.Mensaje;
                return BadRequest(mensajeError);
            }
        }


        //**********************************************************PLANTA*****************************************

        [Route("listpartediariopend")]
        [HttpGet]
        public IHttpActionResult GetListaParteDiario()
        {
            string login = _userCredentialsService.Login;
            return Ok(_unit.ModeloParte.GetListaParteAprobacion(login, "Parte Diario"));
        }

        [Route("listpartemensualpend")]
        [HttpGet]
        public IHttpActionResult GetListaParteMensual()
        {
            string login = _userCredentialsService.Login;
            return Ok(_unit.ModeloParte.GetListaParteAprobacion(login, "Parte Mensual"));
        }

        [Route("aprobarparte")]
        [HttpPost]
        public IHttpActionResult AprobacionModeloParte(ParteMetalurgico parte)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            string login = _userCredentialsService.Login;
            var resultado = _unit.ModeloParte.Ejecutar_WorkFlow_Accion(parte, "sigm_planta", login, Convert.ToChar(parte.EstadoDespuesAprobado), "MOVIL", "");

            if (resultado.Exitoso)
            {
                return Ok(new
                {
                    Message = resultado.Mensaje
                });
            }
            else
            {
                string mensajeError = "No se pudo realizar apobación del parte " + resultado.Mensaje;
                return BadRequest(mensajeError);
            }
        }

        [Route("rechazarparte")]
        [HttpPost]
        public IHttpActionResult RechazarModeloParte(ParteMetalurgico parte)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            string login = _userCredentialsService.Login;
            var resultado = _unit.ModeloParte.Ejecutar_WorkFlow_Accion(parte, "sigm_planta", login, 'V', "MOVIL", "");

            if (resultado.Exitoso)
            {
                return Ok(new
                {
                    Message = resultado.Mensaje
                });
            }
            else
            {
                string mensajeError = "No se pudo realizar la operación " + resultado.Mensaje;
                return BadRequest(mensajeError);
            }
        }
    }
}
 
