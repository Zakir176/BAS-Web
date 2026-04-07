using System.Collections.ObjectModel;
using System.Linq;
using catv1.Models;
namespace catv1.ViewModels;

public class CourseChartItem
{
    public string CourseName { get; set; } = string.Empty;
    public double AttendanceRate { get; set; }
    public string Color { get; set; } = "#3B82F6";
    public double BarWidth => AttendanceRate * 200; // Max width constant for UI
}

public class LecturerHomeViewModel : BaseViewModel
{
    private readonly Supabase.Client _supabase;

    private string _name = "Lecturer";
    public string Name
    {
        get => _name;
        set 
        {
            if (SetProperty(ref _name, value))
            {
                OnPropertyChanged(nameof(Initials));
            }
        }
    }

    public string Initials
    {
        get
        {
            if (string.IsNullOrEmpty(Name) || Name == "Lecturer") return "MU";
            var parts = Name.Split(' ');
            if (parts.Length >= 2) return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            return Name.Length >= 2 ? Name.Substring(0, 2).ToUpper() : "MU";
        }
    }

    private string _sessionCountSubtitle = "You have 0 sessions scheduled for today";
    public string SessionCountSubtitle
    {
        get => _sessionCountSubtitle;
        set => SetProperty(ref _sessionCountSubtitle, value);
    }

    private string _department = "---";
    public string Department
    {
        get => _department;
        set => SetProperty(ref _department, value);
    }

    private int _totalCourses = 0;
    public int TotalCourses
    {
        get => _totalCourses;
        set 
        {
            if (SetProperty(ref _totalCourses, value))
            {
                OnPropertyChanged(nameof(HasCourses));
                OnPropertyChanged(nameof(HasNoCourses));
            }
        }
    }

    private int _enrolledStudents = 0;
    public int EnrolledStudents
    {
        get => _enrolledStudents;
        set => SetProperty(ref _enrolledStudents, value);
    }

    private string _avgAttendance = "0%";
    public string AvgAttendance
    {
        get => _avgAttendance;
        set => SetProperty(ref _avgAttendance, value);
    }

    private bool _isConnected = false;
    public bool IsConnected
    {
        get => _isConnected;
        set => SetProperty(ref _isConnected, value);
    }

    private int _presentCount = 0;
    public int PresentCount
    {
        get => _presentCount;
        set => SetProperty(ref _presentCount, value);
    }

    private int _totalStudentCount = 0;
    public int TotalStudentCount
    {
        get => _totalStudentCount;
        set => SetProperty(ref _totalStudentCount, value);
    }

    public ObservableCollection<CourseChartItem> CourseAttendanceData { get; set; } = new();
    public ObservableCollection<Student> ActiveSessionStudents { get; set; } = new();

    private string _activeSessionName = "No Active Session";
    public string ActiveSessionName
    {
        get => _activeSessionName;
        set => SetProperty(ref _activeSessionName, value);
    }

    private string _activeSessionStats = "0 Present / 0 Total";
    public string ActiveSessionStats
    {
        get => _activeSessionStats;
        set => SetProperty(ref _activeSessionStats, value);
    }

    private bool _isSessionActive = false;
    public bool IsSessionActive
    {
        get => _isSessionActive;
        set => SetProperty(ref _isSessionActive, value);
    }

    public bool HasCourses => TotalCourses > 0;
    public bool HasNoCourses => TotalCourses == 0;

    public Command NewCourseCommand { get; }
    public Command NewSessionCommand { get; }
    public Command StartScanningCommand { get; }
    public Command CreateCourseCommand { get; }
    public Command RefreshCommand { get; }
    public Command SeeAllCoursesCommand { get; }
    public Command ThemeToggleCommand { get; }
    public Command NotificationsCommand { get; }

    public LecturerHomeViewModel(Supabase.Client supabase)
    {
        _supabase = supabase;
        Title = "Lecturer Dashboard";

        NewCourseCommand = new Command(async () => await OnNewCourse());
        NewSessionCommand = new Command(async () => await OnNewSession());
        StartScanningCommand = new Command(async () => await OnStartScanning());
        CreateCourseCommand = new Command(async () => await OnNewCourse());
        RefreshCommand = new Command(async () => await LoadDataAsync());
        SeeAllCoursesCommand = new Command(async () => await Shell.Current.DisplayAlertAsync("Courses", "Course list coming soon!", "OK"));
        ThemeToggleCommand = new Command(async () => await Shell.Current.DisplayAlertAsync("Theme", "Theme switching coming soon!", "OK"));
        NotificationsCommand = new Command(async () => await Shell.Current.DisplayAlertAsync("Notifications", "No new notifications.", "OK"));
    }

    private async Task OnNewCourse()
    {
        await Shell.Current.DisplayAlertAsync("New Course", "Course creation feature coming soon!", "OK");
    }

    private async Task OnNewSession()
    {
        await Shell.Current.DisplayAlertAsync("New Session", "Session management feature coming soon!", "OK");
    }

    private async Task OnStartScanning()
    {
        await Shell.Current.GoToAsync("//lecturer/scanTab/scan");
    }

    public async Task LoadDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var user = _supabase.Auth.CurrentUser;
            if (user == null) return;

            System.Diagnostics.Debug.WriteLine($"[LecturerDashboard] Fetching profile for user: {user.Id}");
            var profileResponse = (await _supabase.From<LecturerProfile>()
                .Where(l => l.Id == user.Id)
                .Get()).Models.FirstOrDefault();

            if (profileResponse == null)
            {
                System.Diagnostics.Debug.WriteLine("[LecturerDashboard] ERROR: Profile not found!");
                IsBusy = false;
                return;
            }

            Name = profileResponse.FullName;
                
