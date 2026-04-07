namespace catv1.Models;

public class ScheduleItem
{
    public string Subject { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public string Status { get; set; } = "Scheduled"; // Scheduled, Ongoing, Finished
}
