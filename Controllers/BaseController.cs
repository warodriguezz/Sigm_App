using BaseDataL.Seguridad;
using BaseDataL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sigm_App.Controllers
{
    [Authorize]
    public class BaseController : ApiController
    {
        protected readonly IUnitOfWork _unit;

        protected readonly UserCredentialsService _userCredentialsService;

        public BaseController(IUnitOfWork unit, UserCredentialsService userCredentials)
        {
            _unit = unit;
            _userCredentialsService = userCredentials;
        }
    }
}
