using System;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Application.Interfaces;
using Aurora.Contracts.Dashboard;

namespace Aurora.Application.Dashboard;

public class DashboardService
{
    private readonly IHermesClient _hermes;
    private readonly ICalendarProvider _calendar;
    private readonly ITaskProvider _tasks;
    private readonly IEmailProvider _emails;

    public DashboardService(
        IHermesClient hermes,
        ICalendarProvider calendar,
        ITaskProvider tasks,
        IEmailProvider emails)
    {
        _hermes = hermes;
        _calendar = calendar;
        _tasks = tasks;
        _emails = emails;
    }

    public async Task<DashboardResponse> GetDashboardAsync(CancellationToken cancellationToken)
    {
        var hermesHealthTask = _hermes.IsHealthyAsync(cancellationToken);
        var calendarTask = _calendar.GetTodaySummaryAsync(cancellationToken);
        var tasksTask = _tasks.GetPendingCountAsync(cancellationToken);
        var emailsTask = _emails.GetImportantCountAsync(cancellationToken);

        await Task.WhenAll(hermesHealthTask, calendarTask, tasksTask, emailsTask);

        var hermesOnline = await hermesHealthTask;
        var calendar = await calendarTask;
        var taskSummary = await tasksTask;
        var emailSummary = await emailsTask;

        var hour = DateTime.Now.Hour;
        var greeting = hour < 12 ? "Bom dia" : hour < 18 ? "Boa tarde" : "Boa noite";

        return new DashboardResponse
        {
            Greeting = greeting,
            Aurora = new AuroraStatus { Status = "online" },
            Agenda = new AgendaSummary
            {
                EventsToday = calendar.Count,
                NextEvent = calendar.Next is { } next
                    ? new NextEvent { Title = next.Title, Time = next.Time }
                    : null
            },
            Tasks = new TasksSummary { Pending = taskSummary.Count },
            Emails = new EmailsSummary { Important = emailSummary.Count },
            System = new SystemStatus
            {
                Api = "online",
                Hermes = hermesOnline ? "online" : "offline",
                Memory = "unknown"
            }
        };
    }
}
