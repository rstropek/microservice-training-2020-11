using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreMicroserviceSample.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Find out wheteher the API is running
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetHealth")]
        public string Get() => DateTime.UtcNow.ToString() + "I'm alive ;)";
    }
}