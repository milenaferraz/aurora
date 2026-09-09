using System.Threading;
using System.Threading.Tasks;

namespace Aurora.Application.Interfaces;

public record EmailSummary(int Count);

public interface IEmailProvider
{
    Task<EmailSummary> GetImportantCountAsync(CancellationToken cancellationToken);
}
