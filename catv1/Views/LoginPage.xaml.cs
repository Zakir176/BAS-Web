using Microsoft.Maui.Controls;

namespace catv1.Views;

public partial class LoginPage : ContentPage
{
    private bool _isStudent = true; // Default to Student

    public LoginPage()
    {
        System.Diagnostics.Debug.WriteLine("CAT_LOG: LoginPage Constructor Start");
        InitializeComponent();
        UpdateUI(); // Set initial state
        System.Diagnostics.Debug.WriteLine("CAT_LOG: LoginPage Constructor End");
    }

    private void OnStudentClicked(object sender, EventArgs e)
    {
        _isStudent = true;
        UpdateUI();
    }

    private void OnLecturerClicked(object sender, EventArgs e)
    {
        _isStudent = false;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_isStudent)
        {
            BtnStudent.BackgroundColor = Color.FromArgb("#3B82F6"); // Blue
            BtnStudent.TextColor = Colors.White;

            BtnLecturer.BackgroundColor = Colors.Transparent;
            BtnLecturer.TextColor = Color.FromArgb("#94A3B8"); // Gray

            LblWelcomeTitle.Text = "Student Login";
        }
        else
        {
            BtnLecturer.BackgroundColor = Color.FromArgb("#3B82F6"); // Blue
            BtnLecturer.TextColor = Colors.White;

            BtnStudent.BackgroundColor = Colors.Transparent;
            BtnStudent.TextColor = Color.FromArgb("#94A3B8"); // Gray

            LblWelcomeTitle.Text = "Lecturer Login";
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (_isStudent)
        {
            await Shell.Current.GoToAsync("//student/home");
        }
        else
        {
            await Shell.Current.GoToAsync("//lecturer/dashboard");
        }
    }
}