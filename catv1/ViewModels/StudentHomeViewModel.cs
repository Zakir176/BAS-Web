using System.Collections.ObjectModel;
using catv1.Models;

namespace catv1.ViewModels;

public class StudentHomeViewModel : BaseViewModel
{
    public string Name { get; set; } = "John Doe";
    public string Id { get; set; } = "STU001";
    public string ClassName { get; set; } = "Class 10A";
    public string Avatar { get; set; } = "dotnet_bot.png";

    public string IdLine => $"ID: {Id} | {ClassName}";

    public double AttendancePercentage { get; set; } = 0.94;
    public int AbsencesCount { get; set; } = 3;

    public string AttendanceDisplay => $"{AttendancePercentage:P0}";

    public ObservableCollection<ActivityLog> RecentActivity { get; set; }

    // For the heatmap, we'll just simulate days for now. 
    // In a real app, this would be a collection of Day objects with status.
    public ObservableCollection<int> CalendarDays { get; }

    public StudentHomeViewModel()
    {
        // Dummy Activity Data
        RecentActivity = new ObservableCollection<ActivityLog>
        {
            new ActivityLog { Status = "Present", DateTime = new DateTime(2023, 9, 15, 8, 30, 0), IsVerified = true },
            new ActivityLog { Status = "Absent", DateTime = new DateTime(2023, 9, 14, 8, 0, 0), IsVerified = false, IsExcused = false }, // Unexcused
            new ActivityLog { Status = "Absent", DateTime = new DateTime(2023, 9, 13), IsVerified = false, IsExcused = true }, // Excused
            new ActivityLog { Status = "Present", DateTime = new DateTime(2023, 9, 12, 8, 28, 0), IsVerified = true }
        };

        // Dummy Calendar Data (Numbers 1-30)
        CalendarDays = new ObservableCollection<int>();
        for (int i = 1; i <= 30; i++)
        {
            CalendarDays.Add(i);
        }
    }
}
