using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreMicroserviceSample.Api.Dtos
{
    public record MachineMetadata(Guid id, string Name, string Description);
}
