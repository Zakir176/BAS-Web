using System.Collections.ObjectModel;
using System.Linq;
using catv1.Models;
using catv1.Services;
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
    private readonly IAuthService _authService;
    private readonly IProfileService _profileService;
    private readonly ICourseService _courseService;
    private readonly IAttendanceService _attendanceService;
    private readonly Supabase.Client _supabase; // For realtime
    private LecturerProfile? _profile;

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
            if (string.IsNullOrWhiteSpace(Name) || Name == "Lecturer") return "MU";

            var parts = Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                var firstInitial = parts[0].Length > 0 ? parts[0][0] : ' ';
                var lastInitial = parts[^1].Length > 0 ? parts[^1][0] : ' ';

                if (firstInitial != ' ' && lastInitial != ' ')
                    return $"{firstInitial}{lastInitial}".ToUpper();
            }

            return Name.Length >= 2 ? Name[..2].ToUpper() : "MU";
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

    public ObservableCollection<CourseChartItem> CourseAttendanceData { get; set; } = [];
    public ObservableCollection<Course> MyCourses { get; set; } = [];
    public ObservableCollection<Student> ActiveSessionStudents { get; set; } = [];

    // GAP 4: Roster search and filter
    private string _rosterSearchText = string.Empty;
    public string RosterSearchText
    {
        get => _rosterSearchText;
        set
        {
            if (SetProperty(ref _rosterSearchText, value))
                OnPropertyChanged(nameof(FilteredSessionStudents));
        }
    }

    private string _rosterFilter = "All"; // "All" | "Present" | "Absent"
    public string RosterFilter
    {
        get => _rosterFilter;
        set
        {
            if (SetProperty(ref _rosterFilter, value))
            {
                OnPropertyChanged(nameof(FilteredSessionStudents));
                OnPropertyChanged(nameof(IsFilterAll));
                OnPropertyChanged(nameof(IsFilterPresent));
                OnPropertyChanged(nameof(IsFilterAbsent));
            }
        }
    }

    public bool IsFilterAll => RosterFilter == "All";
    public bool IsFilterPresent => RosterFilter == "Present";
    public bool IsFilterAbsent => RosterFilter == "Absent";

    public IEnumerable<Student> FilteredSessionStudents
    {
        get
        {
            var list = ActiveSessionStudents.AsEnumerable();

            // Apply status filter
            if (RosterFilter == "Present")
                list = list.Where(s => s.IsPresent);
            else if (RosterFilter == "Absent")
                list = list.Where(s => !s.IsPresent);

            // Apply search text
            if (!string.IsNullOrWhiteSpace(RosterSearchText))
            {
                list = list.Where(s =>
                    (s.FullName?.Contains(RosterSearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (s.StudentNumber?.Contains(RosterSearchText, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            return list;
        }
    }

    public Command FilterAllCommand { get; }
    public Command FilterPresentCommand { get; }
    public Command FilterAbsentCommand { get; }

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
    public Command LogoutCommand { get; }
    public Command<Course> DeleteCourseCommand { get; }

    private Supabase.Realtime.RealtimeChannel? _channel;

    public LecturerHomeViewModel(IAuthService authService, IProfileService profileService, ICourseService courseService, IAttendanceService attendanceService, Supabase.Client supabase)
    {
        _authService = authService;
        _profileService = profileService;
        _courseService = courseService;
        _attendanceService = attendanceService;
        _supabase = supabase;
        Title = "Lecturer Dashboard";

        NewCourseCommand = new Command(async () => await OnNewCourse());
        NewSessionCommand = new Command(async () => await OnNewSession());
        StartScanningCommand = new Command(async () => await OnStartScanning());
        CreateCourseCommand = new Command(async () => await OnNewCourse());
        RefreshCommand = new Command(async () => await LoadDataAsync());
        SeeAllCoursesCommand = new Command(async () => await Shell.Current.GoToAsync("courseList"));
        ThemeToggleCommand = new Command(async () => await OnToggleTheme());
        NotificationsCommand = new Command(async () => await Shell.Current.GoToAsync("notifications"));
        LogoutCommand = new Command(async () => await OnLogout());
        FilterAllCommand = new Command(() => RosterFilter = "All");
        FilterPresentCommand = new Command(() => RosterFilter = "Present");
        FilterAbsentCommand = new Command(() => RosterFilter = "Absent");
        DeleteCourseCommand = new Command<Course>(async (course) => await OnDeleteCourse(course));

        // Connect Realtime
        SubscribeToUpdates();
    }

    private async Task OnLogout()
    {
        bool confirm = await Shell.Current.DisplayAlertAsync("Logout", "Are you sure you want to log out?", "Yes", "No");
        if (!confirm) return;

        try
        {
            await _authService.SignOutAsync();
            await Shell.Current.GoToAsync("//login");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Logout Error: {ex}");
            await Shell.Current.GoToAsync("//login"); // Go anyway
        }
    }

    private void SubscribeToUpdates()
    {
        try
        {
            _channel = _supabase.Realtime.Channel("attendance-updates");
            _channel.Register(new Supabase.Realtime.PostgresChanges.PostgresChangesOptions("public", "attendance_logs"));
            _channel.AddPostgresChangeHandler(Supabase.Realtime.PostgresChanges.PostgresChangesOptions.ListenType.Inserts, (sender, args) =>
            {
                MainThread.BeginInvokeOnMainThread(async () => await LoadDataAsync());
            });
            _channel.Subscribe();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Realtime Error: {ex}");
        }
    }

    private static async Task OnToggleTheme()
    {
        var res = await Shell.Current.DisplayActionSheetAsync("Select Theme", "Cancel", null, "Light", "Dark", "System");
        if (string.IsNullOrEmpty(res) || res == "Cancel") return;

        // This would interact with a ThemeManager in a real app
        await Shell.Current.DisplayAlertAsync("Theme", $"Theme changed to {res}!", "OK");
    }

    private async Task OnNewCourse()
    {
        string name = await Shell.Current.DisplayPromptAsync("New Course", "Enter course name:");
        if (string.IsNullOrWhiteSpace(name)) return;

        // Add a small delay to prevent the keyboard from disappearing between chained dialogs
        await Task.Delay(300);

        string code = await Shell.Current.DisplayPromptAsync("New Course", "Enter course code (e.g. CS101):");
        if (string.IsNullOrWhiteSpace(code)) return;

        try
        {
            IsBusy = true;
            var user = _supabase.Auth.CurrentUser;

            // --- Duplicate check: prevent creating the same course code twice ---
            string lecturerId = user != null ? user.Id ?? string.Empty : string.Empty;
            var existing = await _courseService.GetCourseByCodeAsync(code.Trim(), lecturerId);

            if (existing != null)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Course Already Exists",
                    $"A course with code '{code.Trim().ToUpper()}' already exists. Please use a unique course code.",
                    "OK");
                return;
            }

            var course = new Course
            {
                Id = Guid.NewGuid().ToString(),
                Name = name.Trim(),
                Code = code.Trim().ToUpper(),
                LecturerId = user?.Id ?? "",
                DepartmentId = _profile?.DepartmentId ?? ""
            };

            await _courseService.InsertCourseAsync(course);
            await LoadDataAsync();
            await Shell.Current.DisplayAlertAsync("Success", "Course created successfully!", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OnDeleteCourse(Course course)
    {
        if (course == null) return;

        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Delete Course",
            $"Are you sure you want to delete '{course.Name}' ({course.Code})? This cannot be undone.",
            "Delete",
            "Cancel");

        if (!confirm) return;

        try
        {
            IsBusy = true;
            await _courseService.DeleteCourseAsync(course.Id);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"Could not delete course: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OnNewSession()
    {
        var currentUser = _authService.CurrentUser;
        if (currentUser == null) return;

        // For a session we need a course selection
        var courses = await _courseService.GetCoursesByLecturerAsync(currentUser.Id ?? string.Empty);
        if (courses.Count == 0)
        {
            await Shell.Current.DisplayAlertAsync("Error", "Create a course first!", "OK");
            return;
        }

        string[] names = [.. courses.Select(c => c.Name)];
        string choice = await Shell.Current.DisplayActionSheetAsync("Select Course for Session", "Cancel", null, names);

        if (string.IsNullOrEmpty(choice) || choice == "Cancel") return;

        var selectedCourse = courses.First(c => c.Name == choice);

        try
        {
            IsBusy = true;
            var section = new Section
            {
                Id = Guid.NewGuid().ToString(),
                CourseId = selectedCourse.Id,
                Name = $"{selectedCourse.Name} - {DateTime.Now:MMM dd}",
                LecturerId = currentUser.Id ?? "",
                AcademicYear = DateTime.Now.Year,
                Semester = DateTime.Now.Month < 7 ? 1 : 2
            };

            await _courseService.InsertSectionAsync(section);
            await LoadDataAsync();
            await Shell.Current.DisplayAlertAsync("Success", "Session/Section created successfully!", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static async Task OnStartScanning()
    {
        await Shell.Current.GoToAsync("//lecturer/scanTab/scan");
    }

    public async Task LoadDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var user = _authService.CurrentUser;
            if (user == null) return;

            System.Diagnostics.Debug.WriteLine($"[LecturerDashboard] Fetching profile for user: {user.Id}");
            _profile = await _profileService.GetLecturerByEmailAsync(user.Email ?? string.Empty);

            if (_profile == null)
            {
                System.Diagnostics.Debug.WriteLine("[LecturerDashboard] ERROR: Profile not found!");
                // Try to create a basic profile if missing? No, better alert.
                await Shell.Current.DisplayAlertAsync("Profile Error", "Lecturer profile not found in database. Please ensure your account is correctly set up.", "OK");
                IsBusy = false;
                return;
            }

            Name = _profile.FullName ?? "Lecturer";

            // Fetch Department Name
            if (!string.IsNullOrEmpty(_profile.DepartmentId))
            {
                var deptResponse = await _profileService.GetDepartmentByIdAsync(_profile.DepartmentId);

                if (deptResponse != null)
                {
                    Department = deptResponse.Name;
                }
                else
                {
                    Department = "Unknown Department";
                }
            }
            else
            {
                Department = "No Department Assigned";
            }

            // --- LIVE DATA FETCHING ---

            System.Diagnostics.Debug.WriteLine($"[LecturerDashboard] Profile found: {_profile.FullName}. Fetching courses...");

            // Use user.Id if _profile.Id seems inconsistent (Supabase Auth ID is preferred for consistency)
            string? lookupId = !string.IsNullOrEmpty(_profile.Id) && _profile.Id.Length > 10 ? _profile.Id : user.Id;

            if (string.IsNullOrEmpty(lookupId))
            {
                lookupId = user.Id; // Final fallback
            }

            var coursesResponse = await _courseService.GetCoursesByLecturerAsync(lookupId ?? string.Empty);

            TotalCourses = coursesResponse.Count;
            MyCourses.Clear();
            foreach (var course in coursesResponse)
            {
                MyCourses.Add(course);
            }

            System.Diagnostics.Debug.WriteLine($"[LecturerDashboard] Found {TotalCourses} courses.");

            // 2. Fetch Sections
            var sections = await _courseService.GetSectionsByLecturerAsync(lookupId ?? string.Empty);
            var sectionIds = sections.Select(s => s.Id).ToList();

            var logs = new List<ActivityLog>();

            if (sectionIds.Count > 0)
            {
                // 3. Fetch Enrollments for unique student count
                var enrollmentsResponse = await _courseService.GetEnrollmentsBySectionsAsync(sectionIds);
                EnrolledStudents = enrollmentsResponse.Select(e => e.StudentId).Distinct().Count();

                // 4. Fetch Attendance Logs for avg. attendance
                logs = await _attendanceService.GetSectionLogsAsync(sectionIds);

                if (logs.Count > 0)
                {
                    var presentCount = logs.Count(l => l.Status == "Present" || l.Status == "Late");
                    AvgAttendance = $"{(int)((double)presentCount / logs.Count * 100)}%";
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

                foreach (var course in coursesResponse)
                {
                    var courseSections = sections.Where(s => s.CourseId == course.Id).Select(s => s.Id).ToList();
                    if (courseSections.Count > 0)
                    {
                        var courseLogs = logs.Where(l => courseSections.Contains(l.SectionId)).ToList();
                        if (courseLogs.Count > 0)
                        {
                            var rate = (double)courseLogs.Count(l => l.Status == "Present" || l.Status == "Late") / courseLogs.Count;
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
                    var enrolledIds = enrollmentsResponse.Where(e => e.SectionId == latestLog.SectionId).Select(e => e.StudentId).ToList();

                    ActiveSessionStats = $"{sessionLogs.Count(l => l.Status == "Present" || l.Status == "Late")} Present / {enrolledIds.Count} Total";

                    // To show a roster, we'd need to fetch student names. 
                    // For this enhancement, we'll fetch the students for this section.
                    if (enrolledIds.Count > 0)
                    {
                        var studentsResponse = await _profileService.GetStudentsByIdsAsync(enrolledIds);

                        foreach (var student in studentsResponse)
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

            // GAP 6: Update Session Subtitle to show today's count based on distinct sections in today's logs
            var todaysLogs = logs.Where(l => l.DateTime.Date == DateTime.Today).ToList();
            var todaysSessionCount = todaysLogs.Select(l => l.SectionId).Distinct().Count();
            SessionCountSubtitle = $"You have {todaysSessionCount} session{(todaysSessionCount == 1 ? "" : "s")} today";

            OnPropertyChanged(nameof(HasCourses));
            OnPropertyChanged(nameof(HasNoCourses));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadData Error: {ex}");
            await Shell.Current.DisplayAlertAsync("Data Error", "Unable to load dashboard data. Please check your connection.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}

