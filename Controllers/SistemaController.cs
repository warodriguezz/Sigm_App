using BaseDataL.Seguridad;
using System.Web.Http;

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
