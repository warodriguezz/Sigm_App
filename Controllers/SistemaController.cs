using BaseDataL.Models;
using BaseDataL.Seguridad;
using BaseDataL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.UI.WebControls;

namespace Sigm_App.Controllers
{
    public class SistemaController : ApiController
    {
        protected readonly UserCredentialsService _userCredentialsService;

        public SistemaController( UserCredentialsService userCredentialsService)  
        {
            _userCredentialsService = userCredentialsService;
        }

        [Route("menu")]
        [Authorize]
        [HttpGet]
        public IHttpActionResult GetMenu()
        {
            string login = _userCredentialsService.Login;
            var menuJson = ControlMenu.GetMenu(login);

            if (menuJson != null)
            {
                return Json(menuJson); ;
            }
            else
            {
               return NotFound();
            }

        }
    }
}
