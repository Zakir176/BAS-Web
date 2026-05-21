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

        // Initialize Commands first to avoid NullReferenceException
        StudentCommand = new Command(OnStudentClicked);
        LecturerCommand = new Command(OnLecturerClicked);
        LoginCommand = new Command(async () => await OnLoginClicked());
        SignUpCommand = new Command(OnSignUpClicked);
        TogglePasswordCommand = new Command(OnTogglePasswordClicked);
        ForgotPasswordCommand = new Command(OnForgotPasswordClicked);
        RememberMeCommand = new Command(OnRememberMeClicked);

        UpdateUIState();
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
        if (!IsStudent)
        {
            IsStudent = true;
            UpdateUIState();
        }
    }

    private void OnLecturerClicked()
    {
        if (IsStudent)
        {
            IsStudent = false;
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
        
        // Ensure commands are updated if they depend on IsStudent
        ((Command)LoginCommand).ChangeCanExecute();
    }

    private async Task OnLoginClicked()
    {
        if (IsBusy) return;

        var email = EmailText?.Trim();
        var password = PasswordText?.Trim();

        if (string.IsNullOrWhiteSpace(email))
        {
            await Shell.Current.DisplayAlertAsync("Error", "Please enter your email.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            await Shell.Current.DisplayAlertAsync("Error", "Please enter your password.", "OK");
            return;
        }

        try
        {
            IsBusy = true;
            System.Diagnostics.Debug.WriteLine($"[Login] Attempting login for {email} (IsStudent: {IsStudent})");

            // Sign in with Email and Password
            var session = await _supabase.Auth.SignInWithPassword(email, password);
            if (session != null && session.User != null)
            {
                // Verify that the profile exists in the database
                bool profileExists = false;
                string profileType = IsStudent ? "Student" : "Lecturer";
                
                if (IsStudent)
                {
                    var response = await _supabase.From<Student>()
                        .Filter("email", Supabase.Postgrest.Constants.Operator.ILike, email)
                        .Get();
                    profileExists = response.Models.Count > 0;
                }
                else
                {
                    var userId = session.User.Id;
                    // First try by ID (most accurate if set correctly)
                    var response = await _supabase.From<LecturerProfile>()
                        .Where(x => x.Id == userId)
                        .Get();
                    
                    profileExists = response.Models.Count > 0;

                    // Fallback to Email if ID lookup failed (handles legacy/inconsistent records)
                    if (!profileExists)
                    {
                        System.Diagnostics.Debug.WriteLine($"[Login] ID lookup failed for lecturer {userId}, trying email {email}");
                        var emailResponse = await _supabase.From<LecturerProfile>()
                            .Filter("email", Supabase.Postgrest.Constants.Operator.ILike, email)
                            .Get();
                        profileExists = emailResponse.Models.Count > 0;
                    }
                }

                if (!profileExists)
                {
                    await _supabase.Auth.SignOut();
                    await Shell.Current.DisplayAlertAsync("Login Error", 
                        $"Your account was authenticated, but no {profileType} profile was found for {email}.\n\n" +
                        "If you are a lecturer, please ensure you selected the 'Lecturer' tab before logging in.", "OK");
                    return;
                }

                if (IsRememberMe)
                {
                    Preferences.Set(KeyRememberMe, true);
                    Preferences.Set(KeySavedUserId, email); 
                }
                else
                {
                    Preferences.Set(KeyRememberMe, false);
                    Preferences.Remove(KeySavedUserId);
                }

                if (IsStudent)
                {
                    await Shell.Current.GoToAsync("//student/studentDashboardTab/home");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[Login] Navigating to lecturer dashboard...");
                    await Shell.Current.GoToAsync("//lecturer/lecturerDashboardTab/dashboard");
                }
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Login Error", "Invalid email or password. Please try again.", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Login Error: {ex}");
            var message = ex.Message ?? "Invalid credentials or connection error.";
            if (message.Contains("Invalid login credentials"))
                message = "Invalid email or password. Please try again.";
            
            await Shell.Current.DisplayAlertAsync("Login Error", message, "OK");
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
