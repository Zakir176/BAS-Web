using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace catv1.Models;

[Table("lecturers")]
public class LecturerProfile : BaseModel
{
    [PrimaryKey("id", false)]
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty; // This will be the Supabase Auth UUID

    [Column("lecturer_number")]
    [JsonProperty("lecturer_number")]
    public string LecturerNumber { get; set; } = string.Empty; // This will be the registration number

    [Column("name")]
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}
