using System.ComponentModel;
using System.Runtime.CompilerServices;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace catv1.Models;

[Table("students")]
public class Student : BaseModel, INotifyPropertyChanged
{
    [PrimaryKey("id")]
    [Column("id")]
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [Column("student_number")]
    [JsonProperty("student_number")]
    public string StudentNumber { get; set; } = string.Empty;

    [Column("full_name")]
    [JsonProperty("full_name")]
    public string FullName { get; set; } = string.Empty;

    [Column("email")]
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [Column("phone")]
    [JsonProperty("phone")]
    public string Phone { get; set; } = string.Empty;

    [Column("qr_code")]
    [JsonProperty("qr_code")]
    public string QrCode { get; set; } = string.Empty;

    [Column("department_id")]
    [JsonProperty("department_id")]
    public string DepartmentId { get; set; } = string.Empty;

    [Column("is_active")]
    [JsonProperty("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    [JsonProperty("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    [JsonProperty("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Computed properties (not stored in database)
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string Initials 
    { 
        get 
        {
            if (string.IsNullOrWhiteSpace(FullName)) return "?";
            var parts = FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Join("", parts.Select(p => p[0]));
        }
    }

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string FirstName 
    { 
        get 
        {
            if (string.IsNullOrWhiteSpace(FullName)) return "";
            return FullName.Split(' ').FirstOrDefault() ?? "";
        }
    }

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string LastName 
    { 
        get 
        {
            if (string.IsNullOrWhiteSpace(FullName)) return "";
            var parts = FullName.Split(' ');
            return parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : "";
        }
    }

    // UI Properties (not stored in database)
    private bool _isPresent;
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public bool IsPresent
    {
        get => _isPresent;
        set => SetProperty(ref _isPresent, value);
    }

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public DateTime? ScanTime { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string TimeDisplay => IsPresent && ScanTime.HasValue ? ScanTime.Value.ToString("hh:mm tt") : "";

    // Helper for property change notifications
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value)) return false;
        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}