using Grpc.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NetCoreMicroserviceSample.Api.MachineConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreMicroserviceSample.Api.Hubs
{
    public class MachineDataHub : Hub
    {
        private readonly ILogger<MachineDataHub> logger;
        private readonly IMachineService machineService;

        public MachineDataHub(ILogger<MachineDataHub> logger, IMachineService machineService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.machineService = machineService ?? throw new ArgumentNullException(nameof(machineService));
        }

        public string Hello(string greeterString)
        {
            this.logger.LogInformation($"Hello was called :) String was: {greeterString}. Client was: {this.Clients.Caller}");

            Console.WriteLine(this);

            return greeterString;
        }

        public async IAsyncEnumerable<double> MachineData(Guid machineId, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            logger.LogInformation($"start streaming for machine {machineId}");
            using var measurementStream = await machineService.GetMeasurementStream(
                new() { MachineId = machineId.ToString() },
                cancellationToken: cancellationToken);
            try
            {
                await foreach (var m in measurementStream.ResponseStream.ReadAllAsync(cancellationToken))
                {
                    yield return m.Value;
                }
            }
            finally
            {
                logger.LogInformation($"stopped streaming for machine {machineId}");
            }
        }
    }
}
