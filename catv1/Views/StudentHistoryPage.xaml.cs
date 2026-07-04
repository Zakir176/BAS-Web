using catv1.ViewModels;

namespace catv1.Views;

public partial class StudentHistoryPage : ContentPage
{
    private readonly StudentHistoryViewModel _viewModel;

    public StudentHistoryPage(StudentHistoryViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadHistoryAsync();
    }

    private async void OnBackClicked(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
