using System.Collections.ObjectModel;
using catv1.Models;
using static Supabase.Postgrest.Constants;

namespace catv1.ViewModels;

public class HistoryViewModel : BaseViewModel
{
    private readonly Supabase.Client _supabase;
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

    public HistoryViewModel(Supabase.Client supabase)
    {
        _supabase = supabase;
        SelectedMonth = Months[DateTime.Now.Month - 1];
        SelectedYear = DateTime.Now.Year.ToString();
        
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
            var user = _supabase.Auth.CurrentUser;
            if (user == null) return;

            var sections = await _supabase.From<Section>().Where(s => s.LecturerId == user.Id).Get();
            Grades.Clear();
            foreach (var s in sections.Models) Grades.Add(s.Name);
            
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
            var user = _supabase.Auth.CurrentUser;
            if (user == null) return;

            // 1. Get Section ID
            var section = (await _supabase.From<Section>()
                .Where(s => s.Name == SelectedGrade && s.LecturerId == user.Id)
                .Get()).Models.FirstOrDefault();

            if (section == null) return;

            // 2. Get Enrollments
            var enrollments = await _supabase.From<Enrollment>()
                .Where(e => e.SectionId == section.Id)
                .Get();
            
            var studentIds = enrollments.Models.Select(e => e.StudentId).ToList();
            if (!studentIds.Any())
            {
                Students.Clear();
                PresencePercentage = 0;
                AbsencePercentage = 0;
                return;
            }

            // 3. Get Students and Logs
            var studentsResponse = await _supabase.From<Student>().Filter("id", Operator.In, studentIds.Cast<object>().ToList()).Get();
            var logsResponse = await _supabase.From<ActivityLog>().Where(l => l.SectionId == section.Id).Get();
            
            var allStudents = studentsResponse.Models;
            var allLogs = logsResponse.Models;

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
}
