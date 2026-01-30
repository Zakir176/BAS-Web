using catv1.ViewModels;

namespace catv1.Views;

public partial class StudentHomePage : ContentPage
{
    public StudentHomePage(StudentHomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is StudentHomeViewModel vm)
        {
            await vm.LoadDataAsync();
        }
    }

    private async void OnShowIdClicked(object sender, EventArgs e)
    {
        // For now, just show a pop-up. 
        // Later, this could navigate to a full-screen QR code page.
        await DisplayAlertAsync("Digital ID", "Showing ID for Student #210984...", "Close");
    }

    private async void OnHistoryClicked(object sender, EventArgs e)
    {
        // Navigate to the History Page
        // We use "///" to ensure we switch tabs properly if it's in a TabBar
        // OR if you want to push it onto the stack:
        await Shell.Current.GoToAsync("//student/historyTab/history");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//login");
    }
}