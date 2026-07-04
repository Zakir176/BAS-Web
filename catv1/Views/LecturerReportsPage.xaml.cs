using catv1.ViewModels;

namespace catv1.Views;

public partial class LecturerReportsPage : ContentPage
{
    public LecturerReportsPage(LecturerReportsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//student/studentDashboardTab/home");
    }
}
