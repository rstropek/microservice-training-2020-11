using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetCoreMicroserviceSample.Api.Configuration;
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
        private readonly IOptions<HealthConfiguration> healthConfig;

        public HealthController(IOptions<HealthConfiguration> healthConfig)
        {
            this.healthConfig = healthConfig ?? throw new ArgumentNullException(nameof(healthConfig));
        }

        /// <summary>
        /// Find out wheteher the API is running
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetHealth")]
        public string Get() => $"{DateTime.UtcNow} {this.healthConfig.Value.HealtEndpointGreetingMessage}";
    }
}