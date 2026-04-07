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

    public void OnBarcodeDetected(ZXing.Net.Maui.BarcodeResult[] results)
    {
        if (results == null || results.Length == 0) return;

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

    private void GenerateRoster()
    {
        Roster =
            [
                new Student { FullName="John Doe", StudentNumber="210001", Id = Guid.NewGuid().ToString() },
                new Student { FullName="Jane Smith", StudentNumber="210002", Id = Guid.NewGuid().ToString() },
                new Student { FullName="Michael Brown", StudentNumber="210003", Id = Guid.NewGuid().ToString() },
                new Student { FullName="Sarah Johnson", StudentNumber="210004", Id = Guid.NewGuid().ToString() },
                new Student { FullName="David Wilson", StudentNumber="210005", Id = Guid.NewGuid().ToString() },
                new Student { FullName="Emily Davis", StudentNumber="210006", Id = Guid.NewGuid().ToString() },
                new Student { FullName="James Miller", StudentNumber="210007", Id = Guid.NewGuid().ToString() },
                new Student { FullName="Olivia Taylor", StudentNumber="210008", Id = Guid.NewGuid().ToString() },
                new Student { FullName="Robert Anderson", StudentNumber="210009", Id = Guid.NewGuid().ToString() },
                new Student { FullName="Sophia Thomas", StudentNumber="210010", Id = Guid.NewGuid().ToString() },
                new Student { FullName="William Jackson", StudentNumber="210011", Id = Guid.NewGuid().ToString() },
                new Student { FullName="Isabella White", StudentNumber="210012", Id = Guid.NewGuid().ToString() }
            ];
    }
}
