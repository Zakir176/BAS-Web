using CommunityToolkit.Mvvm.ComponentModel;
using System.Runtime.CompilerServices;

namespace catv1.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set 
        {
            if (SetProperty(ref _isBusy, value))
            {
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }
    }

    public bool IsNotBusy => !IsBusy;

    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string CurrentDateDisplay => DateTime.Now.ToString("dddd, MMMM dd, yyyy");
    public string CurrentMonthDisplay => DateTime.Now.ToString("MMMM yyyy");
}
