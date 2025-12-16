using Mission_Service.Common.Enums;
using Mission_Service.Models;
using Core.Common.Enums;

namespace Mission_Service.Services.UAVStatusService.Interfaces
{
    public interface IUAVStatusService
    {
        UAVType DetermineUAVType(Dictionary<TelemetryFields, double> telemetryData);
        bool IsInActiveMission(Dictionary<TelemetryFields, double> telemetryData);
        Mission? GetActiveMission(int tailId);
        void SetActiveMission(int tailId, Mission mission);
        void ClearActiveMission(int tailId);
    }
}
