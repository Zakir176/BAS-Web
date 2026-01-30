using System.Collections.ObjectModel;
using catv1.Models;

namespace catv1.ViewModels;

public class StudentHomeViewModel : BaseViewModel
{
    private readonly Supabase.Client _supabase;

    private string _name = "Loading...";
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string _id = "---";
    public string Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private string _className = "---";
    public string ClassName
    {
        get => _className;
        set => SetProperty(ref _className, value);
    }

    public string Avatar { get; set; } = "dotnet_bot.png";

    public string IdLine => $"ID: {Id} | {ClassName}";

    private double _attendancePercentage;
    public double AttendancePercentage
    {
        get => _attendancePercentage;
        set
        {
            if (SetProperty(ref _attendancePercentage, value))
                OnPropertyChanged(nameof(AttendanceDisplay));
        }
    }

    private int _absencesCount;
    public int AbsencesCount
    {
        get => _absencesCount;
        set => SetProperty(ref _absencesCount, value);
    }

    public string AttendanceDisplay => $"{AttendancePercentage:P0}";

    public ObservableCollection<ActivityLog> RecentActivity { get; set; }

    public ObservableCollection<int> CalendarDays { get; }

    public StudentHomeViewModel(Supabase.Client supabase)
    {
        _supabase = supabase;
        RecentActivity = new ObservableCollection<ActivityLog>();
        CalendarDays = new ObservableCollection<int>();

        // Dummy Calendar Data
        for (int i = 1; i <= 30; i++)
        {
            CalendarDays.Add(i);
        }
    }

    public async Task LoadDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var user = _supabase.Auth.CurrentUser;
            if (user == null) return;

            // Fetch Student Profile
            // Assumption: students table has a column 'id' that matches user.Id or some lookup
            // For now, fetching by email or assuming the Id in students matches user identity
            var studentResponse = await _supabase.From<Student>()
                .Where(s => s.Id == user.Id) // Adjust if mapping is different
                .Single();

            if (studentResponse != null)
            {
                Name = studentResponse.Name;
                Id = studentResponse.Id;
                // ClassName might need another field or join
            }

            // Fetch Recent Activity
            var logsResponse = await _supabase.From<ActivityLog>()
                .Order("date_time", Supabase.Postgrest.Constants.Ordering.Descending)
                .Limit(5)
                .Get();

            RecentActivity.Clear();
            foreach (var log in logsResponse.Models)
            {
                RecentActivity.Add(log);
            }

            // Calculate attendance dummy for now or fetch stats
            AttendancePercentage = 0.95;
            AbsencesCount = 2;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadData Error: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
