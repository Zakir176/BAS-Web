using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace catv1.Models;

[Table("lecturers")]
public class LecturerProfile : BaseModel
{
    [PrimaryKey("id", false)]
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty; // Supabase Auth UUID

    [Column("registration_id")]
    [JsonProperty("registration_id")]
    public string RegistrationId { get; set; } = string.Empty;

    [Column("first_name")]
    [JsonProperty("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [Column("last_name")]
    [JsonProperty("last_name")]
    public string LastName { get; set; } = string.Empty;

    [Column("email")]
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;
}
