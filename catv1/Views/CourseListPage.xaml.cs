using catv1.ViewModels;

namespace catv1.Views;

public partial class CourseListPage : ContentPage
{
    private readonly CourseListViewModel _viewModel;

    public CourseListPage(CourseListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCoursesAsync();
    }

    private async void OnBackClicked(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
