using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreMicroserviceSample.Api.Domain;
using NetCoreMicroserviceSample.Api.Dtos;
using NetCoreMicroserviceSample.Api.Repository;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

namespace NetCoreMicroserviceSample.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachinesController : ControllerBase
    {
        // generate Swagger file "offline" (e.g. for the build pipeline)
        // dotnet swagger tofile --output obj/api.json bin/Debug/net5.0/NetCoreMicroserviceSample.Api.dll v1

        // generate Autorest client code for TypeScript
        // npx autorest --input-file=../NetCoreMicroserviceSample.Api/obj/api.json --typescript --output-folder=../NetCoreMicroserviceSample.ApiClient --v3

        private readonly IMapper mapper;
        private readonly MachineVisualizationDataContext dbContext;

        public MachinesController(IMapper mapper, MachineVisualizationDataContext dbContext)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Get a list of available machines
        /// </summary>
        /// <returns>Liszt of machines</returns>
        [HttpGet(Name = "GetAllMachines")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(IEnumerable<MachineMetadata>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var machines = await this.dbContext.Machines.ToListAsync();

            var result = machines.Select(m => this.mapper.Map<MachineMetadata>(m));

            return Ok(result);
        }


        /// <summary>
        /// Get a machine by id
        /// </summary>
        /// <param name="id">ID of the machine to get</param>
        /// <returns>Machine</returns>
        [HttpGet("{id}", Name = "MachineById")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(MachineMetadata), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var machine = await dbContext.Machines
                .SingleOrDefaultAsync(m => m.Id == id);

            return machine switch
            {
                null => NotFound(),
                _ => Ok(this.mapper.Map<MachineMetadata>(machine))
            };
        }

        /// <summary>
        /// Get SVG image for machine with given ID
        /// </summary>
        /// <param name="id">ID of the machine</param>
        /// <returns>Machine SVG image</returns>
        [HttpGet("{id}/image", Name = "GetMachineImage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetImageAsync(Guid id)
        {
            var machine = await dbContext.Machines.Select(m => new { m.Id, m.SvgImage }).SingleOrDefaultAsync(m => m.Id == id);
            if (machine == null || string.IsNullOrEmpty(machine.SvgImage))
            {
                return NotFound();
            }

            return new ContentResult { ContentType = "image/svg+xml", StatusCode = (int)HttpStatusCode.OK, Content = machine.SvgImage };
        }

        /// <summary>
        /// Create a machine
        /// </summary>
        /// <param name="machine">Machine to create</param>
        /// <returns>Created machine</returns>
        [HttpPost(Name = "AddMachine")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Machine), StatusCodes.Status201Created)]
        [SwaggerResponseHeader(StatusCodes.Status201Created, "location", "string", "Location of the created machine")]
        public async Task<IActionResult> PostAsync([FromBody] Machine machine)
        {
            dbContext.Machines.Add(machine);
            await dbContext.SaveChangesAsync();
            return CreatedAtRoute("MachineById", new { machine.Id }, machine);
        }

        /// <summary>
        /// Update machine with given id
        /// </summary>
        /// <param name="id">ID of the machine to update</param>
        /// <param name="machine">Machine data</param>
        /// <remarks>
        /// ID in URL and body must match. If they don't, bad request is returned.
        /// </remarks>
        [HttpPut("{id}", Name = "UpdateMachine")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Machine), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutAsync(Guid id, [FromBody] Machine machine)
        {
            if (id != machine.Id)
            {
                return BadRequest(new ProblemDetails
                {
                    Instance = "https://api.foobar.com/errors/idDoesNotMatch",
                    Title = "IDs do not match",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "ID in URL and body do not match."
                });
            }

            var machineToUpdate = await dbContext.Machines.SingleOrDefaultAsync(m => m.Id == machine.Id);
            if (machineToUpdate == null)
            {
                return NotFound();
            }

            machineToUpdate.Name = machine.Name;
            machineToUpdate.Description = machine.Description;

            await dbContext.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Delete a machine with given ID
        /// </summary>
        /// <param name="id">ID of the machine to delete</param>
        [HttpDelete("{id}", Name = "DeleteMachine")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var machine = await dbContext.Machines.SingleOrDefaultAsync(m => m.Id == id);
            if (machine == null)
            {
                return NotFound();
            }

            dbContext.Machines.Remove(machine);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
