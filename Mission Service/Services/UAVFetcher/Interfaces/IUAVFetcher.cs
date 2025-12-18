using Mission_Service.Models;

namespace Mission_Service.Services.UAVFetcher.Interfaces
{
    public interface IUAVFetcher
    {
        Task<IReadOnlyCollection<UAV>> FetchUAVsAsync(CancellationToken cancellationToken);
    }
}
