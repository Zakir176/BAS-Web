using System.Windows.Input;

namespace catv1.ViewModels;

public class ScanViewModel : BaseViewModel
{
    public ICommand BackCommand { get; }

    public ScanViewModel()
    {
        BackCommand = new Command(OnBackClicked);
    }

    private async void OnBackClicked()
    {
        await Shell.Current.GoToAsync("..");
    }
}
