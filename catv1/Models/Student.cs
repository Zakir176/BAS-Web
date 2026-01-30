using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Graphics;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace catv1.Models;

[Table("students")]
public class Student : BaseModel, INotifyPropertyChanged
{
    [PrimaryKey("id", false)]
    public string Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    public string Initials => !string.IsNullOrEmpty(Name) ? string.Join("", Name.Split(' ').Select(n => n[0])) : "?";

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

    public string TimeDisplay => IsPresent && ScanTime.HasValue ? ScanTime.Value.ToString("hh:mm tt") : "";

    // Helper for property change notification to avoid MVVM bloat for just a model
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
