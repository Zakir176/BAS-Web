namespace catv1.Models;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("activity_logs")]
public class ActivityLog : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("status")]
    public string? Status { get; set; } // "Present", "Absent"

    [Column("date_time")]
    public DateTime DateTime { get; set; }

    [Column("is_verified")]
    public bool IsVerified { get; set; }

    [Column("is_excused")]
    public bool IsExcused { get; set; }

    // Derived properties for display
    [Newtonsoft.Json.JsonIgnore]
    public string DateDisplay => DateTime.ToString("MMM dd, yyyy");
    [Newtonsoft.Json.JsonIgnore]
    public string TimeDisplay => Status == "Absent" && IsExcused ? "All Day" : DateTime.ToString("hh:mm tt");
    [Newtonsoft.Json.JsonIgnore]
    public string FullTimeDisplay => $"{DateDisplay} • {TimeDisplay}";
}
