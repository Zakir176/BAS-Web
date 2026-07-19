using System.Windows.Input;
using System.Collections.ObjectModel;
using catv1.Models;
using static Supabase.Postgrest.Constants;

namespace catv1.ViewModels;

public class ScanViewModel : BaseViewModel
{
    private readonly Supabase.Client _supabase;
    private bool _isSessionActive;
    private Section? _selectedSection;
    private DateTime _selectedDate = DateTime.Now;
    private ObservableCollection<Student> _roster = [];
    private ObservableCollection<Section> _sections = [];
    private Student? _lastScannedStudent;
    private bool _showFeedback;
    private string _scanFeedbackMessage = string.Empty;
    private bool _isScanError;

    // Debounce: track last processed barcode to prevent duplicate rapid scans (GAP 7)
    private string _lastScannedBarcode = string.Empty;
    private DateTime _lastScanTime = DateTime.MinValue;

    // GAP 3: track which student IDs have already had an attendance_logs row written mid-session
    private readonly HashSet<string> _writtenStudentIds = [];

    public ICommand BackCommand { get; }
    public ICommand StartSessionCommand { get; }
    public ICommand FinishSessionCommand { get; }
    public ICommand MarkStudentCommand { get; }
    public ICommand UndoScanCommand { get; }
    public ICommand LoadSectionsCommand { get; }

    public ScanViewModel(Supabase.Client supabase)
    {
        _supabase = supabase;
        BackCommand = new Command(OnBackClicked);
        StartSessionCommand = new Command(OnStartSession, () => SelectedSection != null);
        FinishSessionCommand = new Command(OnFinishSession);
        MarkStudentCommand = new Command<Student>(OnMarkStudent);
        UndoScanCommand = new Command(OnUndoScan);
        LoadSectionsCommand = new Command(async () => await LoadSectionsAsync());

        // Initial Load
        _ = LoadSectionsAsync();
    }

    public bool IsSessionActive
    {
        get => _isSessionActive;
        set => SetProperty(ref _isSessionActive, value);
    }

    public bool IsSetupVisible => !IsSessionActive;

    public ObservableCollection<Section> Sections
    {
        get => _sections;
        set => SetProperty(ref _sections, value);
    }

    public Section? SelectedSection
    {
        get => _selectedSection;
        set
        {
            if (SetProperty(ref _selectedSection, value))
            {
                OnPropertyChanged(nameof(SelectedCourse));
                ((Command)StartSessionCommand).ChangeCanExecute();
            }
        }
    }

