using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using NetCoreMicroserviceSample.MachineService;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using static NetCoreMicroserviceSample.MachineService.MachineAccess;

namespace NetCoreMicroserviceSample.Machine
{
    [Authorize]
    public class MachineService : MachineAccessBase
    {
        private readonly ILogger<MachineService> logger;
        private static readonly ConcurrentDictionary<Guid, double> machineSpeed = new();
        private static readonly ConcurrentDictionary<Guid, bool> hookAnimated = new();

        public MachineService(ILogger<MachineService> logger)
        {
            this.logger = logger;
        }

        public override Task<MachineResponse> UpdateSettings(MachineSettingsUpdate request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            logger.LogInformation($"Got request from client {user.Claims.Single(c => c.Type == "client_id")}");
            logger.LogInformation($"Update setting {request.SettingId} for Machine {request.MachineId}: {request.Value}");

            // To make demo more interesting, one setting controls speed of change
            // of machine sensor value.
            if (request.SettingId == "c0927560-36b9-403b-b9cf-d5a96d7cc075")
            {
                machineSpeed[new Guid(request.MachineId)] = request.Value;
            }

            return Task.FromResult(new MachineResponse() { ResultCode = 1 });
        }

        public override Task<MachineResponse> TriggerSwitch(SwitchTrigger request, ServerCallContext context)
        {
            logger.LogInformation($"Triggered switch {request.SwitchId} for Machine {request.MachineId}");

            // To make demo more interesting, one switch controls whether the sensor value changes
            if (request.SwitchId == "87a217b2-90e7-48a7-a477-7b2d2bf40f49")
            {
                var machineId = new Guid(request.MachineId);
                if (hookAnimated.TryGetValue(machineId, out var moving))
                {
                    hookAnimated[machineId] = !moving;
                }
                else
                {
                    // By default, hook is animated. If we get switch message for the
                    // first time, switch to stopped.
                    hookAnimated[machineId] = false;
                }
            }

            return Task.FromResult(new MachineResponse() { ResultCode = 1 });
        }

        public override async Task GetMeasurementStream(MeasurementRequest request, IServerStreamWriter<MeasurementResponse> responseStream, ServerCallContext context)
        {
            try
            {
                var current = 0d;
                var direction = +1d;
                while (true)
                {
                    await responseStream.WriteAsync(new MeasurementResponse() { MachineId = request.MachineId, Value = current - 300d });

                    var machineId = new Guid(request.MachineId);
                    if (!hookAnimated.TryGetValue(machineId, out var animated) || animated)
                    {
                        if (!machineSpeed.TryGetValue(machineId, out double speed))
                        {
                            // Default speed
                            speed = 1d;
                        }

                        current += speed * direction;
                        if (current > 300d)
                        {
                            direction = -1d;
                        }
                        else if (current < 0d)
                        {
                            direction = +1d;
                        }
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(25));
                }
            }
            catch (InvalidOperationException ex)
            {
                logger.LogInformation(ex, "Measurement stream cancelled");
            }
        }
    }
}
