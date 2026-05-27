using CommunityToolkit.Mvvm.ComponentModel;

namespace catv1.ViewModels;

[QueryProperty("Name", "name")]
[QueryProperty("StudentId", "id")]
public partial class BarcodeViewModel : BaseViewModel
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string studentId = "00000000";

    public BarcodeViewModel()
    {
        Title = "Digital ID";
    }
}
