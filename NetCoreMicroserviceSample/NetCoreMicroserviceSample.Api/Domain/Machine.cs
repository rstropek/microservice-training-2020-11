using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreMicroserviceSample.Api.Domain
{
    public class Machine
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;

        public string SvgImage { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}