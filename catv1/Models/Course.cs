using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace catv1.Models;

[Table("courses")]
public class Course : BaseModel
{
    [PrimaryKey("id")]
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [Column("name")]
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [Column("code")]
    [JsonProperty("code")]
    public string Code { get; set; } = string.Empty;

    [Column("department_id")]
    [JsonProperty("department_id")]
    public string DepartmentId { get; set; } = string.Empty;

    [Column("lecturer_id")]
    [JsonProperty("lecturer_id")]
    public string LecturerId { get; set; } = string.Empty;
}
