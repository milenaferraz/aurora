using System.Threading;
using System.Threading.Tasks;

namespace Aurora.Application.Interfaces;

public record TaskSummary(int Count);

public interface ITaskProvider
{
    Task<TaskSummary> GetPendingCountAsync(CancellationToken cancellationToken);
}
