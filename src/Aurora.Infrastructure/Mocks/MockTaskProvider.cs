using System.Threading;
using System.Threading.Tasks;
using Aurora.Application.Interfaces;

namespace Aurora.Infrastructure.Mocks;

public class MockTaskProvider : ITaskProvider
{
    public Task<TaskSummary> GetPendingCountAsync(CancellationToken cancellationToken)
        => Task.FromResult(new TaskSummary(0));
}
