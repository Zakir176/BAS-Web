using Microsoft.Maui.Controls;

namespace catv1.Views;

public partial class LoginPage : ContentPage
{
    private bool _isStudent = true; // Default to Student
    private bool _isRememberMe = false;
    private const string KeyRememberMe = "RememberMe";
    private const string KeySavedUserId = "SavedUserId";

    public LoginPage()
    {
        InitializeComponent();
        
        // Initialize Remember Me state
        _isRememberMe = Preferences.Get(KeyRememberMe, false);
        LblRememberMeCheck.IsVisible = _isRememberMe;

        if (_isRememberMe)
        {
            EntryId.Text = Preferences.Get(KeySavedUserId, string.Empty);
        }

        UpdateUI(); // Set initial state
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
            BtnLecturer.TextColor = Color.FromArgb("#64748B"); // Gray

            LblWelcomeTitle.Text = "Student Login";
            LblIdField.Text = "STUDENT ID (SIN)";
            EntryId.Placeholder = "e.g. 210984";
        }
        else
        {
            BtnLecturer.BackgroundColor = Color.FromArgb("#3B82F6"); // Blue
            BtnLecturer.TextColor = Colors.White;

            BtnStudent.BackgroundColor = Colors.Transparent;
            BtnStudent.TextColor = Color.FromArgb("#64748B"); // Gray

            LblWelcomeTitle.Text = "Lecturer Login";
            LblIdField.Text = "LECTURER ID";
            EntryId.Placeholder = "e.g. L00123";
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // Simple validation visualization (optional)
        if (string.IsNullOrWhiteSpace(EntryId.Text))
        {
            await DisplayAlert("Error", "Please enter your ID.", "OK");
            return;
        }

        // Save or Clear Preferences based on Remember Me
        if (_isRememberMe)
        {
            Preferences.Set(KeyRememberMe, true);
            Preferences.Set(KeySavedUserId, EntryId.Text);
        }
        else
        {
            Preferences.Set(KeyRememberMe, false);
            Preferences.Remove(KeySavedUserId);
        }

        if (_isStudent)
        {
            await Shell.Current.GoToAsync("//student/dashboardTab/home");
        }
        else
        {
            await Shell.Current.GoToAsync("//lecturer/dashboardTab/dashboard");
        }
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("signup");
    }

    private void OnTogglePasswordClicked(object sender, EventArgs e)
    {
        EntryPassword.IsPassword = !EntryPassword.IsPassword;
    }

    private async void OnForgotPasswordClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Forgot Password", "Please contact your system administrator to reset your password.\n\nSupport: admin@university.edu", "OK");
    }

    private void OnRememberMeClicked(object sender, EventArgs e)
    {
        _isRememberMe = !_isRememberMe;
        LblRememberMeCheck.IsVisible = _isRememberMe;
    }
}