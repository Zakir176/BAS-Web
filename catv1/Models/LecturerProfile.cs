using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace catv1.Models;

[Table("lecturers")]
public class LecturerProfile : BaseModel
{
    [PrimaryKey("id", false)]
    [JsonProperty("id")]
    public string Id { get; set; }

    [Column("name")]
    [JsonProperty("name")]
    public string Name { get; set; }
}
