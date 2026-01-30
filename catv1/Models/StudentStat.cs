namespace catv1.Models;

public class StudentStat
{
    public string? Name { get; set; }
    public string? Id { get; set; }
    public string? Avatar { get; set; } // URL or resource name
    public double PresenceRate { get; set; }
    public int Absences { get; set; }
    public string PresenceRateDisplay => $"{PresenceRate:P0} Rate";
    public string AbsencesDisplay => $"{Absences} Absences";
}
