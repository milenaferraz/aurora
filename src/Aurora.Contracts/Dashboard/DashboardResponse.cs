namespace Aurora.Contracts.Dashboard;

public class DashboardResponse
{
    public string Greeting { get; set; } = string.Empty;
    public AuroraStatus Aurora { get; set; } = new();
    public AgendaSummary Agenda { get; set; } = new();
    public TasksSummary Tasks { get; set; } = new();
    public EmailsSummary Emails { get; set; } = new();
    public SystemStatus System { get; set; } = new();
}

public class AuroraStatus
{
    public string Status { get; set; } = "online";
}

public class AgendaSummary
{
    public int EventsToday { get; set; }
    public NextEvent? NextEvent { get; set; }
}

public class NextEvent
{
    public string Title { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
}

public class TasksSummary
{
    public int Pending { get; set; }
}

public class EmailsSummary
{
    public int Important { get; set; }
}

public class SystemStatus
{
    public string Api { get; set; } = "online";
    public string Hermes { get; set; } = "unknown";
    public string Memory { get; set; } = "unknown";
}
