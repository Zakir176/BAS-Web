using System.Windows.Input;
using System.Collections.ObjectModel;
using catv1.Models;
using catv1.Services;
using Supabase.Gotrue;

namespace catv1.ViewModels;

public class SignUpViewModel : BaseViewModel
{
    private string _fullName = string.Empty;
    private string _email = string.Empty;
    private string _id = string.Empty;
    private Department? _selectedDepartment;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private bool _isStudent = true;
    private bool _isPassword = true;

    public string FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public Department? SelectedDepartment
    {
        get => _selectedDepartment;
        set => SetProperty(ref _selectedDepartment, value);
    }

    public ObservableCollection<Department> Departments { get; } = new();

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => SetProperty(ref _confirmPassword, value);
    }

    public bool IsStudent
    {
        get => _isStudent;
        set
        {
            if (SetProperty(ref _isStudent, value))
            {
                OnPropertyChanged(nameof(IsLecturer));
                OnPropertyChanged(nameof(SignUpTitle));
                OnPropertyChanged(nameof(IdFieldLabel));
                OnPropertyChanged(nameof(IdPlaceholder));
                OnPropertyChanged(nameof(DeptFieldLabel));
                OnPropertyChanged(nameof(DeptPlaceholder));
                OnPropertyChanged(nameof(StudentBtnBgColor));
                OnPropertyChanged(nameof(LecturerBtnBgColor));
                OnPropertyChanged(nameof(StudentBtnTextColor));
                OnPropertyChanged(nameof(LecturerBtnTextColor));
            }
        }
    }

    public bool IsLecturer => !IsStudent;

    public string SignUpTitle => IsStudent ? "Student Registration" : "Lecturer Registration";
    public string IdFieldLabel => IsStudent ? "STUDENT ID (SIN)" : "LECTURER ID";
    public string IdPlaceholder => IsStudent ? "e.g. 24000001" : "e.g. L00123";

    public string DeptFieldLabel => IsStudent ? "CLASS SECTION" : "DEPARTMENT";
    public string DeptPlaceholder => IsStudent ? "CS" : "e.g. Computer Science";

    public Color StudentBtnBgColor => IsStudent ? Color.FromArgb("#3B82F6") : Colors.Transparent;
    public Color LecturerBtnBgColor => IsLecturer ? Color.FromArgb("#3B82F6") : Colors.Transparent;
    public Color StudentBtnTextColor => IsStudent ? Colors.White : Color.FromArgb("#64748B");
    public Color LecturerBtnTextColor => IsLecturer ? Colors.White : Color.FromArgb("#64748B");

    public bool IsPassword
    {
        get => _isPassword;
        set => SetProperty(ref _isPassword, value);
    }

    public ICommand StudentCommand { get; }
    public ICommand LecturerCommand { get; }
    public ICommand SetRoleCommand { get; }
    public ICommand SignUpCommand { get; }
    public ICommand LoginCommand { get; }
    public ICommand TogglePasswordCommand { get; }

    private readonly IAuthService _authService;
    private readonly IProfileService _profileService;

    public SignUpViewModel(IAuthService authService, IProfileService profileService)
    {
        _authService = authService;
        _profileService = profileService;

        StudentCommand = new Command(() => IsStudent = true);
        LecturerCommand = new Command(() => IsStudent = false);
        SetRoleCommand = new Command<string>(role => IsStudent = role == "Student");
        SignUpCommand = new Command(OnSignUpClicked);
        LoginCommand = new Command(OnLoginClicked);
        TogglePasswordCommand = new Command(() => IsPassword = !IsPassword);

        _ = LoadDepartmentsAsync();
    }

    private async Task LoadDepartmentsAsync()
    {
        try
        {
            var departments = await _profileService.GetAllDepartmentsAsync();
            Departments.Clear();
            foreach (var dept in departments)
            {
                Departments.Add(dept);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading departments: {ex.Message}");
        }
    }

    private async void OnSignUpClicked()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(FullName))
        {
            await Shell.Current.DisplayAlertAsync("Error", "Please enter your full name.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            await Shell.Current.DisplayAlertAsync("Error", "Please enter your email.", "OK");
            return;
        }

        if (IsStudent && string.IsNullOrWhiteSpace(Id))
        {
            await Shell.Current.DisplayAlertAsync("Error", "Please enter your Student ID.", "OK");
            return;
        }

        if (SelectedDepartment == null)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"Please select a {(IsStudent ? "Class Section" : "Department")}.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlertAsync("Error", "Please enter a password.", "OK");
            return;
        }

        if (Password != ConfirmPassword)
        {
            await Shell.Current.DisplayAlertAsync("Error", "Passwords do not match.", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            // 1. Pre-signup checks (Check public tables first)
            var alreadyExists = await CheckExistingUser(Email, Id, IsStudent);
            if (alreadyExists) return;

            var userData = new Dictionary<string, object>
            {
                { "full_name", FullName },
                { "role", IsStudent ? "student" : "lecturer" }
            };

            if (IsStudent)
            {
                userData.Add("student_number", Id);
            }

            var options = new SignUpOptions
            {
                Data = userData
            };

            var session = await _authService.SignUpAsync(Email, Password, userData);

            if (session?.User?.Id != null)
            {
                if (IsStudent)
                {
                    var student = new Student
                    {
                        Id = session.User.Id, // Ensure we use the Auth ID if possible, or let DB generate
                        StudentNumber = Id,
                        FullName = FullName,
                        DepartmentId = SelectedDepartment.Id,
                        Email = Email,
                        QrCode = Id,
                        Phone = "",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _profileService.InsertStudentAsync(student);
                }
                else
                {
                    var lecturerProfile = new LecturerProfile
                    {
                        Id = session.User.Id,
                        RegistrationId = null, // ID is now DB-generated
                        FullName = FullName,
                        Email = Email,
                        DepartmentId = SelectedDepartment.Id
                    };
                    await _profileService.InsertLecturerAsync(lecturerProfile);
                }

                await Shell.Current.DisplayAlertAsync("Success", "Account created successfully! Please login.", "OK");
                await Shell.Current.GoToAsync("//login");
            }
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("User already exists"))
            {
                await Shell.Current.DisplayAlertAsync("Registration Error", 
                    "An account with this email already exists. " +
                    "If you previously tried to sign up and it failed, please try logging in or use a different email.", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Registration Error", ex.Message, "OK");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task<bool> CheckExistingUser(string email, string id, bool isStudent)
    {
        try
        {
            // Check both tables for email (since email must be unique across app)
            var lecturerWithEmail = await _profileService.GetLecturerByEmailAsync(email);
            if (lecturerWithEmail != null)
            {
                await Shell.Current.DisplayAlertAsync("Error", "This email is already registered as a lecturer.", "OK");
                return true;
            }

            var studentWithEmail = await _profileService.GetStudentByEmailAsync(email);
            if (studentWithEmail != null)
            {
                await Shell.Current.DisplayAlertAsync("Error", "This email is already registered as a student.", "OK");
                return true;
            }

            // Check specific ID uniqueness
            if (isStudent)
            {
                var studentWithId = await _profileService.GetStudentByStudentNumberAsync(id);
                if (studentWithId != null)
                {
                    await Shell.Current.DisplayAlertAsync("Error", "This Student ID is already in use.", "OK");
                    return true;
                }
            }
            else
            {
                // No longer checking RegistrationId as it's not provided by user
            }

            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Pre-signup check failed: {ex.Message}");
            return false; // Proceed anyway if check fails, Auth will catch it
        }
    }

    private async void OnLoginClicked()
    {
        await Shell.Current.GoToAsync("//login");
    }
}

