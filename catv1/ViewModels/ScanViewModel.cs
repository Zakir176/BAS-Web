using System.Windows.Input;
using System.Collections.ObjectModel;
using catv1.Models;

namespace catv1.ViewModels;

public class ScanViewModel : BaseViewModel
{
    private bool _isSessionActive;
    private string _selectedCourse = "Morning Biology";
    private DateTime _selectedDate = DateTime.Now;
    private ObservableCollection<Student> _roster;
    private Student _lastScannedStudent;
    private bool _showFeedback;

    public ICommand BackCommand { get; }
    public ICommand StartSessionCommand { get; }
    public ICommand FinishSessionCommand { get; }
    public ICommand MarkStudentCommand { get; }
    public ICommand UndoScanCommand { get; }

    public ScanViewModel()
    {
        BackCommand = new Command(OnBackClicked);
        StartSessionCommand = new Command(OnStartSession);
        FinishSessionCommand = new Command(OnFinishSession);
        MarkStudentCommand = new Command<Student>(OnMarkStudent);
        UndoScanCommand = new Command(OnUndoScan);

        // Mock Data setup
        GenerateRoster();
    }

    public bool IsSessionActive
    {
        get => _isSessionActive;
        set => SetProperty(ref _isSessionActive, value);
    }

    public bool IsSetupVisible => !IsSessionActive;

    public string SelectedCourse
    {
        get => _selectedCourse;
        set => SetProperty(ref _selectedCourse, value);
    }

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
    public string DayDisplay => SelectedDate.ToString("dddd");

    public ObservableCollection<Student> Roster
    {
        get => _roster;
        set => SetProperty(ref _roster, value);
    }

    public int PresentCount => Roster?.Count(s => s.IsPresent) ?? 0;
    public int AbsentCount => Roster?.Count(s => !s.IsPresent) ?? 0;
    public int TotalCount => Roster?.Count ?? 0;

    // Progress Bars (0 to 1)
    public double PresentProgress => TotalCount > 0 ? (double)PresentCount / TotalCount : 0;
    public double AbsentProgress => TotalCount > 0 ? (double)AbsentCount / TotalCount : 0;

    // Feedback Toast
    public bool ShowFeedback
    {
        get => _showFeedback;
        set => SetProperty(ref _showFeedback, value);
    }

    public Student LastScannedStudent
    {
        get => _lastScannedStudent;
        set => SetProperty(ref _lastScannedStudent, value);
    }

    private async void OnBackClicked()
    {
        if (IsSessionActive)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("End Session?", "This will end the current scanning session.", "End Session", "Cancel");
            if (!confirm) return;
            IsSessionActive = false;
            OnPropertyChanged(nameof(IsSetupVisible));
        }
        else
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    private void OnStartSession()
    {
        IsSessionActive = true;
        OnPropertyChanged(nameof(IsSetupVisible));
    }

    private async void OnFinishSession()
    {
        // Here you would save the data to a backend
        await Application.Current.MainPage.DisplayAlert("Session Complete", $"Attendance marked for {SelectedCourse}.\nPresent: {PresentCount}\nAbsent: {AbsentCount}", "OK");
        IsSessionActive = false;
        OnPropertyChanged(nameof(IsSetupVisible));
    }

    private void OnMarkStudent(Student student)
    {
        if (student == null) return;

        // Toggle
        student.IsPresent = !student.IsPresent;
        student.ScanTime = student.IsPresent ? DateTime.Now : null;

        // Update stats
        RefreshStats();

        if (student.IsPresent)
        {
            LastScannedStudent = student;
            ShowFeedback = true;

            // Hide feedback after 3 seconds
            Device.StartTimer(TimeSpan.FromSeconds(3), () =>
            {
                ShowFeedback = false;
                return false;
            });
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
        OnPropertyChanged(nameof(PresentProgress));
        OnPropertyChanged(nameof(AbsentProgress));
    }

    private void GenerateRoster()
    {
        Roster = new ObservableCollection<Student>
            {
                new Student { Id="2024-001", Name="John Doe" },
                new Student { Id="2024-002", Name="Jane Smith" },
                new Student { Id="2024-003", Name="Michael Brown" },
                new Student { Id="2024-004", Name="Sarah Johnson" },
                new Student { Id="2024-005", Name="David Wilson" },
                new Student { Id="2024-006", Name="Emily Davis" },
                new Student { Id="2024-007", Name="James Miller" },
                new Student { Id="2024-008", Name="Olivia Taylor" },
                new Student { Id="2024-009", Name="Robert Anderson" },
                new Student { Id="2024-010", Name="Sophia Thomas" },
                new Student { Id="2024-011", Name="William Jackson" },
                new Student { Id="2024-012", Name="Isabella White" }
            };
    }
}
