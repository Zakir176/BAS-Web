using catv1.ViewModels;

namespace catv1.Views;

public partial class LecturerHomePage : ContentPage
{
    public LecturerHomePage(LecturerHomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is LecturerHomeViewModel vm)
        {
            await vm.LoadDataAsync();
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//login");
    }
}