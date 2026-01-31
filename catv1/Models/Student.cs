using System.ComponentModel;
using System.Runtime.CompilerServices;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace catv1.Models;

[Table("students")]
public class Student : BaseModel, INotifyPropertyChanged
{
    [PrimaryKey("id", false)]
    public string Id { get; set; } = string.Empty; // Supabase Auth UUID

    [Column("student_id")]
    public string StudentId { get; set; } = string.Empty; // The SIN (e.g. 210984)

    [Column("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("department")]
    public string Department { get; set; } = string.Empty;

    [Newtonsoft.Json.JsonIgnore]
    public string Name => $"{FirstName} {LastName}";

    [Newtonsoft.Json.JsonIgnore]
    public string Initials => !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) 
        ? $"{FirstName[0]}{LastName[0]}" 
        : "?";

    // UI Properties
    private bool _isPresent;
    [Column("is_present")]
    public bool IsPresent
    {
        get => _isPresent;
        set => SetProperty(ref _isPresent, value);
    }

    [Column("scan_time")]
    public DateTime? ScanTime { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public string TimeDisplay => IsPresent && ScanTime.HasValue ? ScanTime.Value.ToString("hh:mm tt") : "";

    // Helper for property change notification
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
