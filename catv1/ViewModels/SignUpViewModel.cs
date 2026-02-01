using System.Windows.Input;
using catv1.Models;
using Supabase.Gotrue;

namespace catv1.ViewModels;

public class SignUpViewModel : BaseViewModel
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _email = string.Empty;
    private string _id = string.Empty;
    private string _department = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private bool _isStudent = true;
    private bool _isPassword = true;

    public string FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }

    public string LastName
    {
        get => _lastName;
        set => SetProperty(ref _lastName, value);
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

    public string Department
    {
        get => _department;
        set => SetProperty(ref _department, value);
    }

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
    public string IdPlaceholder => IsStudent ? "e.g. 210984" : "e.g. L00123";

    public string DeptFieldLabel => IsStudent ? "CLASS SECTION" : "DEPARTMENT";
    public string DeptPlaceholder => IsStudent ? "CS101" : "e.g. Computer Science";

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
    public ICommand SignUpCommand { get; }
    public ICommand LoginCommand { get; }
    public ICommand TogglePasswordCommand { get; }

    private readonly Supabase.Client _supabase;

    public SignUpViewModel(Supabase.Client supabase)
    {
        _supabase = supabase;

        StudentCommand = new Command(() => IsStudent = true);
        LecturerCommand = new Command(() => IsStudent = false);
        SignUpCommand = new Command(OnSignUpClicked);
        LoginCommand = new Command(OnLoginClicked);
        TogglePasswordCommand = new Command(() => IsPassword = !IsPassword);
    }

    private async void OnSignUpClicked()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
        {
            await Shell.Current.DisplayAlertAsync("Error", "Please enter your first and last name.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            await Shell.Current.DisplayAlertAsync("Error", "Please enter your email.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Id))
        {
            await Shell.Current.DisplayAlertAsync("Error", $"Please enter your {(IsStudent ? "Student ID" : "Registration ID")}.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Department))
        {
            await Shell.Current.DisplayAlertAsync("Error", $"Please enter your {(IsStudent ? "Class Section" : "Department")}.", "OK");
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

            var options = new SignUpOptions
            {
                Data = new Dictionary<string, object>
                {
                    { "first_name", FirstName },
                    { "last_name", LastName },
                    { "student_id", Id },
                    { "role", IsStudent ? "student" : "lecturer" }
                }
            };

            var session = await _supabase.Auth.SignUp(Email, Password, options);

            if (session?.User?.Id != null)
            {
                if (IsStudent)
                {
                    var student = new Student
                    {
                        Id = session.User.Id,
                        StudentId = Id,
                        FirstName = FirstName,
                        LastName = LastName,
                        Email = Email,
                        Department = Department,
                        IsPresent = false
                    };
                    await _supabase.From<Student>().Insert(student);
                }
                else
                {
                    var lecturerProfile = new LecturerProfile
                    {
                        Id = session.User.Id,
                        RegistrationId = Id,
                        FirstName = FirstName,
                        LastName = LastName,
                        Email = Email,
                        Department = Department
                    };
                    await _supabase.From<LecturerProfile>().Insert(lecturerProfile);
                }

                await Shell.Current.DisplayAlertAsync("Success", "Account created successfully! Please login.", "OK");
                await Shell.Current.GoToAsync("//login");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Registration Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnLoginClicked()
    {
        await Shell.Current.GoToAsync("//login");
    }
}
