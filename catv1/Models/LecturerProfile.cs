using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace catv1.Models;

[Table("lecturers")]
public class LecturerProfile : BaseModel
{
    [PrimaryKey("id")]
    [Column("id")]
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty; //Supabase Auth UUID

    [Column("registration_id")]
    [JsonProperty("registration_id")]
    public string? RegistrationId { get; set; }

    [Column("full_name")]
    [JsonProperty("full_name")]
    public string FullName { get; set; } = string.Empty;

    [Column("email")]
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [Column("department_id")]
    [JsonProperty("department_id")]
    public string DepartmentId { get; set; } = string.Empty;

    [Column("is_admin")]
    [JsonProperty("is_admin")]
    public bool IsAdmin { get; set; }

    // Display-only properties
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string DepartmentName { get; set; } = "---";
}
