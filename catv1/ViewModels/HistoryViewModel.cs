using System.Collections.ObjectModel;
using catv1.Models;

namespace catv1.ViewModels;

public class HistoryViewModel : BaseViewModel
{
    private string? _selectedGrade;
    private string? _selectedMonth;
    private string? _selectedYear;

    public ObservableCollection<StudentStat> Students { get; set; }
    public ObservableCollection<string> Grades { get; } = new() { "Grade 10-A", "Grade 10-B", "Grade 11-A" };
    public ObservableCollection<string> Months { get; } = new() { "October", "November", "December" };
    public ObservableCollection<string> Years { get; } = new() { "2023", "2024" };

    public string? SelectedGrade
    {
        get => _selectedGrade;
        set => SetProperty(ref _selectedGrade, value);
    }

    public string? SelectedMonth
    {
        get => _selectedMonth;
        set => SetProperty(ref _selectedMonth, value);
    }

    public string? SelectedYear
    {
        get => _selectedYear;
        set => SetProperty(ref _selectedYear, value);
    }

    public double PresencePercentage { get; } = 0.94;
    public double AbsencePercentage { get; } = 0.06;

    public string PresencePercentageDisplay => $"{PresencePercentage:P0}";
    public string AbsencePercentageDisplay => $"{AbsencePercentage:P0}";

    public HistoryViewModel()
    {
        SelectedGrade = Grades[0];
        SelectedMonth = Months[0];
        SelectedYear = Years[0];

        Students = new ObservableCollection<StudentStat>
        {
            new StudentStat { Name = "Jane Cooper", Id = "#00124", PresenceRate = 0.98, Absences = 2, Avatar = "dotnet_bot.png" },
            new StudentStat { Name = "Robert Fox", Id = "#00125", PresenceRate = 0.82, Absences = 6, Avatar = "dotnet_bot.png" },
            new StudentStat { Name = "Esther Howard", Id = "#00126", PresenceRate = 0.95, Absences = 1, Avatar = "dotnet_bot.png" },
            new StudentStat { Name = "Cameron Williamson", Id = "#00127", PresenceRate = 0.71, Absences = 12, Avatar = "dotnet_bot.png" },
             new StudentStat { Name = "Jenny Wilson", Id = "#00128", PresenceRate = 0.88, Absences = 4, Avatar = "dotnet_bot.png" }
        };
    }
}
