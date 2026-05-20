using Core.Common.Enums;
using Mission_Service.Models;
using Mission_Service.Models.Ro;

namespace Mission_Service.Services.UAVStatusService.Interfaces
{
    public interface IUAVStatusService
    {
        bool IsInActiveMission(Dictionary<TelemetryFields, double> telemetryData);
        Mission? GetActiveMission(int tailId);
        void SetActiveMission(int tailId, Mission mission);
        void ClearActiveMission(int tailId);
        IEnumerable<ActiveMissionRo> GetAllActiveMissions();
    }
}
