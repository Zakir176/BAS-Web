namespace catv1.Models;

public class ActivityLog
{
    public string? Status { get; set; } // "Present", "Absent"
    public DateTime DateTime { get; set; }
    public bool IsVerified { get; set; }
    public bool IsExcused { get; set; }

    // Derived properties for display
    public string DateDisplay => DateTime.ToString("MMM dd, yyyy");
    public string TimeDisplay => Status == "Absent" && IsExcused ? "All Day" : DateTime.ToString("hh:mm tt");
    public string FullTimeDisplay => $"{DateDisplay} • {TimeDisplay}";
}
