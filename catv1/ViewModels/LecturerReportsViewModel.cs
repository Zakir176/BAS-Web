using System.Collections.ObjectModel;
using catv1.Models;
using catv1.Services;
using static Supabase.Postgrest.Constants;

namespace catv1.ViewModels;

public class LecturerReportsViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly IProfileService _profileService;
    private readonly ICourseService _courseService;
    private readonly IAttendanceService _attendanceService;
    private string? _selectedGrade;
    private string? _selectedMonth;
    private string? _selectedYear;

    public ObservableCollection<StudentStat> Students { get; set; } = new();
    public ObservableCollection<string> Grades { get; set; } = new();
    public ObservableCollection<string> Months { get; } = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    public ObservableCollection<string> Years { get; } = ["2024", "2025", "2026"];

    private double _presencePercentage;
    public double PresencePercentage
    {
        get => _presencePercentage;
        set 
        {
            if (SetProperty(ref _presencePercentage, value))
                OnPropertyChanged(nameof(PresencePercentageDisplay));
        }
    }

    private double _absencePercentage;
    public double AbsencePercentage
    {
        get => _absencePercentage;
        set
        {
            if (SetProperty(ref _absencePercentage, value))
                OnPropertyChanged(nameof(AbsencePercentageDisplay));
        }
    }

    public string PresencePercentageDisplay => $"{PresencePercentage:P0}";
    public string AbsencePercentageDisplay => $"{AbsencePercentage:P0}";

    public System.Windows.Input.ICommand ExportCommand { get; }

    public LecturerReportsViewModel(IAuthService authService, IProfileService profileService, ICourseService courseService, IAttendanceService attendanceService)
    {
        _authService = authService;
        _profileService = profileService;
        _courseService = courseService;
        _attendanceService = attendanceService;
        SelectedMonth = Months[DateTime.Now.Month - 1];
        SelectedYear = DateTime.Now.Year.ToString();
        
        ExportCommand = new Command(async () => await OnExportClicked());

        _ = LoadInitialDataAsync();
    }

    public string? SelectedGrade
    {
        get => _selectedGrade;
        set { if (SetProperty(ref _selectedGrade, value)) _ = LoadStatsAsync(); }
    }

    public string? SelectedMonth
    {
        get => _selectedMonth;
        set { if (SetProperty(ref _selectedMonth, value)) _ = LoadStatsAsync(); }
    }

    public string? SelectedYear
    {
        get => _selectedYear;
        set { if (SetProperty(ref _selectedYear, value)) _ = LoadStatsAsync(); }
    }

    private async Task LoadInitialDataAsync()
    {
        try 
        {
            var user = _authService.CurrentUser;
            if (user == null) return;

            var sections = await _courseService.GetSectionsByLecturerAsync(user.Id ?? string.Empty);
            Grades.Clear();
            foreach (var s in sections) Grades.Add(s.Name);
            
            if (Grades.Any()) SelectedGrade = Grades[0];
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"History Load Error: {ex}");
        }
    }

    private async Task LoadStatsAsync()
    {
        if (string.IsNullOrEmpty(SelectedGrade) || IsBusy) return;

        try
        {
            IsBusy = true;
            var user = _authService.CurrentUser;
            if (user == null) return;

            // 1. Get Section ID
            var sections = await _courseService.GetSectionsByLecturerAsync(user.Id ?? string.Empty);
            var section = sections.FirstOrDefault(s => s.Name == SelectedGrade);

            if (section == null) return;

            // 2. Get Enrollments
            var enrollments = await _courseService.GetEnrollmentsBySectionsAsync([section.Id]);
            
            var studentIds = enrollments.Select(e => e.StudentId).ToList();
            if (!studentIds.Any())
            {
                Students.Clear();
                PresencePercentage = 0;
                AbsencePercentage = 0;
                return;
            }

            // 3. Get Students and Logs
            var allStudents = await _profileService.GetStudentsByIdsAsync(studentIds);
            var allLogs = await _attendanceService.GetSectionLogsAsync([section.Id]);

            Students.Clear();
            int totalPresent = 0;
            int totalExpected = allStudents.Count * (allLogs.Select(l => l.DateTime.Date).Distinct().Count() is var count && count == 0 ? 1 : count);

            foreach (var student in allStudents)
            {
                var studentLogs = allLogs.Where(l => l.StudentId == student.Id).ToList();
                int presentCount = studentLogs.Count(l => l.Status == "Present" || l.Status == "Late");
                int totalSessions = allLogs.Select(l => l.DateTime.Date).Distinct().Count() is var sessionCount && sessionCount == 0 ? 1 : sessionCount;

                var parts = student.FullName?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
                string initials = "?";
                if (parts.Length >= 2 && parts[0].Length > 0 && parts[^1].Length > 0)
                    initials = $"{parts[0][0]}{parts[^1][0]}".ToUpper();
                else if (student.FullName?.Length >= 2)
                    initials = student.FullName.Substring(0, 2).ToUpper();

                var colors = new[] { "#3B82F6", "#10B981", "#F59E0B", "#8B5CF6", "#EC4899" };
                string assignedColor = colors[Math.Abs(student.Id.GetHashCode()) % colors.Length];

                Students.Add(new StudentStat
                {
                    Name = student.FullName,
                    Id = student.StudentNumber,
                    PresenceRate = (double)presentCount / totalSessions,
                    Absences = totalSessions - presentCount,
                    Initials = initials,
                    Color = assignedColor
                });

                totalPresent += presentCount;
            }

            PresencePercentage = totalExpected > 0 ? (double)totalPresent / totalExpected : 0;
            AbsencePercentage = 1 - PresencePercentage;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Stats Load Error: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OnExportClicked()
    {
        if (Students == null || !Students.Any())
        {
            await Shell.Current.DisplayAlertAsync("Export", "No data to export.", "OK");
            return;
        }

        try
        {
            IsBusy = true;
            
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Student ID,Name,Presence Rate,Absences");
            
            foreach (var student in Students)
            {
                sb.AppendLine($"{student.Id},{student.Name},{student.PresenceRate:P0},{student.Absences}");
            }

            var fileName = $"Report_{SelectedGrade}_{SelectedMonth}_{SelectedYear}.csv";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            
            await File.WriteAllTextAsync(filePath, sb.ToString());

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Export Attendance Report",
                File = new ShareFile(filePath)
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Export Error: {ex}");
            await Shell.Current.DisplayAlertAsync("Export Error", "Failed to export report.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}