                // Fetch Department Name
                if (!string.IsNullOrEmpty(profileResponse.DepartmentId))
                {
                    var deptResponse = (await _supabase.From<Department>()
                        .Where(d => d.Id == profileResponse.DepartmentId)
                        .Get()).Models.FirstOrDefault();
                    
                    if (deptResponse != null)
                    {
                        Department = deptResponse.Name;
                    }
                    else
                    {
                        Department = "Unknown Department";
                    }
                }

                // --- LIVE DATA FETCHING ---

                System.Diagnostics.Debug.WriteLine($"[LecturerDashboard] Profile found: {profileResponse.FullName}. Fetching courses...");
                var coursesResponse = await _supabase.From<Course>()
                    .Where(c => c.LecturerId == profileResponse.Id)
                    .Get();
                TotalCourses = coursesResponse.Models.Count;
                System.Diagnostics.Debug.WriteLine($"[LecturerDashboard] Found {TotalCourses} courses.");

                // 2. Fetch Sections
                var sectionsResponse = await _supabase.From<Section>()
                    .Where(s => s.LecturerId == profileResponse.Id)
                    .Get();
                var sections = sectionsResponse.Models;
                var sectionIds = sections.Select(s => s.Id).ToList();

                if (sectionIds.Any())
                {
                    // 3. Fetch Enrollments for unique student count
                    var enrollmentsResponse = await _supabase.From<Enrollment>()
                        .Filter("section_id", Supabase.Postgrest.Constants.Operator.In, sectionIds)
                        .Get();
                    EnrolledStudents = enrollmentsResponse.Models.Select(e => e.StudentId).Distinct().Count();

                    // 4. Fetch Attendance Logs for avg. attendance
                    var logsResponse = await _supabase.From<ActivityLog>()
                        .Filter("section_id", Supabase.Postgrest.Constants.Operator.In, sectionIds)
                        .Get();
                    var logs = logsResponse.Models;

                    if (logs.Any())
                    {
                        var presentCount = logs.Count(l => l.Status == "Present" || l.Status == "Late");
                        AvgAttendance = $"{(int)((double)presentCount / logs.Count() * 100)}%";
                    }
                    else
                    {
                        AvgAttendance = "0%";
                    }

                    // --- NEW: Course Attendance Overview Data ---
                    CourseAttendanceData.Clear();
                    var courseStats = new List<CourseChartItem>();
                    var colors = new[] { "#3B82F6", "#10B981", "#F59E0B", "#8B5CF6", "#EC4899" };
                    int colorIndex = 0;

                    foreach (var course in coursesResponse.Models)
                    {
                        var courseSections = sections.Where(s => s.CourseId == course.Id).Select(s => s.Id).ToList();
                        if (courseSections.Any())
                        {
                            var courseLogs = logs.Where(l => courseSections.Contains(l.SectionId)).ToList();
                            if (courseLogs.Any())
                            {
                                var rate = (double)courseLogs.Count(l => l.Status == "Present" || l.Status == "Late") / courseLogs.Count();
                                courseStats.Add(new CourseChartItem
                                {
                                    CourseName = course.Name,
                                    AttendanceRate = rate,
                                    Color = colors[colorIndex % colors.Length]
                                });
                                colorIndex++;
                            }
                        }
                    }
                    
                    foreach (var item in courseStats.OrderByDescending(c => c.AttendanceRate).Take(5))
                    {
                        CourseAttendanceData.Add(item);
                    }

                    // --- NEW: Active Session Roster Logic ---
                    ActiveSessionStudents.Clear();
                    // Find the most recent session date from logs
                    var latestLog = logs.OrderByDescending(l => l.DateTime).FirstOrDefault();
                    if (latestLog != null && latestLog.DateTime.Date == DateTime.Today)
                    {
                        IsSessionActive = true;
                        var activeSection = sections.FirstOrDefault(s => s.Id == latestLog.SectionId);
                        ActiveSessionName = activeSection?.Name ?? "Daily Session";
                        
                        var sessionLogs = logs.Where(l => l.SectionId == latestLog.SectionId && l.DateTime.Date == DateTime.Today).ToList();
                        var enrolledIds = enrollmentsResponse.Models.Where(e => e.SectionId == latestLog.SectionId).Select(e => e.StudentId).ToList();
                        
                        ActiveSessionStats = $"{sessionLogs.Count(l => l.Status == "Present" || l.Status == "Late")} Present / {enrolledIds.Count} Total";

                        // To show a roster, we'd need to fetch student names. 
                        // For this enhancement, we'll fetch the students for this section.
                        if (enrolledIds.Any())
                        {
                            var studentsResponse = await _supabase.From<Student>()
                                .Filter("id", Supabase.Postgrest.Constants.Operator.In, enrolledIds)
                                .Get();
                            
                            foreach (var student in studentsResponse.Models)
                            {
                                var log = sessionLogs.FirstOrDefault(l => l.StudentId == student.Id);
                                student.IsPresent = log != null && (log.Status == "Present" || log.Status == "Late");
                                student.ScanTime = log?.DateTime;
                                ActiveSessionStudents.Add(student);
                            }
                        }
                    }
                    else
                    {
                        IsSessionActive = false;
                        ActiveSessionName = "No Active Session";
                        ActiveSessionStats = "0 Present / 0 Total";
                    }
                }
                else
                {
                    EnrolledStudents = 0;
                    AvgAttendance = "0%";
                    CourseAttendanceData.Clear();
                    IsSessionActive = false;
                }

                // Update Session Subtitle (Mock logic for now as we don't have a schedule yet)
                SessionCountSubtitle = $"You have {sections.Count()} sections active this semester";

                OnPropertyChanged(nameof(HasCourses));
                OnPropertyChanged(nameof(HasNoCourses));
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
