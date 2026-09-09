using System.Threading;
using System.Threading.Tasks;

namespace Aurora.Application.Interfaces;

public record CalendarSummary(int Count, NextEventInfo? Next);
public record NextEventInfo(string Title, string Time);

public interface ICalendarProvider
{
    Task<CalendarSummary> GetTodaySummaryAsync(CancellationToken cancellationToken);
}
