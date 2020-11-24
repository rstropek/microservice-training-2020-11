using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreMicroserviceSample.Api.Domain;
using NetCoreMicroserviceSample.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreMicroserviceSample.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachineController : ControllerBase
    {
        private readonly IMapper mapper;

        public MachineController(IMapper mapper)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var machine = new Machine
            {
                Id = Guid.NewGuid(),
                Name = "m1"
            };

            var result = this.mapper.Map<MachineMetadata>(machine);

            return Ok(result);
        }
    }
}
