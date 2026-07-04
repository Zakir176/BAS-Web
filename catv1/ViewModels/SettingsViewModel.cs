using catv1.Services;
using System.Windows.Input;

namespace catv1.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    public ICommand LogoutCommand { get; }

    public bool BiometricLoginEnabled
    {
        get => Preferences.Get("BiometricLoginEnabled", false);
        set
        {
            Preferences.Set("BiometricLoginEnabled", value);
            OnPropertyChanged(nameof(BiometricLoginEnabled));
        }
    }

    public SettingsViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Settings";

        LogoutCommand = new Command(async () => await OnLogout());
    }

    private async Task OnLogout()
    {
        bool confirm = await Shell.Current.DisplayAlertAsync("Logout", "Are you sure you want to log out?", "Yes", "No");
        if (!confirm) return;

        try
        {
            await _authService.SignOutAsync();
            await Shell.Current.GoToAsync("//login");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Logout Error: {ex}");
            await Shell.Current.GoToAsync("//login");
        }
    }
}
