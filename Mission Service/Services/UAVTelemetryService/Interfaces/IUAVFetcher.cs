using Mission_Service.Models;

namespace Mission_Service.Services.UAVTelemetryService.Interfaces
{
    public interface IUAVFetcher
    {
        Task<IReadOnlyCollection<UAV>> FetchUAVsAsync(CancellationToken cancellationToken);
    }
}
