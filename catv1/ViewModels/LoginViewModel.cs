using System.Windows.Input;
using catv1.Models;
using catv1.Services;

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

    private readonly IAuthService _authService;
    private readonly IProfileService _profileService;
    private readonly Plugin.Fingerprint.Abstractions.IFingerprint _fingerprint;

    public LoginViewModel(IAuthService authService, IProfileService profileService, Plugin.Fingerprint.Abstractions.IFingerprint fingerprint)
    {
        _authService = authService;
        _profileService = profileService;
        _fingerprint = fingerprint;
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
            var session = await _authService.SignInAsync(email, password);
            if (session != null && session.User != null)
            {
                // Verify that the profile exists in the database
                bool profileExists = false;
                string profileType = IsStudent ? "Student" : "Lecturer";
                
                if (IsStudent)
                {
                    var student = await _profileService.GetStudentByEmailAsync(email);
                    profileExists = student != null;
                }
                else
                {
                    var userId = session.User.Id;
                    // First try by ID (most accurate if set correctly)
                    var lecturer = await _profileService.GetLecturerByIdAsync(userId ?? string.Empty);
                    profileExists = lecturer != null;

                    // Fallback to Email if ID lookup failed (handles legacy/inconsistent records)
                    if (!profileExists)
                    {
                        System.Diagnostics.Debug.WriteLine($"[Login] ID lookup failed for lecturer {userId}, trying email {email}");
                        var emailLecturer = await _profileService.GetLecturerByEmailAsync(email);
                        profileExists = emailLecturer != null;
                    }
                }

                if (!profileExists)
                {
                    await _authService.SignOutAsync();
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

    public async Task CheckAutoLoginAsync()
    {
        if (IsBusy) return;

        // Initialize Supabase client to load session from SecureStorage
        await _authService.InitializeAsync();

        // Check if there is an active session from the session handler
        if (_authService.CurrentSession != null && _authService.CurrentUser != null)
        {
            try
            {
                IsBusy = true;

                // Biometric Check
                var biometricEnabled = Preferences.Get("BiometricLoginEnabled", false);
                if (biometricEnabled)
                {
                    var isAvailable = await _fingerprint.IsAvailableAsync();
                    if (isAvailable)
                    {
                        var request = new Plugin.Fingerprint.Abstractions.AuthenticationRequestConfiguration("Login", "Please authenticate to continue");
                        var result = await _fingerprint.AuthenticateAsync(request);
                        if (!result.Authenticated)
                        {
                            System.Diagnostics.Debug.WriteLine("[AutoLogin] Biometric authentication failed or canceled.");
                            await _authService.SignOutAsync();
                            IsBusy = false;
                            return;
                        }
                    }
                }

                var user = _authService.CurrentUser;
                var email = user.Email;
                System.Diagnostics.Debug.WriteLine($"[AutoLogin] Active session found for {email}");

                // Determine user role (student or lecturer)
                string role = "student";
                if (user.UserMetadata != null && user.UserMetadata.ContainsKey("role"))
                {
                    role = user.UserMetadata["role"]?.ToString()?.ToLower() ?? "student";
                }

                System.Diagnostics.Debug.WriteLine($"[AutoLogin] User role: {role}");

                if (role == "lecturer")
                {
                    IsStudent = false;
                    UpdateUIState();
                    System.Diagnostics.Debug.WriteLine($"[AutoLogin] Auto-login routing to lecturer dashboard...");
                    await Shell.Current.GoToAsync("//lecturer/lecturerDashboardTab/dashboard");
                }
                else
                {
                    IsStudent = true;
                    UpdateUIState();
                    System.Diagnostics.Debug.WriteLine($"[AutoLogin] Auto-login routing to student dashboard...");
                    await Shell.Current.GoToAsync("//student/studentDashboardTab/home");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AutoLogin] Auto-login routing failed: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
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
        var email = await Shell.Current.DisplayPromptAsync(
            "Forgot Password",
            "Enter your institutional email address and we'll send you a reset link.",
            placeholder: "your@university.edu",
            keyboard: Keyboard.Email);

        if (string.IsNullOrWhiteSpace(email)) return;

        try
        {
            IsBusy = true;
            await _authService.ResetPasswordAsync(email.Trim());
            await Shell.Current.DisplayAlertAsync(
                "Reset Link Sent",
                $"A password reset link has been sent to {email.Trim()}. Please check your inbox (and spam folder).",
                "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ForgotPassword] Error: {ex}");
            await Shell.Current.DisplayAlertAsync(
                "Error",
                "Failed to send reset link. Please check the email address and try again.",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnRememberMeClicked()
    {
        IsRememberMe = !IsRememberMe;
    }
}

