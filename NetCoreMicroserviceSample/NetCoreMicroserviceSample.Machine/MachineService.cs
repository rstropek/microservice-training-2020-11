using Grpc.Core;
using Microsoft.Extensions.Logging;
using NetCoreMicroserviceSample.MachineService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static NetCoreMicroserviceSample.MachineService.MachineAccess;

namespace NetCoreMicroserviceSample.Machine
{
    public class MachineService : MachineAccessBase
    {
        private readonly ILogger<MachineService> logger;

        public MachineService(ILogger<MachineService> logger)
        {
            this.logger = logger;
        }

        public override Task<MachineResponse> UpdateSettings(MachineSettingsUpdate request, ServerCallContext context)
        {
            logger.LogInformation($"Update setting {request.SettingId} for Machine {request.MachineId}: {request.Value}");
            return Task.FromResult(new MachineResponse { ResultCode = 1 });
        }

        public override Task<MachineResponse> TriggerSwitch(SwitchTrigger request, ServerCallContext context)
        {
            logger.LogInformation($"Triggered switch {request.SwitchId} for Machine {request.MachineId}");
            return Task.FromResult(new MachineResponse { ResultCode = 1 });
        }
    }
}
