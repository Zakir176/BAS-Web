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
            vm.SubscribeToUpdates();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is StudentHomeViewModel vm)
        {
            vm.UnsubscribeFromUpdates();
        }
    }

    // DES-001: Removed orphaned OnShowIdClicked handler that showed hardcoded ID #210984.
    // ShowIdCardCommand in the ViewModel handles the correct navigation.

    private async void OnHistoryClicked(object sender, EventArgs e)
    {
        // Navigate to the History Page
        // We use "//" to ensure we switch tabs properly if it's in a TabBar
        // OR if you want to push it onto the stack:
        await Shell.Current.GoToAsync("//student/studentHistoryTab/studentHistory");
    }


}