using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace catv1.Models;

[Table("enrollments")]
public class Enrollment : BaseModel
{
    [PrimaryKey("id")]
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [Column("student_id")]
    [JsonProperty("student_id")]
    public string StudentId { get; set; } = string.Empty;

    [Column("section_id")]
    [JsonProperty("section_id")]
    public string SectionId { get; set; } = string.Empty;

    [Column("enrolled_at")]
    [JsonProperty("enrolled_at")]
    public DateTime EnrolledAt { get; set; }
}
