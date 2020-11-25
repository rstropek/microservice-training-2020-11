using Grpc.Core;
using NetCoreMicroserviceSample.MachineService;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreMicroserviceSample.Api.MachineConnection
{
    public interface IMachineService
    {
        Task<MachineResponse> UpdateSettingsAsync(MachineSettingsUpdate request);

        Task<MachineResponse> TriggerSwitchAsync(SwitchTrigger request);

        Task<AsyncServerStreamingCall<MeasurementResponse>> GetMeasurementStream(MeasurementRequest request, CancellationToken cancellationToken);
    }
}
