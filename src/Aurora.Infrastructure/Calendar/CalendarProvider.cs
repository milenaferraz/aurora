using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Aurora.Application.Interfaces;

namespace Aurora.Infrastructure.Calendar
{
    public class CalendarProvider : ICalendarProvider
    {
        private readonly string _dataFilePath;

        public CalendarProvider(IConfiguration configuration)
        {
            var filePath = configuration["CalendarDataFilePath"];
            if (string.IsNullOrWhiteSpace(filePath))
            {
                filePath = Path.Combine(AppContext.BaseDirectory, "Data", "calendar.json");
            }
            _dataFilePath = filePath;
        }

        public Task<CalendarSummary> GetTodaySummaryAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!File.Exists(_dataFilePath))
                {
                    return Task.FromResult(new CalendarSummary(0, null));
                }

                var json = File.ReadAllText(_dataFilePath);
                var data = JsonSerializer.Deserialize<CalendarData>(json);
                int count = data?.EventCount ?? 0;
                var nextEvent = data?.NextEventInfo;
                return Task.FromResult(new CalendarSummary(count, nextEvent));
            }
            catch
            {
                return Task.FromResult(new CalendarSummary(0, null));
            }
        }
    }

    public class CalendarData
    {
        public int EventCount { get; set; }
        public NextEventInfo? NextEventInfo { get; set; }
    }

    public class NextEventInfo
    {
        public string Title { get; set; } = default!;
        public DateTime StartTime { get; set; }
        public string Location { get; set; } = default!;
    }
}
