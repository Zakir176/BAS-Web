namespace catv1.Models;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("attendance_logs")]
public class ActivityLog : BaseModel
{
    [PrimaryKey("id", false)]
    public string Id { get; set; } = string.Empty;

    [Column("student_id")]
    public string StudentId { get; set; } = string.Empty;

    [Column("section_id")]
    public string SectionId { get; set; } = string.Empty;

    [Column("status")]
    public string? Status { get; set; } // "Present", "Absent", "Late", "Excused"

    [Column("session_date")]
    public DateTime DateTime { get; set; }

    [Column("is_verified")]
    public bool IsVerified { get; set; }

    [Column("is_excused")]
    public bool IsExcused { get; set; }

    [Column("verified_by")]
    public string? VerifiedBy { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string DateDisplay => DateTime.ToString("MMM dd, yyyy");
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string TimeDisplay => Status == "Absent" && IsExcused ? "All Day" : DateTime.ToString("hh:mm tt");
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string FullTimeDisplay => $"{DateDisplay} • {TimeDisplay}";
}
