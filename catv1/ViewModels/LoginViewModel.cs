using System.Windows.Input;
using catv1.Models;

namespace catv1.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private bool _isStudent = true;
    private bool _isRememberMe;
    private bool _isPassword;
    private string _emailText = string.Empty;
    private string _passwordText = string.Empty;
    private string _welcomeTitle = string.Empty;
    private Color _studentBtnBgColor = Colors.Transparent;
    private Color _studentBtnTextColor = Colors.Transparent;
    private Color _lecturerBtnBgColor = Colors.Transparent;
    private Color _lecturerBtnTextColor = Colors.Transparent;

    private const string KeyRememberMe = "RememberMe";
    private const string KeySavedUserId = "SavedUserId";

    private readonly Supabase.Client _supabase;

    public LoginViewModel(Supabase.Client supabase)
    {
        _supabase = supabase;
        // Initialize state
        _isStudent = true;
        _isPassword = true; // Default to hidden password

        IsRememberMe = Preferences.Get(KeyRememberMe, false);
        if (IsRememberMe)
        {
            EmailText = Preferences.Get(KeySavedUserId, string.Empty);
        }

        UpdateUIState();

        // Initialize Commands
        StudentCommand = new Command(OnStudentClicked);
        LecturerCommand = new Command(OnLecturerClicked);
        LoginCommand = new Command(async () => await OnLoginClicked());
        SignUpCommand = new Command(OnSignUpClicked);
        TogglePasswordCommand = new Command(OnTogglePasswordClicked);
        ForgotPasswordCommand = new Command(OnForgotPasswordClicked);
        RememberMeCommand = new Command(OnRememberMeClicked);
    }

    public bool IsStudent
    {
        get => _isStudent;
        set => SetProperty(ref _isStudent, value);
    }

    public string EmailText
    {
        get => _emailText;
        set => SetProperty(ref _emailText, value);
    }

    public bool IsRememberMe
    {
        get => _isRememberMe;
        set => SetProperty(ref _isRememberMe, value);
    }


    // Binding for Entry Password
    public string PasswordText
    {
        get => _passwordText;
        set => SetProperty(ref _passwordText, value);
    }

    // Binding for Entry IsPassword property
    public bool IsPassword
    {
        get => _isPassword;
        set => SetProperty(ref _isPassword, value);
    }

    // UI Bindings
    public string WelcomeTitle
    {
        get => _welcomeTitle;
        set => SetProperty(ref _welcomeTitle, value);
    }


    public Color StudentBtnBgColor
    {
        get => _studentBtnBgColor;
        set => SetProperty(ref _studentBtnBgColor, value);
    }

    public Color StudentBtnTextColor
    {
        get => _studentBtnTextColor;
        set => SetProperty(ref _studentBtnTextColor, value);
    }

    public Color LecturerBtnBgColor
    {
        get => _lecturerBtnBgColor;
        set => SetProperty(ref _lecturerBtnBgColor, value);
    }

    public Color LecturerBtnTextColor
    {
        get => _lecturerBtnTextColor;
        set => SetProperty(ref _lecturerBtnTextColor, value);
    }

    public ICommand StudentCommand { get; }
    public ICommand LecturerCommand { get; }
    public ICommand LoginCommand { get; }
    public ICommand SignUpCommand { get; }
    public ICommand TogglePasswordCommand { get; }
    public ICommand ForgotPasswordCommand { get; }
    public ICommand RememberMeCommand { get; }

    private void OnStudentClicked()
    {
        if (!_isStudent)
        {
            _isStudent = true;
            UpdateUIState();
        }
    }

    private void OnLecturerClicked()
    {
        if (_isStudent)
        {
            _isStudent = false;
            UpdateUIState();
        }
    }

    private void UpdateUIState()
    {
        if (_isStudent)
        {
            StudentBtnBgColor = Color.FromArgb("#3B82F6"); // Blue
            StudentBtnTextColor = Colors.White;

            LecturerBtnBgColor = Colors.Transparent;
            LecturerBtnTextColor = Color.FromArgb("#64748B"); // Gray

            WelcomeTitle = "Student Login";
        }
        else
        {
            LecturerBtnBgColor = Color.FromArgb("#3B82F6"); // Blue
            LecturerBtnTextColor = Colors.White;

            StudentBtnBgColor = Colors.Transparent;
            StudentBtnTextColor = Color.FromArgb("#64748B"); // Gray

            WelcomeTitle = "Lecturer Login";
        }
    }

    private async Task OnLoginClicked()
    {
        if (string.IsNullOrWhiteSpace(EmailText))
        {
            await Shell.Current.DisplayAlertAsync("Error", "Please enter your email.", "OK");
            return;
        }


        if (string.IsNullOrWhiteSpace(PasswordText))
        {
            await Shell.Current.DisplayAlertAsync("Error", "Please enter your password.", "OK");
            return;
        }

        try
        {
            IsBusy = true;
            // Sign in with Email and Password
            var session = await _supabase.Auth.SignInWithPassword(EmailText, PasswordText);
            if (session != null)
            {
                // Verify that the profile exists in the database
                bool profileExists = false;
                if (_isStudent)
                {
                    var response = await _supabase.From<Student>()
                        .Where(x => x.Email == EmailText)
                        .Get();
                    profileExists = response.Models.Any();
                }
                else
                {
                    var response = await _supabase.From<LecturerProfile>()
                        .Where(x => x.Email == EmailText)
                        .Get();
                    profileExists = response.Models.Any();
                }

                if (!profileExists)
                {
                    await _supabase.Auth.SignOut();
                    await Shell.Current.DisplayAlertAsync("Login Error", "Your account was authenticated, but no profile was found. Please register again or contact support.", "OK");
                    return;
                }

                if (IsRememberMe)
                {
                    Preferences.Set(KeyRememberMe, true);
                    Preferences.Set(KeySavedUserId, EmailText); // Save Email instead of ID
                }
                else
                {
                    Preferences.Set(KeyRememberMe, false);
                    Preferences.Remove(KeySavedUserId);
                }

                if (_isStudent)
                {
                    await Shell.Current.GoToAsync("//student/studentDashboardTab/home");
                }
                else
                {
                    await Shell.Current.GoToAsync("//lecturer/lecturerDashboardTab/dashboard");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Login Error: {ex}");
            await Shell.Current.DisplayAlertAsync("Login Error", "Invalid credentials or connection error.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnSignUpClicked()
    {
        try
        {
            await Shell.Current.GoToAsync("signup");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SignUp Error: {ex}");
            await Shell.Current.DisplayAlertAsync("Error", "Unable to open sign up page.", "OK");
        }
    }

    private void OnTogglePasswordClicked()
    {
        IsPassword = !IsPassword;
    }

    private async void OnForgotPasswordClicked()
    {
        await Shell.Current.DisplayAlertAsync("Forgot Password", "Please contact your system administrator to reset your password.\n\nSupport: admin@university.edu", "OK");
    }

    private void OnRememberMeClicked()
    {
        IsRememberMe = !IsRememberMe;
    }
}
