using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Dgp.Service.Rest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HandshakeController : ControllerBase
    {
        [HttpGet]
        public ActionResult<bool> Get()
        {
            return true;
        }
    }
}
