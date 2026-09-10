using System.Threading;
using System.Threading.Tasks;
using Aurora.Application.Interfaces;

namespace Aurora.Infrastructure.Mocks;

public class MockCalendarProvider : ICalendarProvider
{
    public Task<CalendarSummary> GetTodaySummaryAsync(CancellationToken cancellationToken)
        => Task.FromResult(new CalendarSummary(0, null));
}
