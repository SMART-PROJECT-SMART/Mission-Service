using Core.Common.Enums;
using Mission_Service.Models;

namespace Mission_Service.Services.UAVStatusService.Interfaces
{
    public interface IUAVStatusService
    {
        bool IsInActiveMission(Dictionary<TelemetryFields, double> telemetryData);
        Mission? GetActiveMission(int tailId);
        void SetActiveMission(int tailId, Mission mission);
        void ClearActiveMission(int tailId);
    }
}
