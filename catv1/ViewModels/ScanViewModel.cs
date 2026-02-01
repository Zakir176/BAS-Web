using System.Windows.Input;
using System.Collections.ObjectModel;
using catv1.Models;

namespace catv1.ViewModels;

public class ScanViewModel : BaseViewModel
{
    private bool _isSessionActive;
    private string _selectedCourse = "Morning Biology";
    private DateTime _selectedDate = DateTime.Now;
    private ObservableCollection<Student> _roster = [];
    private Student? _lastScannedStudent;
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

    public Student? LastScannedStudent
    {
        get => _lastScannedStudent;
        set => SetProperty(ref _lastScannedStudent, value);
    }

    private async void OnBackClicked()
    {
        if (IsSessionActive)
        {
            bool confirm = await Shell.Current.DisplayAlertAsync("End Session?", "This will end the current scanning session.", "End Session", "Cancel");
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
        await Shell.Current.DisplayAlertAsync("Session Complete", $"Attendance marked for {SelectedCourse}.\nPresent: {PresentCount}\nAbsent: {AbsentCount}", "OK");
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
            Application.Current?.Dispatcher.StartTimer(TimeSpan.FromSeconds(3), () =>
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
        Roster =
            [
                new Student { Id="2024-001", FirstName="John", LastName="Doe", StudentId="210001" },
                new Student { Id="2024-002", FirstName="Jane", LastName="Smith", StudentId="210002" },
                new Student { Id="2024-003", FirstName="Michael", LastName="Brown", StudentId="210003" },
                new Student { Id="2024-004", FirstName="Sarah", LastName="Johnson", StudentId="210004" },
                new Student { Id="2024-005", FirstName="David", LastName="Wilson", StudentId="210005" },
                new Student { Id="2024-006", FirstName="Emily", LastName="Davis", StudentId="210006" },
                new Student { Id="2024-007", FirstName="James", LastName="Miller", StudentId="210007" },
                new Student { Id="2024-008", FirstName="Olivia", LastName="Taylor", StudentId="210008" },
                new Student { Id="2024-009", FirstName="Robert", LastName="Anderson", StudentId="210009" },
                new Student { Id="2024-010", FirstName="Sophia", LastName="Thomas", StudentId="210010" },
                new Student { Id="2024-011", FirstName="William", LastName="Jackson", StudentId="210011" },
                new Student { Id="2024-012", FirstName="Isabella", LastName="White", StudentId="210012" }
            ];
    }
}
