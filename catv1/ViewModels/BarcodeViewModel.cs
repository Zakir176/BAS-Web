using CommunityToolkit.Mvvm.ComponentModel;

namespace catv1.ViewModels;

[QueryProperty("Name", "name")]
[QueryProperty("StudentId", "id")]
public partial class BarcodeViewModel : BaseViewModel
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string studentId = string.Empty;

    public BarcodeViewModel()
    {
        Title = "Digital ID";
    }
}
