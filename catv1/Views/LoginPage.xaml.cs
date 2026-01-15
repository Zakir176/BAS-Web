namespace catv1.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private void OnStudentClicked(object sender, EventArgs e)
    {
        BtnStudent.BackgroundColor = Color.FromArgb("#3B82F6"); // Blue
        BtnStudent.TextColor = Colors.White;
        
        BtnLecturer.BackgroundColor = Colors.Transparent;
        BtnLecturer.TextColor = Color.FromArgb("#94A3B8"); // Gray

        LblWelcomeTitle.Text = "Student Login";
    }

    private void OnLecturerClicked(object sender, EventArgs e)
    {
        BtnLecturer.BackgroundColor = Color.FromArgb("#3B82F6"); // Blue
        BtnLecturer.TextColor = Colors.White;

        BtnStudent.BackgroundColor = Colors.Transparent;
        BtnStudent.TextColor = Color.FromArgb("#94A3B8"); // Gray

        LblWelcomeTitle.Text = "Lecturer Login";
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }
}