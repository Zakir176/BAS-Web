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
            if (!studentIds.Any())
            {
                Roster.Clear();
                RefreshStats();
                return;
            }

            // 2. Fetch Students
            var studentsResponse = await _supabase.From<Student>()
                .Filter("id", Operator.In, studentIds)
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
        IsSessionActive = true;
        OnPropertyChanged(nameof(IsSetupVisible));
    }

    private async void OnFinishSession()
    {
        if (Roster == null || !Roster.Any() || SelectedSection == null) return;

        bool confirm = await Shell.Current.DisplayAlertAsync("Submit Attendance", $"Confirm attendance for {PresentCount} students?", "Submit", "Cancel");
        if (!confirm) return;

        try
        {
            IsBusy = true;
            var logs = new List<ActivityLog>();
            foreach (var student in Roster)
            {
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

            await _supabase.From<ActivityLog>().Insert(logs);
            
            await Shell.Current.DisplayAlertAsync("Success", "Attendance has been synchronized with the database.", "OK");
            IsSessionActive = false;
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
            ShowFeedback = true;

            // Simple timer for feedback
            Task.Delay(3000).ContinueWith(_ => MainThread.BeginInvokeOnMainThread(() => ShowFeedback = false));
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

    public void OnBarcodeDetected(ZXing.Net.Maui.BarcodeResult[] results)
    {
        if (results == null || results.Length == 0 || IsBusy) return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            foreach (var result in results)
            {
                var student = Roster.FirstOrDefault(s => s.StudentNumber == result.Value);
                if (student != null && !student.IsPresent)
                {
                    OnMarkStudent(student);
                    break;
                }
            }
        });
    }
}
