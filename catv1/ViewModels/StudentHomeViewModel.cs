using System.Collections.ObjectModel;
using catv1.Models;

namespace catv1.ViewModels;

public class CalendarDay
{
    public int Day { get; set; }
    public string Background { get; set; } = "#1F1F1F"; // Default dark
    public string TextColor { get; set; } = "White";
}

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

    private string _qrCodeValue = string.Empty;
    public string QrCodeValue
    {
        get => _qrCodeValue;
        set => SetProperty(ref _qrCodeValue, value);
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
    public ObservableCollection<CalendarDay> CalendarDays { get; }
    public ObservableCollection<ScheduleItem> TodaySchedule { get; set; }

    private string _streakStatus = "Active";
    public string StreakStatus
    {
        get => _streakStatus;
        set => SetProperty(ref _streakStatus, value);
    }

    private double _streakProgress = 0.7;
    public double StreakProgress
    {
        get => _streakProgress;
        set => SetProperty(ref _streakProgress, value);
    }

    private int _daysPresent = 2;
    public int DaysPresent
    {
        get => _daysPresent;
        set => SetProperty(ref _daysPresent, value);
    }

    private int _daysAbsent = 0;
    public int DaysAbsent
    {
        get => _daysAbsent;
        set => SetProperty(ref _daysAbsent, value);
    }

    public Command ShowIdCardCommand { get; }
    public Command ViewReportsCommand { get; }
    public Command NotificationsCommand { get; }
    public Command SettingsCommand { get; }

    private Supabase.Realtime.RealtimeChannel? _channel;

    public StudentHomeViewModel(Supabase.Client supabase)
    {
        _supabase = supabase;
        RecentActivity = [];
        CalendarDays = [];
        TodaySchedule = [];

        ShowIdCardCommand = new Command(async () => await OnShowIdCard());
        ViewReportsCommand = new Command(async () => await OnViewReports());
        NotificationsCommand = new Command(async () => await Shell.Current.DisplayAlertAsync("Notifications", "No new notifications.", "OK"));
        SettingsCommand = new Command(async () => await Shell.Current.DisplayAlertAsync("Settings", "Settings feature coming soon!", "OK"));

        PreviousMonthCommand = new Command(OnPreviousMonth);
        NextMonthCommand = new Command(OnNextMonth);

        GenerateCalendarDays();
        SubscribeToUpdates();
    }

    private DateTime _currentCalendarDate = DateTime.Now;

    public string CurrentMonthYear => _currentCalendarDate.ToString("MMMM yyyy");

    public Command PreviousMonthCommand { get; }
    public Command NextMonthCommand { get; }

    private void OnPreviousMonth()
    {
        _currentCalendarDate = _currentCalendarDate.AddMonths(-1);
        OnPropertyChanged(nameof(CurrentMonthYear));
        GenerateCalendarDays();
    }

    private void OnNextMonth()
    {
        _currentCalendarDate = _currentCalendarDate.AddMonths(1);
        OnPropertyChanged(nameof(CurrentMonthYear));
        GenerateCalendarDays();
    }

    private List<ActivityLog> _cachedLogs = new();

    private void GenerateCalendarDays()
    {
        CalendarDays.Clear();
        int daysInMonth = DateTime.DaysInMonth(_currentCalendarDate.Year, _currentCalendarDate.Month);

        for (int i = 1; i <= daysInMonth; i++)
        {
            var bg = "#1E293B";
            var logForDay = _cachedLogs.FirstOrDefault(l => l.DateTime.Day == i && l.DateTime.Month == _currentCalendarDate.Month && l.DateTime.Year == _currentCalendarDate.Year);
            if (logForDay != null)
            {
                bg = logForDay.Status switch
                {
                    "Present" => "#10B981",
                    "Absent" => "#EF4444",
                    "Late" => "#F59E0B",
                    "Excused" => "#6366F1",
                    _ => "#1E293B"
                };
            }
            CalendarDays.Add(new CalendarDay { Day = i, Background = bg });
        }
    }

    private async Task OnShowIdCard()
    {
        if (string.IsNullOrEmpty(Id))
        {
            await Shell.Current.DisplayAlertAsync("Error", "Student ID not found.", "OK");
            return;
        }
        await Shell.Current.GoToAsync($"barcode?name={Uri.EscapeDataString(Name)}&id={Uri.EscapeDataString(Id)}&qrcode={Uri.EscapeDataString(QrCodeValue)}");
    }

    private async Task OnViewReports()
    {
        await Shell.Current.GoToAsync("//student/studentHistoryTab/studentHistory");
    }

    public async Task LoadDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var user = _supabase.Auth.CurrentUser;
            if (user == null) return;

            System.Diagnostics.Debug.WriteLine($"[StudentDashboard] Fetching profile for user: {user.Id}");
            var studentResponse = (await _supabase.From<Student>()
                .Where(s => s.Id == user.Id)
                .Get()).Models.FirstOrDefault();

            if (studentResponse == null)
            {
                System.Diagnostics.Debug.WriteLine($"[StudentDashboard] ID lookup failed for student {user.Id}, trying email {user.Email}");
                studentResponse = (await _supabase.From<Student>()
                    .Filter("email", Supabase.Postgrest.Constants.Operator.ILike, user.Email)
                    .Get()).Models.FirstOrDefault();
            }

            if (studentResponse == null)
            {
                System.Diagnostics.Debug.WriteLine("[StudentDashboard] ERROR: Profile not found!");
                IsBusy = false;
                return;
            }

            Name = studentResponse.FullName;
            Id = studentResponse.StudentNumber; // Display Student Number
            QrCodeValue = studentResponse.QrCode ?? string.Empty;
            
            // Fetch Department Name
            if (!string.IsNullOrEmpty(studentResponse.DepartmentId))
            {
                var deptResponse = (await _supabase.From<Department>()
                    .Where(d => d.Id == studentResponse.DepartmentId)
                    .Get()).Models.FirstOrDefault();
                
                if (deptResponse != null)
                {
                    ClassName = deptResponse.Name;
                }
                else
                {
                    ClassName = "Unknown Department";
                }
            }

            // Fetch ALL Attendance Logs for Stats and Streak
            var allLogsResponse = await _supabase.From<ActivityLog>()
                .Where(l => l.StudentId == user.Id)
                .Order("session_date", Supabase.Postgrest.Constants.Ordering.Descending)
                .Get();

            var allLogs = allLogsResponse.Models;
            _cachedLogs = allLogs;

            // Update Heatmap
            GenerateCalendarDays();

            // Update Recent Activity
            RecentActivity.Clear();
            foreach (var log in allLogs.Take(5))
            {
                RecentActivity.Add(log);
            }

            // Calculate Stats
            if (allLogs.Any())
            {
                DaysPresent = allLogs.Count(l => l.Status == "Present" || l.Status == "Late");
                DaysAbsent = allLogs.Count(l => l.Status == "Absent");
                AbsencesCount = DaysAbsent;
                AttendancePercentage = (double)DaysPresent / allLogs.Count;

                // Calculate Streak
                int streak = 0;
                foreach (var log in allLogs)
                {
                    if (log.Status == "Present" || log.Status == "Late")
                        streak++;
                    else if (log.Status == "Absent")
                        break; // Streak broken
                }
                StreakStatus = streak > 0 ? "Active" : "None";
                StreakProgress = Math.Min(streak / 10.0, 1.0); // Simple progress visualization
            }
            else
            {
                DaysPresent = 0;
                DaysAbsent = 0;
                AbsencesCount = 0;
                AttendancePercentage = 0;
                StreakStatus = "None";
                StreakProgress = 0;
            }

            System.Diagnostics.Debug.WriteLine($"[StudentDashboard] Profile found: {studentResponse.FullName}. Fetching schedule...");
            // Fetch Enrollments and Sections for Schedule
            TodaySchedule.Clear();
            var enrollmentsResponse = await _supabase.From<Enrollment>()
                .Where(e => e.StudentId == user.Id)
                .Get();

            foreach (var enrollment in enrollmentsResponse.Models)
            {
                var sectionResponse = (await _supabase.From<Section>()
                    .Where(s => s.Id == enrollment.SectionId)
                    .Get()).Models.FirstOrDefault();

                if (sectionResponse != null)
                {
                    var courseResponse = (await _supabase.From<Course>()
                        .Where(c => c.Id == sectionResponse.CourseId)
                        .Get()).Models.FirstOrDefault();

                    if (courseResponse != null)
                    {
                        var semesterLabel = sectionResponse.Semester == 1 ? "Semester 1" : "Semester 2";
                        TodaySchedule.Add(new ScheduleItem
                        {
                            Subject = courseResponse.Name,
                            Time = $"{semesterLabel}, {sectionResponse.AcademicYear}",
                            Room = sectionResponse.Name,
                            Status = "Active"
                        });
                    }
                }
            }
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

    public void SubscribeToUpdates()
    {
        try
        {
            if (_channel != null) return; // Already subscribed

            var user = _supabase.Auth.CurrentUser;
            if (user == null) return;

            _channel = _supabase.Realtime.Channel($"student-attendance-{user.Id}");
            var options = new Supabase.Realtime.PostgresChanges.PostgresChangesOptions("public", "attendance_logs");
            
            _channel.Register(options);
            _channel.AddPostgresChangeHandler(Supabase.Realtime.PostgresChanges.PostgresChangesOptions.ListenType.Inserts, (sender, args) =>
            {
                System.Diagnostics.Debug.WriteLine($"[StudentDashboard] Realtime insert received, reloading data...");
                MainThread.BeginInvokeOnMainThread(async () => await LoadDataAsync());
            });
            _channel.Subscribe();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Realtime Error: {ex}");
        }
    }

    public void UnsubscribeFromUpdates()
    {
        try
        {
            if (_channel != null)
            {
                _channel.Unsubscribe();
                _channel = null;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Realtime Unsubscribe Error: {ex}");
        }
    }
}
