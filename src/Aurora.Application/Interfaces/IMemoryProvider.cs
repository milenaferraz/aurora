using System.Threading;
using System.Threading.Tasks;

namespace Aurora.Application.Interfaces
{
    public interface IMemoryProvider
    {
        Task<string> GetAsync(string key, CancellationToken cancellationToken = default);
        Task SetAsync(string key, string value, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    }
}
