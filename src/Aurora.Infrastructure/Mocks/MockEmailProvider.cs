using System.Threading;
using System.Threading.Tasks;
using Aurora.Application.Interfaces;

namespace Aurora.Infrastructure.Mocks;

public class MockEmailProvider : IEmailProvider
{
    public Task<EmailSummary> GetImportantCountAsync(CancellationToken cancellationToken)
        => Task.FromResult(new EmailSummary(0));
}