    public string SelectedCourse => SelectedSection?.Name ?? "Select a Class";

    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            SetProperty(ref _selectedDate, value);
            OnPropertyChanged(nameof(DateDisplay));
            OnPropertyChanged(nameof(DayDisplay));
        }
    }

    public string DateDisplay => SelectedDate.ToString("MMM dd, yyyy");
    public string DayDisplay => SelectedDate.ToString("dd ddd");

    public ObservableCollection<Student> Roster
    {
        get => _roster;
        set => SetProperty(ref _roster, value);
    }

    public int PresentCount => Roster?.Count(s => s.IsPresent) ?? 0;
    public int AbsentCount => Roster?.Count(s => !s.IsPresent) ?? 0;
    public int TotalCount => Roster?.Count ?? 0;

    public double PresentProgress => TotalCount > 0 ? (double)PresentCount / TotalCount : 0;
    public double AbsentProgress => TotalCount > 0 ? (double)AbsentCount / TotalCount : 0;

    public bool ShowFeedback
    {
        get => _showFeedback;
        set => SetProperty(ref _showFeedback, value);
    }

    public Student? LastScannedStudent
    {
        get => _lastScannedStudent;
        set => SetProperty(ref _lastScannedStudent, value);
    }

    /// <summary>Message shown in the feedback toast — may be success or error.</summary>
    public string ScanFeedbackMessage
    {
        get => _scanFeedbackMessage;
        set => SetProperty(ref _scanFeedbackMessage, value);
    }

    /// <summary>True when the feedback toast represents an error (red), false for success (green).</summary>
    public bool IsScanError
    {
        get => _isScanError;
        set
        {
            if (SetProperty(ref _isScanError, value))
                OnPropertyChanged(nameof(IsNotScanError));
        }
    }

    public bool IsNotScanError => !IsScanError;

    public async Task LoadSectionsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var user = _supabase.Auth.CurrentUser;
            if (user == null) return;

            var response = await _supabase.From<Section>()
                .Where(x => x.LecturerId == user.Id)
                .Get();

            Sections.Clear();
            foreach (var section in response.Models)
            {
                Sections.Add(section);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadSections Error: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task LoadRosterAsync()
    {
        if (SelectedSection == null) return;

        try
        {
            IsBusy = true;
            // 1. Fetch Enrollments
            var enrollmentsResponse = await _supabase.From<Enrollment>()
                .Where(e => e.SectionId == SelectedSection.Id)
                .Get();

            var studentIds = enrollmentsResponse.Models.Select(e => e.StudentId).ToList();
            if (studentIds.Count == 0)
            {
                Roster.Clear();
                RefreshStats();
                return;
            }

            // 2. Fetch Students
            var studentsResponse = await _supabase.From<Student>()
                .Filter("id", Operator.In, studentIds.Cast<object>().ToList())
                .Get();

            Roster.Clear();
            foreach (var student in studentsResponse.Models.OrderBy(s => s.FullName))
            {
                Roster.Add(student);
            }
            RefreshStats();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadRoster Error: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnBackClicked()
    {
        if (IsSessionActive)
        {
            bool confirm = await Shell.Current.DisplayAlertAsync("End Session?", "This will end the current scanning session without saving.", "Continue", "Cancel");
            if (!confirm) return;
            IsSessionActive = false;
            OnPropertyChanged(nameof(IsSetupVisible));
        }
        else
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    private async void OnStartSession()
    {
        if (SelectedSection == null) return;

        await LoadRosterAsync();
        _writtenStudentIds.Clear(); // reset mid-session write tracking
        IsSessionActive = true;
        OnPropertyChanged(nameof(IsSetupVisible));
    }

    private async void OnFinishSession()
    {
        if (Roster == null || Roster.Count == 0 || SelectedSection == null) return;

        bool confirm = await Shell.Current.DisplayAlertAsync("Submit Attendance", $"Confirm attendance for {PresentCount} students?", "Submit", "Cancel");
        if (!confirm) return;

        try
        {
            IsBusy = true;
            var logs = new List<ActivityLog>();
            foreach (var student in Roster)
            {
                // GAP 3: skip students whose log was already written mid-session
                if (_writtenStudentIds.Contains(student.Id)) continue;

                logs.Add(new ActivityLog
                {
                    Id = Guid.NewGuid().ToString(),
                    StudentId = student.Id,
                    SectionId = SelectedSection.Id,
                    Status = student.IsPresent ? "Present" : "Absent",
                    DateTime = SelectedDate.Date.Add(DateTime.Now.TimeOfDay),
                    IsVerified = student.IsPresent,
                    IsExcused = false
                });
            }

            if (logs.Count > 0)
                await _supabase.From<ActivityLog>().Insert(logs);

            await Shell.Current.DisplayAlertAsync("Success", "Attendance has been synchronized with the database.", "OK");
            IsSessionActive = false;
            _writtenStudentIds.Clear();
            OnPropertyChanged(nameof(IsSetupVisible));
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"Failed to save attendance: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnMarkStudent(Student student)
    {
        if (student == null) return;

        student.IsPresent = !student.IsPresent;
        student.ScanTime = student.IsPresent ? DateTime.Now : null;

        RefreshStats();

        if (student.IsPresent)
        {
            LastScannedStudent = student;
            IsScanError = false;
            ScanFeedbackMessage = $"{student.FullName} marked Present";
            ShowFeedback = true;

            // GAP 3: write the attendance log immediately on mark, fire-and-forget
            _ = WriteAttendanceLogAsync(student);

            // Simple timer for feedback
            Task.Delay(3000).ContinueWith(_ => MainThread.BeginInvokeOnMainThread(() =>
            {
                ShowFeedback = false;
                ScanFeedbackMessage = string.Empty;
            }));
        }
    }

    /// <summary>Writes a single Present attendance_logs row immediately so the DB stays in sync mid-session (GAP 3).</summary>
    private async Task WriteAttendanceLogAsync(Student student)
    {
        if (SelectedSection == null || _writtenStudentIds.Contains(student.Id)) return;

        try
        {
            var log = new ActivityLog
            {
                Id = Guid.NewGuid().ToString(),
                StudentId = student.Id,
                SectionId = SelectedSection.Id,
                Status = "Present",
                DateTime = SelectedDate.Date.Add(DateTime.Now.TimeOfDay),
                IsVerified = true,
                IsExcused = false
            };

            await _supabase.From<ActivityLog>().Insert(log);
            _writtenStudentIds.Add(student.Id);
            System.Diagnostics.Debug.WriteLine($"[ScanPage] Attendance log written for {student.FullName}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ScanPage] Failed to write attendance log: {ex.Message}");
            // Non-fatal: OnFinishSession will retry for this student since the ID won't be in _writtenStudentIds
        }
    }

    private void OnUndoScan()
    {
        if (LastScannedStudent != null)
        {
            LastScannedStudent.IsPresent = false;
            LastScannedStudent.ScanTime = null;
            ShowFeedback = false;
            RefreshStats();
        }
    }

    private void RefreshStats()
    {
        OnPropertyChanged(nameof(PresentCount));
        OnPropertyChanged(nameof(AbsentCount));
        OnPropertyChanged(nameof(TotalCount));
        OnPropertyChanged(nameof(PresentProgress));
        OnPropertyChanged(nameof(AbsentProgress));
    }

    public async void OnBarcodeDetected(ZXing.Net.Maui.BarcodeResult[] results)
    {
        // Camera runs even on the setup screen; ignore detections until a session is started.
        if (!IsSessionActive) return;
        if (results == null || results.Length == 0 || IsBusy) return;

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            // Normalize: strip ALL whitespace and control characters (\r, \n, tabs, etc.)
            // Barcodes frequently emit trailing \r or \n which causes double-fire mismatches.
            var raw = results[0].Value;
            if (string.IsNullOrEmpty(raw)) return;
            var barcodeValue = string.Concat(raw.Where(c => !char.IsControl(c) && !char.IsWhiteSpace(c)));
            if (string.IsNullOrEmpty(barcodeValue)) return;

            // GAP 7 — Debounce: ignore the same barcode within 2 seconds (normalized comparison)
            if (barcodeValue == _lastScannedBarcode &&
                (DateTime.Now - _lastScanTime).TotalMilliseconds < 2000)
            {
                return;
            }

            _lastScannedBarcode = barcodeValue;
            _lastScanTime = DateTime.Now;

            System.Diagnostics.Debug.WriteLine($"[ScanPage] Barcode read: '{barcodeValue}'");

            // Match by StudentNumber or internal Id (barcodes encode StudentNumber)
            var student = Roster.FirstOrDefault(s =>
                s.StudentNumber?.Trim() == barcodeValue ||
                s.Id?.Trim() == barcodeValue);

            if (student != null)
            {
                if (!student.IsPresent)
                {
                    OnMarkStudent(student);
                }
                // Already marked — reset debounce so re-scanning after a period works
            }
            else
            {
                // Verify if the student exists in the database to distinguish between "Not Enrolled" and "Invalid Barcode"
                try
                {
                    var dbStudentResponse = await _supabase.From<Student>()
                        .Where(s => s.StudentNumber == barcodeValue || s.Id == barcodeValue)
                        .Get();

                    var dbStudent = dbStudentResponse.Models.FirstOrDefault();
                    if (dbStudent != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[ScanPage] Student {dbStudent.FullName} ({barcodeValue}) found in DB but not in Roster. (Not Enrolled)");
                        ShowScanError($"{dbStudent.FullName} is not enrolled in {SelectedCourse}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[ScanPage] No student found for barcode: {barcodeValue}");
                        ShowScanError($"No student found with ID: {barcodeValue}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ScanPage] Error querying student: {ex.Message}");
                    ShowScanError($"Error verifying ID: {barcodeValue}");
                }
            }

            await Task.CompletedTask;
        });
    }

    /// <summary>Shows an inline error in the feedback toast — non-blocking, auto-clears after 3 s.</summary>
    private void ShowScanError(string message)
    {
        IsScanError = true;
        ScanFeedbackMessage = message;
        ShowFeedback = true;

        Task.Delay(3000).ContinueWith(_ => MainThread.BeginInvokeOnMainThread(() =>
        {
            ShowFeedback = false;
            IsScanError = false;
            ScanFeedbackMessage = string.Empty;
        }));
    }
}
