namespace catv1.Models;

public class StudentStat
{
    public string? Name { get; set; }
    public string? Id { get; set; }
    public string? Avatar { get; set; } // URL
    public string? Initials { get; set; }
    public string Color { get; set; } = "#3B82F6"; // Default Blue
    public double PresenceRate { get; set; }
    public int Absences { get; set; }
    public string PresenceRateDisplay => $"{PresenceRate:P0} Rate";
    public string AbsencesDisplay => $"{Absences} Absences";
}
