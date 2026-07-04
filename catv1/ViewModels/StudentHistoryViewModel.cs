using System.Collections.ObjectModel;
using catv1.Models;
using catv1.Services;

namespace catv1.ViewModels;

public class StudentHistoryViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly IAttendanceService _attendanceService;

    public ObservableCollection<ActivityLog> AttendanceLogs { get; } = new();

    private string _totalPresent = "0";
    public string TotalPresent
    {
        get => _totalPresent;
        set => SetProperty(ref _totalPresent, value);
    }

    private string _totalAbsent = "0";
    public string TotalAbsent
    {
        get => _totalAbsent;
        set => SetProperty(ref _totalAbsent, value);
    }

    public StudentHistoryViewModel(IAuthService authService, IAttendanceService attendanceService)
    {
        _authService = authService;
        _attendanceService = attendanceService;
        
        Title = "My Attendance History";
    }

    public async Task LoadHistoryAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var user = _authService.CurrentUser;
            if (user == null) return;

            var logs = await _attendanceService.GetStudentLogsAsync(user.Id ?? string.Empty);
            
            AttendanceLogs.Clear();
            int presentCount = 0;
            int absentCount = 0;

            foreach (var log in logs.OrderByDescending(l => l.DateTime))
            {
                AttendanceLogs.Add(log);
                if (log.Status == "Present" || log.Status == "Late")
                {
                    presentCount++;
                }
                else if (log.Status == "Absent")
                {
                    absentCount++;
                }
            }

            TotalPresent = presentCount.ToString();
            TotalAbsent = absentCount.ToString();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading history: {ex}");
            await Shell.Current.DisplayAlertAsync("Error", "Failed to load attendance history.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
