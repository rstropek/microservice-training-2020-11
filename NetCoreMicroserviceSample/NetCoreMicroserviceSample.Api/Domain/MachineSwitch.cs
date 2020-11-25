using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreMicroserviceSample.Api.Domain
{
    public class MachineSwitch
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid MachineId { get; set; }

        public Machine Machine { get; set; } = null!;

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int PositionX { get; set; }

        public int PositionY { get; set; }
    }
}
