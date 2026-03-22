using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace catv1.Models;

[Table("sections")]
public class Section : BaseModel
{
    [PrimaryKey("id")]
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [Column("course_id")]
    [JsonProperty("course_id")]
    public string CourseId { get; set; } = string.Empty;

    [Column("name")]
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [Column("semester")]
    [JsonProperty("semester")]
    public int Semester { get; set; }

    [Column("academic_year")]
    [JsonProperty("academic_year")]
    public int AcademicYear { get; set; }

    [Column("lecturer_id")]
    [JsonProperty("lecturer_id")]
    public string LecturerId { get; set; } = string.Empty;
}
