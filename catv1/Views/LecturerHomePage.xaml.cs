namespace catv1.Views;

public partial class LecturerHomePage : ContentPage
{
    public LecturerHomePage()
    {
        InitializeComponent();
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//login");
    }
}