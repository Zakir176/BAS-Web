using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace catv1.Models;

[Table("departments")]
public class Department : BaseModel
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

    [Column("created_at")]
    [JsonProperty("created_at")]
    public DateTime? CreatedAt { get; set; }
}
