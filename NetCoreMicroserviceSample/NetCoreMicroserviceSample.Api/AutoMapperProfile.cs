using AutoMapper;
using NetCoreMicroserviceSample.Api.Domain;
using NetCoreMicroserviceSample.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreMicroserviceSample.Api
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            this.CreateMap<Machine, MachineMetadata>();
        }
    }
}
