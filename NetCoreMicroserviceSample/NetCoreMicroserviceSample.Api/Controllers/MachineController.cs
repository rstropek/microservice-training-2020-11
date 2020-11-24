using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreMicroserviceSample.Api.Domain;
using NetCoreMicroserviceSample.Api.Dtos;
using NetCoreMicroserviceSample.Api.Repository;
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
        private readonly MachineVisualizationDataContext dbContext;

        public MachineController(IMapper mapper, MachineVisualizationDataContext dbContext)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var machines = await this.dbContext.Machines.ToListAsync();

            var result = machines.Select(m => this.mapper.Map<MachineMetadata>(m));

            return Ok(result);
        }
    }
}
